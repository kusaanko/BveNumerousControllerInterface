using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Diagnostics;
using Mackoy.Bvets;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class NumerousControllerInterface : IInputDevice
    {
        public static int IntVersion { get { return 5; } }

        public static DirectInput Input;
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

        public static bool IsMasterControllerUpdateRequested;

        public static Timer TimerController;
        private bool _isUpdateController;
        private bool _isDisposeRequested;
        private static bool s_isRunningGetAllControllers;

        public NumerousControllerInterface()
        {
            ButtonFeature.Initialize();
            Controllers = new List<NCIController>();
            _prePowerLevel = new Dictionary<NCIController, int>();
            _preBreakLevel = new Dictionary<NCIController, int>();
            _preButtons = new Dictionary<NCIController, List<int>>();
            s_powerController = "";
            s_breakController = "";
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
            string arch = BinaryInfo.arch;
            string net_ver = BinaryInfo.net_ver;
            string update_url = "https://raw.githubusercontent.com/kusaanko/BveNumerousControllerInterface/main/update_info.json";
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                    client.Encoding = System.Text.Encoding.UTF8;

                    string content = client.DownloadString(update_url);
                    Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    object target = json[arch + "_" + net_ver];
                    if (target != null && target.GetType() == typeof(JObject))
                    {
                        JObject update_info = (JObject)target;
                        string latestTag = (string)update_info.GetValue("latest");
                        int intVersion = (int)update_info.GetValue("int_version");
                        if (intVersion > IntVersion)
                        {
                            // 更新画面を出す
                            string history = "";
                            try
                            {
                                string url = (string)json["release_url"];
                                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                                content = client.DownloadString(url);
                                List<object> assets_json = JsonConvert.DeserializeObject<List<object>>(content);
                                bool startLogging = false;
                                int historyCount = 0;
                                foreach (object obj in assets_json)
                                {
                                    if (obj.GetType() == typeof(JObject))
                                    {
                                        JObject asset = (JObject)obj;
                                        string tag = ((string)asset.GetValue("tag_name"));
                                        if (tag.Equals(latestTag))
                                        {
                                            startLogging = true;
                                        }
                                        if (startLogging)
                                        {
                                            historyCount++;
                                            history += asset.GetValue("name") + "\n";
                                            history += asset.GetValue("body") + "\n\n";
                                        }
                                        if (historyCount > 20) break;
                                    }
                                }
                            }
                            catch (Exception) { }
                            string downloadFilePath = Path.Combine(downloadTmpDir, (string)update_info.GetValue("asset"));
                            using (UpdateForm form = new UpdateForm(
                                (string)update_info.GetValue("version"),
                                history,
                                (string)update_info.GetValue("download_page"),
                                (string)update_info.GetValue("download_url"),
                                downloadFilePath
                                ))
                            {
                                form.ShowDialog();
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            if ((s_selectMasterControllerForm == null || s_selectMasterControllerForm.IsDisposed) && !s_isRunningGetAllControllers) 
            {
                GetAllControllers();
            }
        }

        public static void GetAllControllers()
        {
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
            foreach (NCIController controller in Controllers)
            {
                ControllerProfile profile = SettingsInstance.GetProfile(controller);
                if (s_powerController.Equals(controller.GetName()) && profile.HasPower(controller))
                {
                    isPowerControllerExists = true;
                }
                if (s_breakController.Equals(controller.GetName()) && profile.HasBreak(controller))
                {
                    isBreakControllerExists = true;
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
                    bool isMasterControllerOnly = true;
                    List<string> masterControllerList = new List<string>();
                    int[] masterList = new int[] { ButtonFeature.BringNotchUp.Value, ButtonFeature.BringNotchDown.Value };
                    foreach (NCIController controller in Controllers)
                    {
                        ControllerProfile profile = SettingsInstance.GetProfile(controller);
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
            Input = new DirectInput();
            SettingsInstance = Settings.LoadFromXml(settingsPath);

            GetAllControllers();
            // 更新の確認はバックグラウンドで行う

            bool checkUpdates = SettingsInstance.CheckUpdates;

            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attributes.Length == 1)
            {
                AssemblyConfigurationAttribute assemblyConfiguration = attributes[0] as AssemblyConfigurationAttribute;
                if (assemblyConfiguration != null)
                {
                    if (assemblyConfiguration.Configuration.Equals("Debug")) checkUpdates = false;
                }
            }
            if (checkUpdates)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    CheckUpdates();
                })).Start();
            }
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
                            onLeverMoved(1, powerLevel);
                        }
                        else
                        {
                            onLeverMoved(3, powerLevel);
                        }
                    }
                    _prePowerLevel[controller] = powerLevel;
                }
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
    }
}
