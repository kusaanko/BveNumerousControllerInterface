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

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ControllerSetupForm : Form
    {
        private IController stick;
        private int step;
        private bool setupPower;
        private bool[] initButtonState;
        private int[] initSliders;
        private bool[] buttons;
        private int[] sliders;
        private List<int> useButtons;
        private List<int> useSliders;
        private int notchPos;
        private ControllerProfile profile;
        private int mode;
        private int useAxis;
        private int axisMin;
        private int axisMax;
        private int[] PowerButtons = new int[0];
        private bool[,] PowerButtonStatus = new bool[0, 0];
        private int[] PowerAxises = new int[0];
        private int[,] PowerAxisStatus = new int[0,0];
        private int[] BreakButtons = new int[0];
        private bool[,] BreakButtonStatus = new bool[0, 0];
        private int[] BreakAxises = new int[0];
        private int[,] BreakAxisStatus = new int[0, 0];
        public ControllerSetupForm(IController controller, ControllerProfile profile, bool setupPower)
        {
            stick = controller;
            InitializeComponent();
            stick.Read();
            initButtonState = stick.GetButtons();
            initSliders = profile.GetSliders(stick);
            timer1.Start();
            this.setupPower = setupPower;
            this.profile = profile;
            useButtons = new List<int>();
            useSliders = new List<int>();
            countLabel.Text = "";
            step = -1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            buttons = stick.GetButtons();
            sliders = profile.GetSliders(stick);
            if(mode == 0)
            {
                if(step == 0)
                {
                    for(int i = 0;i < sliders.Length;i++)
                    {
                        if(Math.Abs(sliders[i] - initSliders[i]) > 400)
                        {
                            useAxis = i;
                            step++;
                            infoLabel.Text = (setupPower ? "力行" : "ブレーキ") + "を切にして次へを押してください。";
                            break;
                        }
                    }
                }else if (step == 1)
                {
                    axisMin = sliders[useAxis];
                    countLabel.Text = "現在の値:" + sliders[useAxis];
                }else if(step == 2)
                {
                    axisMax = sliders[useAxis];
                    countLabel.Text = "現在の値:" + sliders[useAxis];
                }
            }
            else if (mode == 1)
            {
                if (step == 0)
                {
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (initButtonState[i] != buttons[i] && !useButtons.Contains(i))
                        {
                            useButtons.Add(i);
                            useButtons.Sort();
                        }
                    }
                    for (int i = 0; i < sliders.Length; i++)
                    {
                        if (initSliders[i] != sliders[i] && !useSliders.Contains(i))
                        {
                            useSliders.Add(i);
                            useSliders.Sort();
                        }
                    }
                }else if(step == 2)
                {
                    countLabel.Text = "現在のノッチ:" + (setupPower ? profile.GetPower(stick, 99) : profile.GetBreak(stick, 99));
                    InaccuracyModeCheckBox.Visible = true;
                }
                else if (step == 3)
                {
                    Close();
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if(mode == 0)
            {
                if(step == 1)
                {
                    step++;
                    infoLabel.Text = (setupPower ? "力行" : "ブレーキ") + "を最大にして次へを押してください。";
                }else if(step == 2)
                {
                    step++;
                    if(setupPower)
                    {
                        profile.PowerButtons = new int[0];
                        profile.PowerButtonStatus = new bool[0, 0];
                        profile.PowerAxises = new int[] { useAxis };
                        profile.PowerAxisStatus = new int[,] { { axisMin, axisMax } };
                        profile.InaccuracyModePower = false;
                    }else
                    {
                        profile.BreakButtons = new int[0];
                        profile.BreakButtonStatus = new bool[0, 0];
                        profile.BreakAxises = new int[] { useAxis };
                        profile.BreakAxisStatus = new int[,] { { axisMin, axisMax } };
                        profile.InaccuracyModeBreak = false;
                    }
                    profile.CalcDuplicated();
                    countLabel.Text = "";
                    infoLabel.Text = "完了しました。次へを押して終了します。";
                }else if(step == 3)
                {
                    Close();
                }
            }
            else if (mode == 1)
            {
                if (step == 0)
                {
                    if (setupPower)
                    {
                        PowerButtons = useButtons.ToArray();
                        PowerButtonStatus = new bool[1, PowerButtons.Length];
                        PowerAxises = useSliders.ToArray();
                        PowerAxisStatus = new int[1, PowerAxises.Length];
                    }
                    else
                    {
                        BreakButtons = useButtons.ToArray();
                        BreakButtonStatus = new bool[1, BreakButtons.Length];
                        BreakAxises = useSliders.ToArray();
                        BreakAxisStatus = new int[1, BreakAxises.Length];
                    }
                    step++;
                    infoLabel.Text = (setupPower ? "力行" : "ブレーキ") + "を切から順番に入れ、次へをクリックして下さい。\n終了時は2回次へをクリックして下さい。";
                    countLabel.Text = "登録するノッチ:0";
                }
                else if (step == 1)
                {
                    if (setupPower)
                    {
                        int k = PowerButtonStatus.GetLength(0) - 1;
                        int match = 0;
                        for (int i = 0; i < PowerButtons.Length; i++)
                        {
                            if (PowerButtonStatus[k, i] == buttons[PowerButtons[i]])
                            {
                                match++;
                            }
                        }
                        for (int i = 0; i < PowerAxises.Length; i++)
                        {
                            if (PowerAxisStatus[k, i] == sliders[PowerAxises[i]])
                            {
                                match++;
                            }
                        }
                        if (match == PowerButtons.Length + PowerAxises.Length)
                        {
                            //終了
                            if (k == notchPos - 1)
                            {
                                step++;
                                infoLabel.Text = "完了しました。次へを押して終了";
                                profile.PowerButtons = PowerButtons;
                                profile.PowerButtonStatus = PowerButtonStatus;
                                profile.PowerAxises = PowerAxises;
                                profile.PowerAxisStatus = PowerAxisStatus;
                                profile.InaccuracyModePower = false;
                                profile.CalcDuplicated();
                                return;
                            }
                        }
                        PowerButtonStatus = ResizeBoolTwo(PowerButtonStatus, notchPos + 1, PowerButtons.Length);
                        PowerAxisStatus = ResizeIntTwo(PowerAxisStatus, notchPos + 1, PowerAxises.Length);
                        for (int i = 0; i < PowerButtons.Length; i++)
                        {
                            PowerButtonStatus[notchPos, i] =
                                buttons[PowerButtons[i]];
                        }
                        for (int i = 0; i < PowerAxises.Length; i++)
                        {
                            PowerAxisStatus[notchPos, i] = sliders[PowerAxises[i]];
                        }
                        profile.CalcDuplicated();
                    }
                    else
                    {
                        int k = BreakButtonStatus.GetLength(0) - 1;
                        int match = 0;
                        for (int i = 0; i < BreakButtons.Length; i++)
                        {
                            if (BreakButtonStatus[k, i] == buttons[BreakButtons[i]])
                            {
                                match++;
                            }
                        }
                        for (int i = 0; i < BreakAxises.Length; i++)
                        {
                            if (BreakAxisStatus[k, i] == sliders[BreakAxises[i]])
                            {
                                match++;
                            }
                        }
                        if (match == BreakButtons.Length + BreakAxises.Length)
                        {
                            //終了
                            if (k == notchPos - 1)
                            {
                                step++;
                                infoLabel.Text = "完了しました。次へを押して終了";
                                profile.BreakButtons = BreakButtons;
                                profile.BreakButtonStatus = BreakButtonStatus;
                                profile.BreakAxises = BreakAxises;
                                profile.BreakAxisStatus = BreakAxisStatus;
                                profile.InaccuracyModeBreak = false;
                                profile.CalcDuplicated();
                                return;
                            }
                        }
                        BreakButtonStatus = ResizeBoolTwo(BreakButtonStatus, notchPos + 1, BreakButtons.Length);
                        BreakAxisStatus = ResizeIntTwo(BreakAxisStatus, notchPos + 1, BreakAxises.Length);
                        for (int i = 0; i < BreakButtons.Length; i++)
                        {
                            BreakButtonStatus[notchPos, i] =
                                buttons[BreakButtons[i]];
                        }
                        for (int i = 0; i < BreakAxises.Length; i++)
                        {
                            BreakAxisStatus[notchPos, i] = sliders[BreakAxises[i]];
                        }
                    }
                    notchPos++;
                    countLabel.Text = "登録するノッチ:" + notchPos + "";
                }
                else if (step == 2)
                {
                    step++;
                }
            }
        }

        private static bool[,] ResizeBoolTwo(bool[,] src, int size1, int size2)
        {
            bool[,] tmp = new bool[size1,size2];
            Array.ConstrainedCopy(src, 0, tmp, 0, src.Length);
            return tmp;
        }

        private static int[,] ResizeIntTwo(int[,] src, int size1, int size2)
        {
            int[,] tmp = new int[size1, size2];
            Array.ConstrainedCopy(src, 0, tmp, 0, src.Length);
            return tmp;
        }

        private void cacelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void useAxisButton_Click(object sender, EventArgs e)
        {
            mode = 0;
            step = 0;
            useAxisButton.Hide();
            useButtonAxisButton.Hide();
            infoLabel.Text = "使用する軸を動かして下さい。";
        }

        private void useButtonAxisButton_Click(object sender, EventArgs e)
        {
            mode = 1;
            step = 0;
            useAxisButton.Hide();
            useButtonAxisButton.Hide();
            infoLabel.Text = "一度すべての" + (setupPower ? "力行" : "ブレーキ") + "に入れてください。";
        }

        private void InaccuracyModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(setupPower)
            {
                this.profile.InaccuracyModeBreak = InaccuracyModeCheckBox.Checked;
            }else
            {
                this.profile.InaccuracyModeBreak = InaccuracyModeCheckBox.Checked;
            }
        }
    }
}
