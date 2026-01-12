using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;
using Mackoy.Bvets;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System.Linq;
using System.Net;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using static Kusaanko.Bvets.NumerousControllerInterface.Controller.NCIController;
using System.Threading;
using SlimDX.DirectInput;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class NumerousControllerInterface : IInputDevice
    {
        private static bool DebugUpdater = false;
        public static int IntVersion { get { return 15; } }
        public static string UserAgent = "NumerousContollerInterfaceUpdater v" + IntVersion;

        public static List<NCIController> Controllers;

        public static Settings SettingsInstance = null;
        public static ConfigForm ConfigFormInstance;
        private static SelectMasterControllerForm s_selectMasterControllerForm;

        public event InputEventHandler KeyDown;
        public event InputEventHandler KeyUp;
        public event InputEventHandler LeverMoved;

        private Dictionary<NCIController, int> _prePowerLevel;
        private Dictionary<NCIController, int> _preBrakeLevel;
        private Dictionary<NCIController, List<int>> _preButtons;
        // ボタンのID、押した時間
        private Dictionary<NCIController, Dictionary<int, DateTime>> _lastButtonPressedTime;
        // ボタンのID、連打中かどうか
        private Dictionary<NCIController, Dictionary<int, bool>> _isButtonRepeating;
        private Dictionary<NCIController, Reverser> _preReverser;

        public static List<NumerousControllerPlugin> Plugins;

        private static int s_onePowerMax;
        private static int s_oneBrakeMax;
        private static int s_twoPowerMax;
        private static int s_twoBrakeMax;
        private static int s_twoPowerMin;
        private static int s_powerNotch;
        private static int s_brakeNotch;

        private static int s_preControllerCount;
        private static int s_preEnabledMasterControllerCount;

        private static string s_powerController;
        private static string s_brakeController;
        private static string s_reverserController;

        public static bool IsMasterControllerUpdateRequested;

        public static System.Windows.Forms.Timer TimerController;
        private bool _isUpdateController;
        private bool _isDisposeRequested;
        private static bool s_isRunningGetAllControllers;
        public static Version BveExPluginVersion;
        // string, Type, string
        public static List<object[]> BveExPluginAvailableValues;
        private static Thread _ControllerOutputThread;
        private static bool _ControllerOutputThreadAlive;

        public NumerousControllerInterface()
        {
            // SlimDXのバージョンを無視する
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            ButtonFeature.Initialize();
            Controllers = new List<NCIController>();
            _prePowerLevel = new Dictionary<NCIController, int>();
            _preBrakeLevel = new Dictionary<NCIController, int>();
            _preButtons = new Dictionary<NCIController, List<int>>();
            _preReverser = new Dictionary<NCIController, Reverser>();
            _lastButtonPressedTime = new Dictionary<NCIController, Dictionary<int, DateTime>>();
            _isButtonRepeating = new Dictionary<NCIController, Dictionary<int, bool>>();
            Plugins = new List<NumerousControllerPlugin>();
            s_powerController = "";
            s_brakeController = "";
            s_reverserController = "";
            s_preControllerCount = -1;
            if (TimerController == null)
            {
                TimerController = new System.Windows.Forms.Timer();
                TimerController.Interval = 2000;
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

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs e) {
            if (e.Name.StartsWith("SlimDX"))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name.StartsWith("SlimDX"))
                    {
                        return assembly;
                    }
                }
            }
            return null;
        }

        public static bool CheckUpdates(bool forceUpdate)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            string targetArch = BinaryInfo.arch;
            string net_ver = BinaryInfo.net_ver;
            string update_url = "https://raw.githubusercontent.com/kusaanko/BveNumerousControllerInterface/main/update_info.xml";
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add("User-Agent", UserAgent);
                    client.Encoding = System.Text.Encoding.UTF8;

                    string content = client.DownloadString(update_url);
                    XElement xml = XDocument.Parse(content).Root;
                    string history = "";
                    foreach (XElement release in xml.Elements("ReleaseNotes").Elements("Release"))
                    {
                        history += release.Element("Title").Value.Replace("\\n", "\n") + "\n";
                        history += release.Element("Context").Value.Replace("\\n", "\n") + "\n\n";
                    }
                    foreach (XElement target in xml.Element("Targets").Elements("Target"))
                    {
                        string arch = target.Attribute("arch").Value;
                        string dotnet = target.Attribute("dotnet").Value;
                        if (arch == targetArch && dotnet == net_ver)
                        {
                            string latestTag = target.Element("Latest").Value;
                            int intVersion = int.Parse(target.Element("IntVersion").Value);
                            if ((intVersion > IntVersion && (SettingsInstance.IgnoreUpdate < intVersion || forceUpdate)) || DebugUpdater)
                            {
                                string tempDir = Path.GetTempPath();
                                string downloadTmpDir = Path.Combine(tempDir, "NumerousControllerInterface");
                                try
                                {
                                    if (Directory.Exists(downloadTmpDir))
                                    {
                                        DirectoryInfo info = new DirectoryInfo(downloadTmpDir);
                                        info.Delete(true);
                                    }
                                }
                                catch (Exception) { }
                                string downloadFilePath = Path.Combine(downloadTmpDir, target.Element("Asset").Value);
                                string installer = "";
                                string downloadPage = null;
                                string downloadUrl = null;

                                if (target.Element("Installer") != null)
                                {
                                    installer = target.Element("Installer").Value;
                                }
                                if (target.Element("DownloadPage") != null)
                                {
                                    downloadPage = target.Element("DownloadPage").Value;
                                }
                                if (target.Element("DownloadUrl") != null)
                                {
                                    downloadUrl = target.Element("DownloadUrl").Value;
                                }
                                using (UpdateForm form = new UpdateForm(
                                    target.Element("Version").Value,
                                    history,
                                    downloadPage,
                                    downloadUrl,
                                    downloadFilePath,
                                    installer,
                                    intVersion
                                    ))
                                {
                                    form.ShowDialog();
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
            return false;
        }

        // コントローラー更新用のタイマー
        private static void TimerTick(object sender, EventArgs e)
        {
            if ((s_selectMasterControllerForm == null || s_selectMasterControllerForm.IsDisposed) && !s_isRunningGetAllControllers) 
            {
                GetAllControllers();
            }
        }

        public static void GetAllControllers()
        {
            if (SettingsInstance == null) return;
            s_isRunningGetAllControllers = true;
            Controllers.Clear();
            ControllerProfile.GetAllControllers();
            foreach(NCIController controller in ControllerProfile.controllers)
            {
                if(SettingsInstance.GetIsEnabled(controller.GetName()))
                {
                    Controllers.Add(controller);
                }
            }
            bool isPowerControllerExists = false;
            bool isBrakeControllerExists = false;
            bool isReverserControllerExists = false;
            foreach (NCIController controller in Controllers)
            {
                ControllerProfile profile = SettingsInstance.GetProfile(controller);
                if (profile == null) continue;
                if (s_powerController.Equals(controller.GetName()) && profile.HasPower(controller))
                {
                    isPowerControllerExists = true;
                }
                if (s_brakeController.Equals(controller.GetName()) && profile.HasBrake(controller))
                {
                    isBrakeControllerExists = true;
                }
                if (s_reverserController.Equals(controller.GetName()) && profile.HasReverser(controller))
                {
                    isReverserControllerExists = true;
                }
            }
            if(!isPowerControllerExists)
            {
                s_powerController = "";
            }
            if (!isBrakeControllerExists)
            {
                s_brakeController = "";
            }
            if (!isReverserControllerExists)
            {
                s_reverserController = "";
                isReverserControllerExists = false;
                foreach (NCIController controller in Controllers)
                {
                    ControllerProfile profile = SettingsInstance.GetProfile(controller);
                    if (profile == null) continue;
                    if (profile.HasReverser(controller))
                    {
                        s_reverserController = controller.GetName();
                        isReverserControllerExists = true;
                    }
                }
            }
            if (ConfigFormInstance != null && !ConfigFormInstance.IsDisposed && ControllerProfile.controllers.Count != s_preControllerCount)
            {
                ConfigFormInstance.UpdateControllers();
            }
            if (Controllers.Count == 0)
            {
                if (SettingsInstance.AlertNoControllerFound && s_preControllerCount != ControllerProfile.controllers.Count)
                {
                    s_preControllerCount = ControllerProfile.controllers.Count;
                    MessageBox.Show("有効化されたコントローラーを検出できませんでした。\nコントローラーが正常に検出されている場合は設定から使用するコントローラーを有効にしてください。", "NumerousControllerInterface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (s_preEnabledMasterControllerCount != Controllers.Count || IsMasterControllerUpdateRequested)
                {
                    // マスコン、力行のみのコントローラー、制動のみのコントローラーの重複をチェック
                    IsMasterControllerUpdateRequested = false;
                    bool isMasterControllerOnly = true;
                    List<string> masterControllerList = new List<string>();
                    int[] masterList = new int[] { ButtonFeature.BringNotchUp.Value, ButtonFeature.BringNotchDown.Value };
                    foreach (NCIController controller in Controllers)
                    {
                        ControllerProfile profile = SettingsInstance.GetProfile(controller);
                        if (profile == null) continue;
                        bool hasMaster = false;
                        if (profile.HasPower(controller) && profile.HasBrake(controller))
                        {
                            hasMaster = true;
                        }
                        else
                        {
                            foreach (ButtonFeature feature in profile.KeyMap.Values)
                            {
                                if (masterList.Contains(feature.Value))
                                {
                                    hasMaster = true;
                                    break;
                                }
                            }
                        }
                        if (hasMaster)
                        {
                            masterControllerList.Add(controller.GetName());
                            if (!masterControllerList.Contains(controller.GetName()))
                            {
                                isMasterControllerOnly = false;
                            }
                        }
                    }
                    List<string> powerControllerList = new List<string>();
                    int[] powerList = new int[] { ButtonFeature.BringNotchUp.Value, ButtonFeature.BringNotchDown.Value, ButtonFeature.BringPowerUp.Value, ButtonFeature.BringPowerDown.Value };
                    foreach (NCIController controller in Controllers)
                    {
                        ControllerProfile profile = SettingsInstance.GetProfile(controller);
                        if (profile == null) continue;
                        bool hasPower = false;
                        if (profile.HasPower(controller))
                        {
                            hasPower = true;
                        }
                        else
                        {
                            foreach (ButtonFeature feature in profile.KeyMap.Values)
                            {
                                if (powerList.Contains(feature.Value))
                                {
                                    hasPower = true;
                                    break;
                                }
                            }
                        }
                        if (hasPower)
                        {
                            powerControllerList.Add(controller.GetName());
                            if (!masterControllerList.Contains(controller.GetName()))
                            {
                                isMasterControllerOnly = false;
                            }
                        }
                    }
                    List<string> brakeControllerList = new List<string>();
                    int[] brakeList = new int[] { ButtonFeature.BringNotchUp.Value, ButtonFeature.BringNotchDown.Value, ButtonFeature.BringBrakeUp.Value, ButtonFeature.BringBrakeDown.Value };
                    foreach (NCIController controller in Controllers)
                    {
                        ControllerProfile profile = SettingsInstance.GetProfile(controller);
                        if (profile == null) continue;
                        bool hasBrake = false;
                        if (profile.HasBrake(controller))
                        {
                            hasBrake = true;
                        }
                        else
                        {
                            foreach (ButtonFeature feature in profile.KeyMap.Values)
                            {
                                if (brakeList.Contains(feature.Value))
                                {
                                    hasBrake = true;
                                    break;
                                }
                            }
                        }
                        if (hasBrake)
                        {
                            brakeControllerList.Add(controller.GetName());
                            if (!masterControllerList.Contains(controller.GetName()))
                            {
                                isMasterControllerOnly = false;
                            }
                        }
                    }
                    if (isMasterControllerOnly)
                    {
                        if (masterControllerList.Count >= 2)
                        {
                            using (s_selectMasterControllerForm = new SelectMasterControllerForm(masterControllerList, "マスコン", controller =>
                            {
                                s_powerController = controller;
                                s_brakeController = controller;
                            }))
                            {
                                s_selectMasterControllerForm.ShowDialog();
                            }
                        }
                        else if (masterControllerList.Count == 1)
                        {
                            s_powerController = masterControllerList[0];
                            s_brakeController = masterControllerList[0];
                        }
                    }
                    else
                    {
                        if (powerControllerList.Count >= 2)
                        {
                            using (s_selectMasterControllerForm = new SelectMasterControllerForm(powerControllerList, "力行のみ持つコントローラー", controller =>
                            {
                                s_powerController = controller;
                            }))
                            {
                                s_selectMasterControllerForm.ShowDialog();
                            }
                        }
                        else if (powerControllerList.Count == 1)
                        {
                            s_powerController = powerControllerList[0];
                        }
                        if (brakeControllerList.Count >= 2)
                        {
                            using (s_selectMasterControllerForm = new SelectMasterControllerForm(brakeControllerList, "制動のみ持つコントローラー", controller =>
                            {
                                s_brakeController = controller;
                            }))
                            {
                                s_selectMasterControllerForm.ShowDialog();
                            }
                        }
                        else if (brakeControllerList.Count == 1)
                        {
                            s_brakeController = brakeControllerList[0];
                        }
                    }
                }
            }
            s_preControllerCount = ControllerProfile.controllers.Count;
            s_preEnabledMasterControllerCount = Controllers.Count;
            s_isRunningGetAllControllers = false;
        }

        public void Load(string settingsPath)
        {
            SettingsInstance = Settings.LoadFromXml(settingsPath);

            foreach (NumerousControllerPlugin plugin in Plugins)
            {
                plugin.Load();
            }

            GetAllControllers();
            // 更新の確認はバックグラウンドで行う

            bool checkUpdates = SettingsInstance.CheckUpdates;

            if(IsDebug())
            {
                checkUpdates = false || DebugUpdater;
            } else
            {
                DebugUpdater = false;
            }
            if (checkUpdates)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    int waitCount = 0;
                    // BVEのメインフォームが開くまで待機
                    while (Application.OpenForms.Count == 0)
                    {
                        Thread.Sleep(100);
                        waitCount++;
                        if (waitCount > 20 * 10)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(500);
                    CheckUpdates(false);
                })).Start();
            }
        }

        public static bool IsDebug()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attributes.Length == 1)
            {
                AssemblyConfigurationAttribute assemblyConfiguration = attributes[0] as AssemblyConfigurationAttribute;
                if (assemblyConfiguration != null)
                {
                    if (assemblyConfiguration.Configuration.Equals("Debug"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetAxisRanges(int[][] ranges)
        {
            // ranges
            // ranges[axis][0] = min
            // ranges[axis][1] = max

            // Axis3 = One Handle
            // notch > 0 ... Power
            // notch < 0 ... Brake
            s_onePowerMax = ranges[3][1];
            s_oneBrakeMax = -ranges[3][0];
            // Axis2 = Two Handle Brake
            s_twoBrakeMax = ranges[2][1];
            // Axis1 = Two Handle Power
            s_twoPowerMax = ranges[1][1];
            s_twoPowerMin = -ranges[1][0];

            // 力行と制動をリセット
            s_powerNotch = 0;
            s_brakeNotch = GetBrakeMax();

            Debug.WriteLine("One Power Max: " + s_onePowerMax);
            Debug.WriteLine("One Brake Max: " + s_oneBrakeMax);
            Debug.WriteLine("Two Power Max: " + s_twoPowerMax);
            Debug.WriteLine("Two Power Min: " + s_twoPowerMin);
            Debug.WriteLine("Two Brake Max: " + s_twoBrakeMax);
        }

        // 力行の最大を取得
        public static int GetPowerMax()
        {
            if(IsTwoHandle())
            {
                return s_twoPowerMax;
            }
            return s_onePowerMax;
        }

        // 力行の最小を取得
        public static int GetPowerMin()
        {
            if (IsTwoHandle())
            {
                return s_twoPowerMin;
            }
            return 0;
        }

        // 制動の最大を取得
        public static int GetBrakeMax()
        {
            if (IsTwoHandle())
            {
                return s_twoBrakeMax;
            }
            return s_oneBrakeMax;
        }

        // 運転中の車両がツーハンドルかどうか(Bveはワンハンドル、ツーハンドル関係なくaxis1～3に値が設定される）
        public static bool IsTwoHandle()
        {
            return s_twoPowerMax > 0;
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
                ControllerProfile profile = SettingsInstance.GetProfile(controller);
                if (profile == null) continue;
                // 力行
                if(controller.GetName().Equals(s_powerController))
                {
                    int powerLevel = profile.GetPower(controller, GetPowerMax());
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
                    bool two = IsTwoHandle();
                    if(prePower != powerLevel)
                    {
                        if (two)
                        {
                            OnLeverMoved(1, Math.Max(powerLevel, GetPowerMin()));
                        }
                        else
                        {
                            OnLeverMoved(3, powerLevel);
                        }
                    }
                    _prePowerLevel[controller] = powerLevel;
                }
                // 制動
                if (controller.GetName().Equals(s_brakeController))
                {
                    int brakeLevel = profile.GetBrake(controller, GetBrakeMax());
                    int preBrake;
                    if (!_preBrakeLevel.ContainsKey(controller))
                    {
                        _preBrakeLevel.Add(controller, -1);
                        preBrake = -1;
                    }
                    else
                    {
                        preBrake = _preBrakeLevel[controller];
                    }
                    bool two = IsTwoHandle();
                    if (preBrake != brakeLevel)
                    {
                        if (two)
                        {
                            OnLeverMoved(2, brakeLevel);
                        }
                        else
                        {
                            OnLeverMoved(3, -brakeLevel);
                        }
                    }
                    _preBrakeLevel[controller] = brakeLevel;
                }
                // リバーサー
                if (controller.GetName().Equals(s_reverserController))
                {
                    Reverser reverserPos = profile.GetReverser(controller);
                    Reverser preReverser;
                    if (!_preReverser.ContainsKey(controller))
                    {
                        _preReverser.Add(controller, Reverser.CENTER);
                        preReverser = Reverser.CENTER;
                    }
                    else
                    {
                        preReverser = _preReverser[controller];
                    }
                    if (preReverser != reverserPos)
                    {
                        int rev = 0;
                        if(reverserPos == Reverser.BACKWARD)
                        {
                            rev = -1;
                        }
                        if (reverserPos == Reverser.CENTER)
                        {
                            rev = 0;
                        }
                        if (reverserPos == Reverser.FORWARD)
                        {
                            rev = 1;
                        }
                        OnLeverMoved(0, rev);
                    }
                    _preReverser[controller] = reverserPos;
                }
                // ボタン
                ProcessControllerButtonInput(profile, controller);
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
            foreach (NumerousControllerPlugin plugin in Plugins)
            {
                plugin.Dispose();
            }
            TimerController.Stop();
            _isDisposeRequested = true;
        }

        private void OnLeverMoved(int axis, int notch)
        {
            if (LeverMoved != null)
            {
                if(axis == 3)
                {
                    if(notch < 0)
                    {
                        s_brakeNotch = -notch;
                        s_powerNotch = 0;
                    }else
                    {
                        s_brakeNotch = 0;
                        s_powerNotch = notch;
                    }
                }
                LeverMoved(this, new InputEventArgs(axis, notch));
            }
        }

        private void ProcessControllerButtonInput(ControllerProfile profile, NCIController controller)
        {
            List<int> buttons = profile.GetButtons(controller);
            if (!_preButtons.ContainsKey(controller))
            {
                _preButtons.Add(controller, new List<int>());
            }
            if (!_lastButtonPressedTime.ContainsKey(controller))
            {
                _lastButtonPressedTime.Add(controller, new Dictionary<int, DateTime>());
            }
            if (!_isButtonRepeating.ContainsKey(controller))
            {
                _isButtonRepeating.Add(controller, new Dictionary<int, bool>());
            }
            List<int> preButton = _preButtons[controller];
            foreach (int i in buttons)
            {
                var buttonRepeat = profile.HoldToRepeat.ContainsKey(i) ? profile.HoldToRepeat[i] : false;
                var buttonRepeatTime = profile.HoldToRepeatTime.ContainsKey(i) ? profile.HoldToRepeatTime[i] : 0.5;
                if (buttonRepeat)
                {
                    // 初回ボタン投下処理
                    if (!_lastButtonPressedTime[controller].ContainsKey(i))
                    {
                        ButtonFeature key = profile.KeyMap[i];
                        OnKeyDown(key.Axis, key.Value, false);
                        OnKeyUp(key.Axis, key.Value);
                        _isButtonRepeating[controller][i] = false;
                        // ボタンの押した時間を記録
                        _lastButtonPressedTime[controller][i] = DateTime.Now;
                    } else
                    {
                        // 連打中なら前回のボタン投下からの時間を計測
                        DateTime lastPressedTime = _lastButtonPressedTime[controller][i];
                        TimeSpan timeSinceLastPressed = DateTime.Now - lastPressedTime;

                        var targetTime = buttonRepeatTime;
                        var isButtonRepeating = _isButtonRepeating[controller].ContainsKey(i) ? _isButtonRepeating[controller][i] : false;

                        if (isButtonRepeating)
                        {
                            targetTime = 0.05;
                        }

                        if (!_isButtonRepeating[controller].ContainsKey(i))
                        {
                            _isButtonRepeating[controller][i] = false;
                        }
                        if (timeSinceLastPressed.TotalSeconds >= targetTime)
                        {
                            // 一定時間経過したら再度ボタンを押す
                            ButtonFeature key = profile.KeyMap[i];
                            OnKeyDown(key.Axis, key.Value, true);
                            OnKeyUp(key.Axis, key.Value);
                            _isButtonRepeating[controller][i] = true;
                            // ボタンの押した時間を記録
                            _lastButtonPressedTime[controller][i] = DateTime.Now;
                        }
                    }
                }
                else
                {
                    // リピートしないなら投下した瞬間だけボタンを押す
                    if (!preButton.Contains(i))
                    {
                        if (profile.KeyMap.ContainsKey(i))
                        {
                            ButtonFeature key = profile.KeyMap[i];
                            OnKeyDown(key.Axis, key.Value, false);
                        }
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
                        OnKeyUp(key.Axis, key.Value);
                    }
                    if (_isButtonRepeating[controller].ContainsKey(i))
                    {
                        _isButtonRepeating[controller].Remove(i);
                    }
                    if (_lastButtonPressedTime[controller].ContainsKey(i))
                    {
                        _lastButtonPressedTime[controller].Remove(i);
                    }
                }
            }
            _preButtons[controller] = buttons;
        }

        private void OnKeyDown(int axis, int keyCode, bool isRepeating)
        {
            if (LeverMoved != null && KeyDown != null)
            {
                if (axis == 100)
                {
                    // キーボードイベント
                    if ((keyCode >= 'A' && keyCode <= 'Z') || (keyCode >= '0' && keyCode <= '9'))
                    {
                        SendKeys.SendWait(Char.ToString((char)keyCode));
                    }
                    else if (keyCode >= 101 && keyCode <= 112)
                    {
                        // Function key
                        SendKeys.SendWait("{F" + (keyCode - 100) + "}");
                    }
                    else if (keyCode >= 150)
                    {
                        if (keyCode == 150) SendKeys.SendWait("{PGUP}");
                        if (keyCode == 151) SendKeys.SendWait("{PGDN}");
                        if (keyCode == 152) SendKeys.SendWait("{HOME}");
                        if (keyCode == 153) SendKeys.SendWait("{END}");
                        if (keyCode == 154) SendKeys.SendWait("{INSERT}");
                        if (keyCode == 155) SendKeys.SendWait("{DELETE}");
                        if (keyCode == 156) SendKeys.SendWait("{ESC}");
                        if (keyCode == 157) SendKeys.SendWait("{TAB}");
                        if (keyCode == 158) SendKeys.SendWait("{BACKSPACE}");
                        if (keyCode == 159) SendKeys.SendWait("{ENTER}");
                        if (keyCode == 160) SendKeys.SendWait(" ");
                        if (keyCode == 161) SendKeys.SendWait("{UP}");
                        if (keyCode == 162) SendKeys.SendWait("{DOWN}");
                        if (keyCode == 163) SendKeys.SendWait("{LEFT}");
                        if (keyCode == 164) SendKeys.SendWait("{RIGHT}");
                        if (keyCode == 165) SendKeys.SendWait("{ADD}");
                        if (keyCode == 166) SendKeys.SendWait("{SUBTRACT}");
                        if (keyCode == 167) SendKeys.SendWait("{MULTIPLY}");
                        if (keyCode == 168) SendKeys.SendWait("{DIVIDE}");
                    }
                }
                else if (axis == 99)
                {
                    switch(keyCode)
                    {
                        case 0:// 非常にする
                            s_brakeNotch = GetBrakeMax();
                            s_powerNotch = 0;
                            break;
                        case 1:// 全て切にする
                            s_brakeNotch = 0;
                            s_powerNotch = 0;
                            break;
                        case 2:// 制動切
                            s_brakeNotch = 0;
                            break;
                        case 3:// 制動上げ
                            // リピート入力時は非常の前で止まるように
                            s_brakeNotch = isRepeating
                                ? Math.Min(s_brakeNotch + 1, GetBrakeMax() - 1)
                                : s_brakeNotch + 1;
                            break;
                        case 4:// 制動下げ
                            s_brakeNotch--;
                            break;
                        case 5:// 力行切
                            s_powerNotch = 0;
                            break;
                        case 6:// 力行上げ
                            s_powerNotch++;
                            break;
                        case 7:// 力行下げ
                            s_powerNotch--;
                            break;
                        case 8:// ノッチ上げ
                            if (s_brakeNotch > 0)
                            {
                                s_brakeNotch--;
                                s_powerNotch = 0;
                            }
                            else
                            {
                                s_brakeNotch = 0;
                                s_powerNotch++;
                            }
                            break;
                        case 9://ノッチ下げ
                            if (s_powerNotch > 0)
                            {
                                s_brakeNotch = 0;
                                s_powerNotch--;
                            }
                            else
                            {
                                // リピート入力時は非常の前で止まるように
                                s_brakeNotch = isRepeating
                                    ? Math.Min(s_brakeNotch + 1, GetBrakeMax() - 1)
                                    : s_brakeNotch + 1;
                                s_powerNotch = 0;
                            }
                            break;
                    }
                    if(s_brakeNotch < 0)
                    {
                        s_brakeNotch = 0;
                    }
                    if(s_brakeNotch > GetBrakeMax())
                    {
                        s_brakeNotch = GetBrakeMax();
                    }
                    if (s_powerNotch < GetPowerMin())
                    {
                        s_powerNotch = GetPowerMin();
                    }
                    if (s_powerNotch > GetPowerMax())
                    {
                        s_powerNotch = GetPowerMax();
                    }
                    bool two = IsTwoHandle();
                    if (s_brakeNotch == 0 && s_powerNotch == 0)
                    {
                        if (two)
                        {
                            OnLeverMoved(1, 0);
                            OnLeverMoved(2, 0);
                        }
                        else
                        {
                            OnLeverMoved(3, 0);
                        }
                    }else if(s_brakeNotch > 0)
                    {
                        if (two)
                        {
                            OnLeverMoved(2, s_brakeNotch);
                        }
                        else
                        {
                            OnLeverMoved(3, -s_brakeNotch);
                        }
                    }
                    else
                    {
                        if (two)
                        {
                            OnLeverMoved(1, s_powerNotch);
                        }
                        else
                        {
                            OnLeverMoved(3, s_powerNotch);
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

        private void OnKeyUp(int axis, int keyCode)
        {
            if (KeyUp != null)
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

        // BveEx連携機能
        private static void BveExPluginSendOutputThreadFrame()
        {
            while (_ControllerOutputThreadAlive)
            {
                if (ConfigFormInstance != null && !ConfigFormInstance.IsDisposed)
                {
                    Thread.Sleep(100);
                    continue;
                }
                // リストの改変を防ぐため一度コピーする
                List<NCIController> controllers = new List<NCIController>();
                controllers.AddRange(Controllers);
                foreach (NCIController controller in controllers)
                {
                    if (SettingsInstance != null)
                    {
                        if (SettingsInstance.GetIsEnabled(controller.GetName()))
                        {
                            controller.SendOutput();
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        public static void BveExPluginSetVersion(Version version)
        {
            BveExPluginVersion = version;
            _ControllerOutputThread = new Thread(BveExPluginSendOutputThreadFrame);
            _ControllerOutputThread.Start();
            _ControllerOutputThreadAlive = true;
        }

        public static void BveExPluginDisposed()
        {
            _ControllerOutputThreadAlive = false;
        }

        // 利用可能な機能の一覧を設定
        // キー、値の型、表示名
        public static void BveExPluginReportAvailableValues(List<object[]> values)
        {
            BveExPluginAvailableValues = values;
        }

        // 値の変更を通知
        // キー、値
        public static void BveExPluginReportValueChanged(string key, object value)
        {
            if (key == "BveTypes.ClassWrappers.HandleSet.PowerNotch" && value.GetType() == typeof(int))
            {
                var val = (int)value;
                if (0 <= val && GetPowerMax() >= val)
                {
                    s_powerNotch = val;
                    s_brakeNotch = 0; // 力行を上げたら制動は切る
                }
            }
            if (key == "BveTypes.ClassWrappers.HandleSet.BrakeNotch" && value.GetType() == typeof(int))
            {
                var val = (int)value;
                if (0 <= val && GetBrakeMax() >= val)
                {
                    s_brakeNotch = val;
                    s_powerNotch = 0; // 制動を上げたら力行は切る
                }
            }
            foreach (NCIController controller in Controllers)
            {
                ControllerProfile profile = SettingsInstance.GetProfile(controller);
                if (SettingsInstance.GetIsEnabled(controller.GetName()) && profile != null && controller.HasOutputs())
                {
                    foreach (var bveExValue in profile.BveExValue)
                    {
                        if (key == bveExValue.Value)
                        {
                            OutputType outputType = controller.GetOutputs()[bveExValue.Key];
                            if (value.GetType() == typeof(int))
                            {
                                switch (outputType)
                                {
                                    case OutputType.Int:
                                        controller.SetOutput(bveExValue.Key, (int)value);
                                        break;
                                    case OutputType.Double:
                                        controller.SetOutput(bveExValue.Key, (double)(int)value);
                                        break;
                                }
                            }
                            if (value.GetType() == typeof(float))
                            {
                                switch (outputType)
                                {
                                    case OutputType.Int:
                                        controller.SetOutput(bveExValue.Key, (int)(float)value);
                                        break;
                                    case OutputType.Double:
                                        controller.SetOutput(bveExValue.Key, (double)(float)value);
                                        break;
                                }
                            }
                            if (value.GetType() == typeof(double))
                            {
                                switch (outputType)
                                {
                                    case OutputType.Int:
                                        controller.SetOutput(bveExValue.Key, (int)(double)value);
                                        break;
                                    case OutputType.Double:
                                        controller.SetOutput(bveExValue.Key, (double)value);
                                        break;
                                }
                            }
                            if (value.GetType() == typeof(bool))
                            {
                                switch (outputType)
                                {
                                    case OutputType.Bool:
                                        controller.SetOutput(bveExValue.Key, (bool)value);
                                        break;
                                }
                            }
                            if (value.GetType() == typeof(string))
                            {
                                switch (outputType)
                                {
                                    case OutputType.String:
                                        controller.SetOutput(bveExValue.Key, (string)value);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        // イベントの発生を通知
        // キー
        public static void BveExPluginReportEventFired(string key)
        {

        }

        // NumerousControllerInterfaceが使用する機能の一覧を取得
        public static List<string> BveExPluginGetUseValueList()
        {
            List<string> values = new List<string>
            {
                "BveTypes.ClassWrappers.HandleSet.PowerNotch",
                "BveTypes.ClassWrappers.HandleSet.BrakeNotch"
            };
            foreach (NCIController controller in Controllers)
            {
                if (SettingsInstance.GetIsEnabled(controller.GetName()))
                {
                    ControllerProfile profile = SettingsInstance.GetProfile(controller);
                    if (profile != null)
                    {
                        if (profile.BveExValue != null) {
                            foreach (string key in profile.BveExValue.Values)
                            {
                                if (!values.Contains(key))
                                {
                                    values.AddRange(profile.BveExValue.Values);
                                }
                            }
                        }
                    }
                }
            }
            return values;
        }
    }
}
