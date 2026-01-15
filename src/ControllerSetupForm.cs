using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Threading;
using System.Diagnostics;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ControllerSetupForm : Form
    {
        private NCIController _stick;
        private int _step;
        private bool _setupPower;
        private bool[] _initButtonState;
        private int[] _initSliders;
        private bool[] _buttons;
        private int[] _sliders;
        private List<int> _useButtons;
        private List<int> _useSliders;
        private int _notchPos;
        private ControllerProfile _profile;
        private int _mode;
        private int _useAxis;
        private int _axisReverse;
        private int _axisMin;
        private int _axisMax;
        private int[] _powerButtons = new int[0];
        private List<List<bool>> _powerButtonStatus = new List<List<bool>>();
        private int[] _powerAxises = new int[0];
        private List<List<int>> _powerAxisStatus = new List<List<int>>();
        private int[] _brakeButtons = new int[0];
        private List<List<bool>> _brakeButtonStatus = new List<List<bool>>();
        private int[] _brakeAxises = new int[0];
        private List<List<int>> _brakeAxisStatus = new List<List<int>>();
        public ControllerSetupForm(NCIController controller, ControllerProfile profile, bool setupPower)
        {
            _stick = controller;
            InitializeComponent();
            _initButtonState = _stick.GetButtonsSafe();
            _initSliders = profile.GetSliders(_stick);
            timer1.Start();
            this._setupPower = setupPower;
            this._profile = profile;
            _useButtons = new List<int>();
            _useSliders = new List<int>();
            countLabel.Text = "";
            _step = -1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _buttons = _stick.GetButtonsSafe();
            _sliders = _profile.GetSliders(_stick);
            if(_mode == 0)
            {
                if(_step == 0)
                {
                    for(int i = 0;i < _sliders.Length;i++)
                    {
                        if(Math.Abs(_sliders[i] - _initSliders[i]) > 400)
                        {
                            _useAxis = i;
                            _step++;
                            infoLabel.Text = (_setupPower ? "力行" : "制動") + "を切にして次へを押してください。";
                            break;
                        }
                    }
                }else if (_step == 1)
                {
                    _axisMin = _sliders[_useAxis];
                    countLabel.Text = "現在の値:" + _sliders[_useAxis];
                }else if(_step == 2)
                {
                    _axisMax = _sliders[_useAxis];
                    countLabel.Text = "現在の値:" + _sliders[_useAxis];
                }
                else if (_step == 3)
                {
                    _axisReverse = _sliders[_useAxis];
                    countLabel.Text = "現在の値:" + _sliders[_useAxis];
                } else if (_step == 4)
                {
                    countLabel.Text = "現在のノッチ:" + (_setupPower ? _profile.GetPower(_stick, 5, 5) : _profile.GetBrake(_stick, 9));
                }
            }
            else if (_mode == 1)
            {
                if (_step == 0)
                {
                    for (int i = 0; i < _buttons.Length; i++)
                    {
                        if (_initButtonState[i] != _buttons[i] && !_useButtons.Contains(i))
                        {
                            _useButtons.Add(i);
                            _useButtons.Sort();
                        }
                    }
                    for (int i = 0; i < _sliders.Length; i++)
                    {
                        if (_initSliders[i] != _sliders[i] && !_useSliders.Contains(i))
                        {
                            _useSliders.Add(i);
                            _useSliders.Sort();
                        }
                    }
                }else if(_step == 2)
                {
                    countLabel.Text = "現在のノッチ:" + (_setupPower ? _profile.GetPower(_stick, 99, 0) : _profile.GetBrake(_stick, 99));
                    InaccuracyModeCheckBox.Visible = true;
                }
                else if (_step == 3)
                {
                    Close();
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if(_mode == 0)
            {
                if (_step == 1)
                {
                    _step++;
                    infoLabel.Text = (_setupPower ? "力行" : "制動") + "を最大にして次へを押してください。";

                    if (!_setupPower)
                    {
                        _step++;
                    }
                }
                else if (_step == 2)
                {
                    _step++;
                    if (_setupPower)
                    {
                        infoLabel.Text = "力行を逆回しの方向に最大にして次へを押してください。(逆回しを設定しない場合は切の位置にしてください)";
                    }
                }
                else if (_step == 3)
                {
                    _step++;
                    if (_setupPower)
                    {
                        _profile.PowerButtons = new int[0];
                        _profile.PowerButtonStatus = new List<List<bool>>();
                        _profile.PowerAxises = new int[] { _useAxis };
                        // _axisReverse < _axisMin < _axisMax  or  _axisMax < _axisMin < _axisReverseになることを保証する
                        if (_axisMin < _axisMax)
                        {
                            if (_axisReverse > _axisMin)
                            {
                                _axisReverse = _axisMin;
                            }
                        } else if (_axisMin > _axisMax)
                        {
                            if (_axisReverse < _axisMin)
                            {
                                _axisReverse = _axisMin;
                            }
                        }
                            _profile.PowerAxisStatus = new List<List<int>>(new List<int>[] { new List<int>(new int[] { _axisMin, _axisMax, _axisReverse }) });
                        _profile.InaccuracyModePower = false;
                    }
                    else
                    {
                        _profile.BrakeButtons = new int[0];
                        _profile.BrakeButtonStatus = new List<List<bool>>();
                        _profile.BrakeAxises = new int[] { _useAxis };
                        _profile.BrakeAxisStatus = new List<List<int>>(new List<int>[] { new List<int>(new int[] { _axisMin, _axisMax }) });
                        _profile.InaccuracyModeBrake = false;
                    }
                    _profile.CalcDuplicated();
                    countLabel.Text = "";
                    infoLabel.Text = "完了しました。次へを押して終了します。";
                    deadZoneLabel.Visible = true;
                    deadZoneNumericUpDown.Visible = true;
                }
                else if (_step == 4)
                {
                    Close();
                }
            }
            else if (_mode == 1)
            {
                if (_step == 0)
                {
                    if (_setupPower)
                    {
                        _powerButtons = _useButtons.ToArray();
                        _powerButtonStatus = new List<List<bool>>();
                        _powerAxises = _useSliders.ToArray();
                        _powerAxisStatus = new List<List<int>>();
                    }
                    else
                    {
                        _brakeButtons = _useButtons.ToArray();
                        _brakeButtonStatus = new List<List<bool>>();
                        _brakeAxises = _useSliders.ToArray();
                        _brakeAxisStatus = new List<List<int>>();
                    }
                    _step++;
                    infoLabel.Text = (_setupPower ? "力行" : "制動") + "を切から順番に入れ、次へをクリックして下さい。\n終了時は2回次へをクリックして下さい。";
                    countLabel.Text = "登録するノッチ:0";
                }
                else if (_step == 1)
                {
                    if (_setupPower)
                    {
                        if (_powerButtonStatus.Count > 0)
                        {
                            int k = _powerButtonStatus.Count - 1;
                            int match = 0;
                            for (int i = 0; i < _powerButtons.Length; i++)
                            {
                                if (_powerButtonStatus[k][i] == _buttons[_powerButtons[i]])
                                {
                                    match++;
                                }
                            }
                            for (int i = 0; i < _powerAxises.Length; i++)
                            {
                                if (_powerAxisStatus[k][i] == _sliders[_powerAxises[i]])
                                {
                                    match++;
                                }
                            }
                            if (match == _powerButtons.Length + _powerAxises.Length)
                            {
                                //終了
                                if (k == _notchPos - 1)
                                {
                                    _step++;
                                    infoLabel.Text = "完了しました。次へを押して終了";
                                    _profile.PowerButtons = _powerButtons;
                                    _profile.PowerButtonStatus = _powerButtonStatus;
                                    _profile.PowerAxises = _powerAxises;
                                    _profile.PowerAxisStatus = _powerAxisStatus;
                                    _profile.InaccuracyModePower = false;
                                    _profile.CalcDuplicated();
                                    return;
                                }
                            }
                        }
                        _powerButtonStatus.Add(new List<bool>(new bool[_powerButtons.Length]));
                        _powerAxisStatus.Add(new List<int>(new int[_powerAxises.Length]));
                        for (int i = 0; i < _powerButtons.Length; i++)
                        {
                            _powerButtonStatus[_notchPos][i] =
                                _buttons[_powerButtons[i]];
                        }
                        for (int i = 0; i < _powerAxises.Length; i++)
                        {
                            _powerAxisStatus[_notchPos][i] = _sliders[_powerAxises[i]];
                        }
                        _profile.CalcDuplicated();
                    }
                    else
                    {
                        if (_brakeButtonStatus.Count > 0)
                        {
                            int k = _brakeButtonStatus.Count - 1;
                            int match = 0;
                            for (int i = 0; i < _brakeButtons.Length; i++)
                            {
                                if (_brakeButtonStatus[k][i] == _buttons[_brakeButtons[i]])
                                {
                                    match++;
                                }
                            }
                            for (int i = 0; i < _brakeAxises.Length; i++)
                            {
                                if (_brakeAxisStatus[k][i] == _sliders[_brakeAxises[i]])
                                {
                                    match++;
                                }
                            }
                            if (match == _brakeButtons.Length + _brakeAxises.Length)
                            {
                                //終了
                                if (k == _notchPos - 1)
                                {
                                    _step++;
                                    infoLabel.Text = "完了しました。次へを押して終了";
                                    _profile.BrakeButtons = _brakeButtons;
                                    _profile.BrakeButtonStatus = _brakeButtonStatus;
                                    _profile.BrakeAxises = _brakeAxises;
                                    _profile.BrakeAxisStatus = _brakeAxisStatus;
                                    _profile.InaccuracyModeBrake = false;
                                    _profile.CalcDuplicated();
                                    return;
                                }
                            }
                        }
                        _brakeButtonStatus.Add(new List<bool>(new bool[_brakeButtons.Length]));
                        _brakeAxisStatus.Add(new List<int>(new int[_brakeAxises.Length]));
                        for (int i = 0; i < _brakeButtons.Length; i++)
                        {
                            _brakeButtonStatus[_notchPos][i] =
                                _buttons[_brakeButtons[i]];
                        }
                        for (int i = 0; i < _brakeAxises.Length; i++)
                        {
                            _brakeAxisStatus[_notchPos][i] = _sliders[_brakeAxises[i]];
                        }
                    }
                    _notchPos++;
                    countLabel.Text = "登録するノッチ:" + _notchPos + "";
                }
                else if (_step == 2)
                {
                    _step++;
                }
            }
        }

        private void cacelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void useAxisButton_Click(object sender, EventArgs e)
        {
            _mode = 0;
            _step = 0;
            useAxisButton.Hide();
            useButtonAxisButton.Hide();
            infoLabel.Text = "使用する軸を動かして下さい。";
        }

        private void useButtonAxisButton_Click(object sender, EventArgs e)
        {
            _mode = 1;
            _step = 0;
            useAxisButton.Hide();
            useButtonAxisButton.Hide();
            infoLabel.Text = "一度すべての" + (_setupPower ? "力行" : "制動") + "に入れてください。";
        }

        private void InaccuracyModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(_setupPower)
            {
                this._profile.InaccuracyModeBrake = InaccuracyModeCheckBox.Checked;
            }else
            {
                this._profile.InaccuracyModeBrake = InaccuracyModeCheckBox.Checked;
            }
        }

        private void deadZoneNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_setupPower)
            {
                this._profile.PowerAxisDeadZone = (int) deadZoneNumericUpDown.Value;
            }
            else
            {
                this._profile.BrakeAxisDeadZone = (int) deadZoneNumericUpDown.Value;
            }
        }
    }
}
