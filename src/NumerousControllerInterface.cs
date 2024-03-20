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

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class NumerousControllerInterface : IInputDevice
    {
        private static bool DebugUpdater = false;
        public static int IntVersion { get { return 13; } }
        public static string UserAgent = "NumerousContollerInterfaceUpdater v" + IntVersion;

        public static List<NCIController> Controllers;

        public static Settings SettingsInstance = null;
        public static ConfigForm ConfigFormInstance;
        private static SelectMasterControllerForm s_selectMasterControllerForm;

        public event InputEventHandler KeyDown;
        public event InputEventHandler KeyUp;
        public event InputEventHandler LeverMoved;

        private Dictionary<NCIController, int> _prePowerLevel;
        private Dictionary<NCIController, int> _preBreakLevel;
        private Dictionary<NCIController, List<int>> _preButtons;
        private Dictionary<NCIController, Reverser> _preReverser;

        public static List<NumerousControllerPlugin> Plugins;

        private int _onePowerMax;
        private int _oneBreakMax;
        private int _twoPowerMax;
        private int _twoBreakMax;
        private int _powerNotch;
        private int _breakNotch;

        private static int s_preControllerCount;
        private static int s_preEnabledMasterControllerCount;

        private static string s_powerController;
        private static string s_breakController;
        private static string s_reverserController;

        public static bool IsMasterControllerUpdateRequested;

        public static Timer TimerController;
        private bool _isUpdateController;
        private bool _isDisposeRequested;
        private static bool s_isRunningGetAllControllers;
        public static Version AtsExPluginVersion;

        public NumerousControllerInterface()
        {
            // SlimDXのバージョンを無視する
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            ButtonFeature.Initialize();
            Controllers = new List<NCIController>();
            _prePowerLevel = new Dictionary<NCIController, int>();
            _preBreakLevel = new Dictionary<NCIController, int>();
            _preButtons = new Dictionary<NCIController, List<int>>();
            _preReverser = new Dictionary<NCIController, Reverser>();
            Plugins = new List<NumerousControllerPlugin>();
            s_powerController = "";
            s_breakController = "";
            s_reverserController = "";
            s_preControllerCount = -1;
            if (TimerController == null)
            {
                TimerController = new Timer();
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

        private static void CheckUpdates()
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
                            if (intVersion > IntVersion)
                            {
                                string downloadFilePath = Path.Combine(downloadTmpDir, target.Element("Asset").Value);
                                string installer = "";

                                if (target.Element("Installer") != null)
                                {
                                    installer = target.Element("Installer").Value;
                                }
                                using (UpdateForm form = new UpdateForm(
                                    target.Element("Version").Value,
                                    history,
                                    target.Element("DownloadPage").Value,
                                    target.Element("DownloadUrl").Value,
                                    downloadFilePath,
                                    installer
                                    ))
                                {
                                    form.ShowDialog();
                                }
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
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
            bool isBreakControllerExists = false;
            bool isReverserControllerExists = false;
            foreach (NCIController controller in Controllers)
            {
                ControllerProfile profile = SettingsInstance.GetProfile(controller);
                if (profile == null) continue;
                if (s_powerController.Equals(controller.GetName()) && profile.HasPower(controller))
                {
                    isPowerControllerExists = true;
                }
                if (s_breakController.Equals(controller.GetName()) && profile.HasBreak(controller))
                {
                    isBreakControllerExists = true;
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
            if (!isBreakControllerExists)
            {
                s_breakController = "";
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
                ConfigFormInstance.updateControllers();
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
                        if (profile.HasPower(controller) && profile.HasBreak(controller))
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
                    List<string> breakControllerList = new List<string>();
                    int[] breakList = new int[] { ButtonFeature.BringNotchUp.Value, ButtonFeature.BringNotchDown.Value, ButtonFeature.BringBreakUp.Value, ButtonFeature.BringBreakDown.Value };
                    foreach (NCIController controller in Controllers)
                    {
                        ControllerProfile profile = SettingsInstance.GetProfile(controller);
                        if (profile == null) continue;
                        bool hasBreak = false;
                        if (profile.HasBreak(controller))
                        {
                            hasBreak = true;
                        }
                        else
                        {
                            foreach (ButtonFeature feature in profile.KeyMap.Values)
                            {
                                if (breakList.Contains(feature.Value))
                                {
                                    hasBreak = true;
                                    break;
                                }
                            }
                        }
                        if (hasBreak)
                        {
                            breakControllerList.Add(controller.GetName());
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
                                s_breakController = controller;
                            }))
                            {
                                s_selectMasterControllerForm.ShowDialog();
                            }
                        }
                        else if (masterControllerList.Count == 1)
                        {
                            s_powerController = masterControllerList[0];
                            s_breakController = masterControllerList[0];
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
                        if (breakControllerList.Count >= 2)
                        {
                            using (s_selectMasterControllerForm = new SelectMasterControllerForm(breakControllerList, "制動のみ持つコントローラー", controller =>
                            {
                                s_breakController = controller;
                            }))
                            {
                                s_selectMasterControllerForm.ShowDialog();
                            }
                        }
                        else if (breakControllerList.Count == 1)
                        {
                            s_breakController = breakControllerList[0];
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
            }
            if (checkUpdates)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    CheckUpdates();
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
            _onePowerMax = ranges[3][1] + 1;
            _oneBreakMax = -ranges[3][0] + 1;
            _twoPowerMax = ranges[2][1] + 1;
            _twoBreakMax = -ranges[2][0] + 1;
        }

        // 力行の最大を取得
        public int GetPowerMax()
        {
            if(_onePowerMax > 1)
            {
                return _onePowerMax;
            }
            return _twoPowerMax;
        }

        // 制動の最大を取得
        public int GetBreakMax()
        {
            if (_oneBreakMax > 1)
            {
                return _oneBreakMax;
            }
            return _twoBreakMax;
        }

        // 運転中の車両がツーハンドルかどうか
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
                            // 逆回しのために中央位置分だけ減らす
                            onLeverMoved(1, powerLevel - profile.PowerCenterPosition);
                        }
                        else
                        {
                            onLeverMoved(3, powerLevel);
                        }
                    }
                    _prePowerLevel[controller] = powerLevel;
                }
                // 制動
                if (controller.GetName().Equals(s_breakController))
                {
                    int breakLevel = profile.GetBreak(controller, GetBreakMax());
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
                    _preBreakLevel[controller] = breakLevel;
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
                        onLeverMoved(0, rev);
                    }
                    _preReverser[controller] = reverserPos;
                }
                // ボタン 一瞬だけ押すために前Tickでボタンを押していたかどうかを判定
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
            foreach (NumerousControllerPlugin plugin in Plugins)
            {
                plugin.Dispose();
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
                            _breakNotch = GetBreakMax() - 1;
                            _powerNotch = 0;
                            break;
                        case 1:// 全て切にする
                            _breakNotch = 0;
                            _powerNotch = 0;
                            break;
                        case 2:// 制動切
                            _breakNotch = 0;
                            break;
                        case 3:// 制動上げ
                            _breakNotch++;
                            break;
                        case 4:// 制動下げ
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

        // AtsEx連携機能
        public static void AtsExPluginSetVersion(Version version)
        {
            AtsExPluginVersion = version;
        }

        public static void AtsExPluginDisposed()
        {

        }

        // 利用可能な機能の一覧を設定
        public static void AtsExPluginReportAvailableFeatures(List<Tuple<string, Type>> features)
        {

        }

        // 値の変更を通知
        public static void AtsExPluginReportValueChanged(string key, object value)
        {

        }

        // イベントの発生を通知
        public static void AtsExPluginReportEventFired(string key)
        {

        }

        // NumerousControllerInterfaceが使用する機能の一覧を取得
        public static List<string> AtsExPluginGetUseFeatureList()
        {
            return new List<string>();
        }
    }
}
