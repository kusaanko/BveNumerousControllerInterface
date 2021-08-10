using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Diagnostics;
using Mackoy.Bvets;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class NumerousControllerInterface : IInputDevice
    {
        public static DirectInput Input;
        public static List<NCIController> Controllers;

        public static Settings SettingsInstance = null;
        public static ConfigForm ConfigFormInstance;

        public event InputEventHandler KeyDown;
        public event InputEventHandler KeyUp;
        public event InputEventHandler LeverMoved;

        private Dictionary<NCIController, int> _prePowerLevel;
        private Dictionary<NCIController, int> _preBreakLevel;
        private Dictionary<NCIController, List<int>> _preButtons;

        private int _onePowerMax;
        private int _oneBreakMax;
        private int _twoPowerMax;
        private int _twoBreakMax;
        private int _powerNotch;
        private int _breakNotch;

        private static int s_preControllerCount;

        private static string s_masterController;

        public static Timer TimerController;
        private bool _isUpdateController;
        private bool _isDisposeRequested;

        public NumerousControllerInterface()
        {
            ButtonFeature.Initialize();
            Controllers = new List<NCIController>();
            _prePowerLevel = new Dictionary<NCIController, int>();
            _preBreakLevel = new Dictionary<NCIController, int>();
            _preButtons = new Dictionary<NCIController, List<int>>();
            s_masterController = "";
            s_preControllerCount = -1;
            if (TimerController == null)
            {
                TimerController = new Timer();
                TimerController.Interval = 1000;
                TimerController.Tick += new System.EventHandler(TimerTick);
            }
            TimerController.Start();
            System.Threading.Thread mainThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                while (!_isDisposeRequested)
                {
                    MainTick();
                }
            }));
            mainThread.Start();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            if (ConfigFormInstance != null && !ConfigFormInstance.IsDisposed && (ConfigFormInstance.ControllerSetupForm == null || ConfigFormInstance.ControllerSetupForm.IsDisposed)) 
            {
                GetAllControllers();
            }
        }

        public static void GetAllControllers()
        {
            Controllers.Clear();
            ControllerProfile.GetAllControllers();
            foreach(NCIController controller in ControllerProfile.controllers)
            {
                if(SettingsInstance.GetIsEnabled(controller.GetName()))
                {
                    Controllers.Add(controller);
                }
            }
            s_masterController = "";
            if(ConfigFormInstance != null && !ConfigFormInstance.IsDisposed && ControllerProfile.controllers.Count != s_preControllerCount)
            {
                ConfigFormInstance.updateControllers();
            }
            if (Controllers.Count == 0)
            {
                if (SettingsInstance.AlertNoControllerFound && s_preControllerCount != ControllerProfile.controllers.Count)
                {
                    s_preControllerCount = ControllerProfile.controllers.Count;
                    MessageBox.Show("有効化されたコントローラーを検出できませんでした。", "NumerousControllerInterface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                List<string> masterControllerList = new List<string>();
                foreach(NCIController controller in Controllers)
                {
                    if (SettingsInstance.GetProfile(controller).IsMasterController)
                    {
                        masterControllerList.Add(controller.GetName());
                    }
                }
                if(masterControllerList.Count >= 2)
                {
                    using(SelectMasterControllerForm form = new SelectMasterControllerForm(masterControllerList, controller =>
                    {
                        s_masterController = controller;
                    }))
                    {
                        form.ShowDialog();
                    }
                }else if(masterControllerList.Count == 1)
                {
                    s_masterController = masterControllerList[0];
                }
            }
            s_preControllerCount = ControllerProfile.controllers.Count;
        }

        public void Load(string settingsPath)
        {
            Input = new DirectInput();
            SettingsInstance = Settings.LoadFromXml(settingsPath);

            GetAllControllers();
        }

        public void SetAxisRanges(int[][] ranges)
        {
            _onePowerMax = ranges[3][1] + 1;
            _oneBreakMax = -ranges[3][0] + 1;
            _twoPowerMax = ranges[2][1] + 1;
            _twoBreakMax = -ranges[2][0] + 1;
        }

        public int GetPowerMax()
        {
            if(_onePowerMax > 1)
            {
                return _onePowerMax;
            }
            return _twoPowerMax;
        }

        public int GetBreakMax()
        {
            if (_oneBreakMax > 1)
            {
                return _oneBreakMax;
            }
            return _twoBreakMax;
        }

        public bool IsTwoHandle()
        {
            if (_oneBreakMax > 1)
            {
                return false;
            }
            return true;
        }

        public void Tick()
        {
            _isUpdateController = true;
        }

        public void MainTick()
        {
            if(!_isUpdateController)
            {
                System.Threading.Thread.Sleep(16);
                return;
            }
            _isUpdateController = false;
            if (ConfigFormInstance != null && !ConfigFormInstance.IsDisposed)
            {
                return;
            }
            if (Controllers.Count == 0)
            {
                return;
            }
            foreach(NCIController controller in Controllers)
            {
                ControllerProfile profile = SettingsInstance.Profiles[SettingsInstance.ProfileMap[controller.GetName()]];
                if(profile.IsMasterController && controller.GetName().Equals(s_masterController))
                {
                    int powerLevel = profile.GetPower(controller, GetPowerMax());
                    int breakLevel = profile.GetBreak(controller, GetBreakMax());
                    int prePower;
                    if (!_prePowerLevel.ContainsKey(controller))
                    {
                        _prePowerLevel.Add(controller, -1);
                        prePower = -1;
                    }
                    else
                    {
                        prePower = _prePowerLevel[controller];
                    }
                    int preBreak;
                    if (!_preBreakLevel.ContainsKey(controller))
                    {
                        _preBreakLevel.Add(controller, -1);
                        preBreak = -1;
                    }
                    else
                    {
                        preBreak = _preBreakLevel[controller];
                    }
                    bool two = IsTwoHandle();
                    if(prePower != powerLevel)
                    {
                        if (two)
                        {
                            onLeverMoved(1, powerLevel);
                        }
                        else
                        {
                            onLeverMoved(3, powerLevel);
                        }
                    }
                    if (preBreak != breakLevel)
                    {
                        if (two)
                        {
                            onLeverMoved(2, breakLevel);
                        }
                        else
                        {
                            onLeverMoved(3, -breakLevel);
                        }
                    }
                    _prePowerLevel[controller] = powerLevel;
                    _preBreakLevel[controller] = breakLevel;
                }
                List<int> buttons = profile.GetButtons(controller);
                if(!_preButtons.ContainsKey(controller))
                {
                    _preButtons.Add(controller, new List<int>());
                }
                List<int> preButton = _preButtons[controller];
                foreach(int i in buttons)
                {
                    if(!preButton.Contains(i))
                    {
                        if (profile.KeyMap.ContainsKey(i))
                        {
                            ButtonFeature key = profile.KeyMap[i];
                            onKeyDown(key.Axis, key.Value);
                        }
                    }
                }
                foreach (int i in preButton)
                {
                    if (!buttons.Contains(i))
                    {
                        if (profile.KeyMap.ContainsKey(i))
                        {
                            ButtonFeature key = profile.KeyMap[i];
                            onKeyUp(key.Axis, key.Value);
                        }
                    }
                }
                _preButtons[controller] = buttons;
            }
        }

        public void Configure(IWin32Window owner)
        {
            using (ConfigFormInstance= new ConfigForm())
            {
                ConfigFormInstance.ShowDialog(owner);
            }
        }

        public void Dispose()
        {
            ControllerProfile.DisposeAllControllers();
            if (SettingsInstance != null)
            {
                SettingsInstance.SaveToXml();
                SettingsInstance = null;
            }
            TimerController.Stop();
            _isDisposeRequested = true;
        }

        private void onLeverMoved(int axis, int notch)
        {
            if (LeverMoved != null)
            {
                if(axis == 3)
                {
                    if(notch < 0)
                    {
                        _breakNotch = -notch;
                        _powerNotch = 0;
                    }else
                    {
                        _breakNotch = 0;
                        _powerNotch = notch;
                    }
                }
                LeverMoved(this, new InputEventArgs(axis, notch));
            }
        }

        private void onKeyDown(int axis, int keyCode)
        {
            if (LeverMoved != null)
            {
                if (axis == 99)
                {
                    switch(keyCode)
                    {
                        case 0:// 非常にする
                            _breakNotch = GetBreakMax() - 1;
                            _powerNotch = 0;
                            break;
                        case 1:// 全て切にする
                            _breakNotch = 0;
                            _powerNotch = 0;
                            break;
                        case 2:// ブレーキ切
                            _breakNotch = 0;
                            break;
                        case 3:// ブレーキ上げ
                            _breakNotch++;
                            break;
                        case 4:// ブレーキ下げ
                            _breakNotch--;
                            break;
                        case 5:// 力行切
                            _powerNotch = 0;
                            break;
                        case 6:// 力行上げ
                            _powerNotch++;
                            break;
                        case 7:// 力行下げ
                            _powerNotch--;
                            break;
                        case 8:// ノッチ上げ
                            if (_breakNotch > 0)
                            {
                                _breakNotch--;
                                _powerNotch = 0;
                            }
                            else
                            {
                                _breakNotch = 0;
                                _powerNotch++;
                            }
                            break;
                        case 9://ノッチ下げ
                            if (_powerNotch > 0)
                            {
                                _breakNotch = 0;
                                _powerNotch--;
                            }
                            else
                            {
                                _breakNotch++;
                                _powerNotch = 0;
                            }
                            break;
                    }
                    if(_breakNotch < 0)
                    {
                        _breakNotch = 0;
                    }
                    if(_breakNotch >= GetBreakMax())
                    {
                        _breakNotch = GetBreakMax() - 1;
                    }
                    if (_powerNotch < 0)
                    {
                        _powerNotch = 0;
                    }
                    if (_powerNotch>= GetPowerMax())
                    {
                        _powerNotch = GetPowerMax() - 1;
                    }
                    bool two = IsTwoHandle();
                    if (_breakNotch == 0 && _powerNotch == 0)
                    {
                        if (two)
                        {
                            onLeverMoved(1, 0);
                            onLeverMoved(2, 0);
                        }
                        else
                        {
                            onLeverMoved(3, 0);
                        }
                    }else if(_breakNotch > 0)
                    {
                        if (two)
                        {
                            onLeverMoved(2, _breakNotch);
                        }
                        else
                        {
                            onLeverMoved(3, -_breakNotch);
                        }
                    }
                    else
                    {
                        if (two)
                        {
                            onLeverMoved(1, _powerNotch);
                        }
                        else
                        {
                            onLeverMoved(3, _powerNotch);
                        }
                    }
                }
                else
                {
                    if(axis < 0)
                    {
                        KeyDown(this, new InputEventArgs(axis, keyCode));
                    }
                    else
                    {
                        LeverMoved(this, new InputEventArgs(axis, keyCode));
                    }
                }
            }
        }

        private void onKeyUp(int axis, int keyCode)
        {
            if (LeverMoved != null)
            {
                if (axis == 99)
                {

                }
                else
                {
                    KeyUp(this, new InputEventArgs(axis, keyCode));
                }
            }
        }
    }
}
