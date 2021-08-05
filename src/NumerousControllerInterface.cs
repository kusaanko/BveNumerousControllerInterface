using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Diagnostics;
using Mackoy.Bvets;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class NumerousControllerInterface : IInputDevice
    {
        public static DirectInput input;
        public static List<IController> controllers;

        public static Settings settings = null;
        public static ConfigForm configForm;

        public event InputEventHandler KeyDown;
        public event InputEventHandler KeyUp;
        public event InputEventHandler LeverMoved;

        private Dictionary<IController, int> prePowerLevel;
        private Dictionary<IController, int> preBreakLevel;
        private Dictionary<IController, List<int>> preButtons;

        private int onePowerMax;
        private int oneBreakMax;
        private int twoPowerMax;
        private int twoBreakMax;
        private int powerNotch;
        private int breakNotch;

        private static int preControllerCount;

        private static string masterController;

        public static Timer timer;
        private bool updateController;
        private bool isDisposeRequested;

        public NumerousControllerInterface()
        {
            controllers = new List<IController>();
            prePowerLevel = new Dictionary<IController, int>();
            preBreakLevel = new Dictionary<IController, int>();
            preButtons = new Dictionary<IController, List<int>>();
            masterController = "";
            preControllerCount = -1;
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += new System.EventHandler(TimerTick);
            }
            timer.Start();
            System.Threading.Thread mainThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                while (!isDisposeRequested)
                {
                    MainTick();
                }
            }));
            mainThread.Start();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            if (configForm != null && !configForm.IsDisposed && (configForm.controllerSetupForm == null || configForm.controllerSetupForm.IsDisposed)) 
            {
                GetAllControllers();
            }
        }

        public static void GetAllControllers()
        {
            controllers.Clear();
            ControllerProfile.GetAllControllers();
            foreach(IController controller in ControllerProfile.controllers)
            {
                if(settings.GetIsEnabled(controller.GetName()))
                {
                    controllers.Add(controller);
                }
            }
            masterController = "";
            if(configForm != null && !configForm.IsDisposed && ControllerProfile.controllers.Count != preControllerCount)
            {
                configForm.updateControllers();
            }
            if (controllers.Count == 0)
            {
                if (settings.AlertNoControllerFound && preControllerCount != ControllerProfile.controllers.Count)
                {
                    preControllerCount = ControllerProfile.controllers.Count;
                    MessageBox.Show("有効化されたコントローラーを検出できませんでした。", "NumerousControllerInterface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                List<string> masterControllerList = new List<string>();
                foreach(IController controller in controllers)
                {
                    if (settings.GetProfile(controller).IsMasterController)
                    {
                        masterControllerList.Add(controller.GetName());
                    }
                }
                if(masterControllerList.Count >= 2)
                {
                    using(SelectMasterControllerForm form = new SelectMasterControllerForm(masterControllerList, controller =>
                    {
                        masterController = controller;
                    }))
                    {
                        form.ShowDialog();
                    }
                }else if(masterControllerList.Count == 1)
                {
                    masterController = masterControllerList[0];
                }
            }
            preControllerCount = ControllerProfile.controllers.Count;
        }

        public void Load(string settingsPath)
        {
            input = new DirectInput();
            settings = Settings.LoadFromXml(settingsPath);

            GetAllControllers();
        }

        public void SetAxisRanges(int[][] ranges)
        {
            onePowerMax = ranges[3][1] + 1;
            oneBreakMax = -ranges[3][0] + 1;
            twoPowerMax = ranges[2][1] + 1;
            twoBreakMax = -ranges[2][0] + 1;
        }

        public int GetPowerMax()
        {
            if(onePowerMax > 1)
            {
                return onePowerMax;
            }
            return twoPowerMax;
        }

        public int GetBreakMax()
        {
            if (oneBreakMax > 1)
            {
                return oneBreakMax;
            }
            return twoBreakMax;
        }

        public bool IsTwoHandle()
        {
            if (oneBreakMax > 1)
            {
                return false;
            }
            return true;
        }

        public void Tick()
        {
            updateController = true;
        }

        public void MainTick()
        {
            if(!updateController)
            {
                System.Threading.Thread.Sleep(16);
                return;
            }
            updateController = false;
            if (configForm != null && !configForm.IsDisposed)
            {
                return;
            }
            if (controllers.Count == 0)
            {
                return;
            }
            foreach(IController controller in controllers)
            {
                ControllerProfile profile = settings.Profiles[settings.ProfileMap[controller.GetName()]];
                controller.Read();
                if(profile.IsMasterController && controller.GetName().Equals(masterController))
                {
                    int powerLevel = profile.GetPower(controller, GetPowerMax());
                    int breakLevel = profile.GetBreak(controller, GetBreakMax());
                    int prePower;
                    if (!prePowerLevel.ContainsKey(controller))
                    {
                        prePowerLevel.Add(controller, -1);
                        prePower = -1;
                    }
                    else
                    {
                        prePower = prePowerLevel[controller];
                    }
                    int preBreak;
                    if (!preBreakLevel.ContainsKey(controller))
                    {
                        preBreakLevel.Add(controller, -1);
                        preBreak = -1;
                    }
                    else
                    {
                        preBreak = preBreakLevel[controller];
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
                    prePowerLevel[controller] = powerLevel;
                    preBreakLevel[controller] = breakLevel;
                }
                List<int> buttons = profile.GetButtons(controller);
                if(!preButtons.ContainsKey(controller))
                {
                    preButtons.Add(controller, new List<int>());
                }
                List<int> preButton = preButtons[controller];
                foreach(int i in buttons)
                {
                    if(!preButton.Contains(i))
                    {
                        if (profile.KeyMap.ContainsKey(i))
                        {
                            int[] key = profile.KeyMap[i];
                            onKeyDown(key[0], key[1]);
                        }
                    }
                }
                foreach (int i in preButton)
                {
                    if (!buttons.Contains(i))
                    {
                        if (profile.KeyMap.ContainsKey(i))
                        {
                            int[] key = profile.KeyMap[i];
                            onKeyUp(key[0], key[1]);
                        }
                    }
                }
                preButtons[controller] = buttons;
            }
        }

        public void Configure(IWin32Window owner)
        {
            using (configForm= new ConfigForm())
            {
                configForm.ShowDialog(owner);
            }
        }

        public void Dispose()
        {
            ControllerProfile.DisposeAllControllers();
            if (settings != null)
            {
                settings.SaveToXml();
                settings = null;
            }
            timer.Stop();
            isDisposeRequested = true;
        }

        private void onLeverMoved(int axis, int notch)
        {
            if (LeverMoved != null)
            {
                if(axis == 3)
                {
                    if(notch < 0)
                    {
                        breakNotch = -notch;
                        powerNotch = 0;
                    }else
                    {
                        breakNotch = 0;
                        powerNotch = notch;
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
                        case 0://非常にする
                            breakNotch = GetBreakMax() - 1;
                            powerNotch = 0;
                            break;
                        case 1://全て切にする
                            breakNotch = 0;
                            powerNotch = 0;
                            break;
                        case 2://ブレーキ切
                            breakNotch = 0;
                            break;
                        case 3://ブレーキ上げ
                            breakNotch++;
                            break;
                        case 4://ブレーキ下げ
                            breakNotch--;
                            break;
                        case 5://力行切
                            powerNotch = 0;
                            break;
                        case 6://力行上げ
                            powerNotch++;
                            break;
                        case 7://力行下げ
                            powerNotch--;
                            break;
                        case 8://ノッチ上げ
                            if (breakNotch > 0)
                            {
                                breakNotch--;
                                powerNotch = 0;
                            }
                            else
                            {
                                breakNotch = 0;
                                powerNotch++;
                            }
                            break;
                        case 9://ノッチ下げ
                            if (powerNotch > 0)
                            {
                                breakNotch = 0;
                                powerNotch--;
                            }
                            else
                            {
                                breakNotch++;
                                powerNotch = 0;
                            }
                            break;
                    }
                    if(breakNotch < 0)
                    {
                        breakNotch = 0;
                    }
                    if(breakNotch >= GetBreakMax())
                    {
                        breakNotch = GetBreakMax() - 1;
                    }
                    if (powerNotch < 0)
                    {
                        powerNotch = 0;
                    }
                    if (powerNotch>= GetPowerMax())
                    {
                        powerNotch = GetPowerMax() - 1;
                    }
                    bool two = IsTwoHandle();
                    if (breakNotch == 0 && powerNotch == 0)
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
                    }else if(breakNotch > 0)
                    {
                        if (two)
                        {
                            onLeverMoved(2, breakNotch);
                        }
                        else
                        {
                            onLeverMoved(3, -breakNotch);
                        }
                    }
                    else
                    {
                        if (two)
                        {
                            onLeverMoved(1, powerNotch);
                        }
                        else
                        {
                            onLeverMoved(3, powerNotch);
                        }
                    }
                }
                else
                {
                    KeyDown(this, new InputEventArgs(axis, keyCode));
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
