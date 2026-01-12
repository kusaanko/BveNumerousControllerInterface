using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ConfigForm : Form
    {
        private Dictionary<string, NCIController> _controllers = new Dictionary<string, NCIController>();
        private List<string> _buttonFeatureIdIndex = new List<string>();
        private System.ComponentModel.ComponentResourceManager resources;
        public ControllerSetupForm ControllerSetupForm;
        private List<string> _BveExValueKey = new List<string>();

        public ConfigForm()
        {
            InitializeComponent();
            foreach (string featureId in ButtonFeature.Features.Keys)
            {
                _buttonFeatureIdIndex.Add(featureId);
                buttonFunctionComboBox.Items.Add(ButtonFeature.Features[featureId].Name);
            }
            flexiblePowerModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.None]);
            flexiblePowerModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.Flexible]);
            flexiblePowerModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.LastMax]);
            flexibleBrakeModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.None]);
            flexibleBrakeModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.EBFixed]);
            flexibleBrakeModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.FlexibleWithoutEB]);
            flexibleBrakeModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.Flexible]);
            foreach (NumerousControllerPlugin plugin in NumerousControllerInterface.Plugins)
            {
                pluginConfigComboBox.Items.Add(plugin.GetName());
            }
            if (pluginConfigComboBox.Items.Count > 0)
            {
                pluginConfigComboBox.SelectedIndex = 0;
            }
            this.resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ControllerProfile.GetAllControllers();
            UpdateControllers();
            timer1.Start();
            alertNoCountrollerFoundCheckBox.Checked = NumerousControllerInterface.SettingsInstance.AlertNoControllerFound;
            checkUpdatesCheckBox.Checked = NumerousControllerInterface.SettingsInstance.CheckUpdates;
            SetComPortEnabled(false);
            bveExStatusLabel.Text = NumerousControllerInterface.BveExPluginVersion != null ? "BveEXプラグイン読み込み済み(" + NumerousControllerInterface.BveExPluginVersion.ToString() + ")" : "BveEXプラグイン未検出";
        }

        public void UpdateControllers()
        {
            timer1.Stop();
            controllerList.Items.Clear();
            _controllers.Clear();
            foreach (NCIController controller in ControllerProfile.controllers)
            {
                controllerList.Items.Add(controller.GetName());
                _controllers.Add(controller.GetName(), controller);
            }
            UpdateProfile();
            SetEnabled(false);
            // COMポートを更新
            availableComPortList.Items.Clear();
            usingComPortList.Items.Clear();
            foreach (string port in SerialPort.GetPortNames())
            {
                if (NumerousControllerInterface.SettingsInstance.EnabledComPorts.Contains(port))
                {
                    usingComPortList.Items.Add(port);
                } else
                {
                    availableComPortList.Items.Add(port);
                }
            }
            UpdateCOMControllerSettings();
            timer1.Start();
        }

        private void SetEnabled(bool enabled)
        {
            isEnabledCheckBox.Enabled = enabled;
            profileComboBox.Enabled = enabled;
            newProfileButton.Enabled = enabled;
            changeNameButton.Enabled = enabled;
            duplicateProfileButton.Enabled = enabled;
            removeProfileButton.Enabled = enabled;
        }

        private void SetComPortEnabled(bool enabled)
        {
            comPortProfileComboBox.Enabled = enabled;
            comPortNewProfileButton.Enabled = enabled;
            comPortChangeProfileNameButton.Enabled = enabled;
            comPortDuplicateProfileButton.Enabled = enabled;
            comPortDeleteProfileButton.Enabled = enabled;
            comPortNotSupportedCheckBox.Enabled = enabled;
        }

        private void SelectDropDownList(ComboBox list, ButtonFeature assign)
        {
            list.SelectedIndex = _buttonFeatureIdIndex.IndexOf(assign.Id);
        }

        private void UpdateProfile()
        {
            profileComboBox.Items.Clear();
            foreach (string name in NumerousControllerInterface.SettingsInstance.Profiles.Keys)
            {
                profileComboBox.Items.Add(name);
            }
        }

        private void SelectProfile(string profile)
        {
            if (profile == null)
            {
                profileComboBox.SelectedIndex = -1;
            }
            else
            {
                for (int i = 0; i < profileComboBox.Items.Count; i++)
                {
                    if (profile.Equals(profileComboBox.Items[i]))
                    {
                        profileComboBox.SelectedIndex = i;
                    }
                }
            }
            LoadFromProfile();
            SetEnabled(true);
        }

        private void LoadFromProfile()
        {
            if (profileComboBox.SelectedIndex != -1)
            {
                ControllerProfile profile = NumerousControllerInterface.SettingsInstance.Profiles[profileComboBox.Text];
                isTwoHandleComboBox.Checked = profile.IsTwoHandle;
                powerCenterPositionNumericUpDown.Enabled = profile.IsTwoHandle;
                powerCenterPositionNumericUpDown.Value = profile.PowerCenterPosition;
                flexiblePowerModeComboBox.SelectedItem = ControllerProfile.FlexibleNotchModeStrings[(int)profile.FlexiblePower];
                flexibleBrakeModeComboBox.SelectedItem = ControllerProfile.FlexibleNotchModeStrings[(int)profile.FlexibleBrake];
                powerCenterPositionNumericUpDown.Value = profile.PowerCenterPosition;
                buttonList.Items.Clear();
                foreach (int i in profile.KeyMap.Keys)
                {
                    buttonList.Items.Add(GetButtonName(i));
                }
                settingPowerButton.Enabled = GetController().GetPowerCount() == 0;
                settingBrakeButton.Enabled = GetController().GetBrakeCount() == 0;
                removePowerButton.Enabled = GetController().GetPowerCount() == 0;
                removeBrakeButton.Enabled = GetController().GetBrakeCount() == 0;

                settingsTabControl.Enabled = true;

                if (NumerousControllerInterface.BveExPluginVersion != null && GetController().HasOutputs())
                {
                    dataOutputListBox.Items.Clear();
                    foreach (var pair in GetController().GetOutputs())
                    {
                        dataOutputListBox.Items.Add(pair.Key);
                    }
                    bveExConfigurationTabItem.Enabled = true;
                }
                else
                {
                    bveExConfigurationTabItem.Enabled = false;
                }
            } else
            {
                settingsTabControl.Enabled = false;
            }
        }

        private void LoadControllerEnabled()
        {
            isEnabledCheckBox.Checked = NumerousControllerInterface.SettingsInstance.GetIsEnabled(controllerList.Text);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ControllerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string controller = controllerList.Text;
            if (controller == null || controller.Equals("")) return;
            if (NumerousControllerInterface.SettingsInstance.ProfileMap.ContainsKey(controller))
            {
                SelectProfile(NumerousControllerInterface.SettingsInstance.ProfileMap[controller]);
            } else
            {
                SelectProfile(null);
            }
            LoadFromProfile();
            LoadControllerEnabled();
            controllerTypeLabel.Text = this.resources.GetString("controllerTypeLabel.Text") + GetController().GetControllerType();
        }
        private void UpdateCOMControllerSettings()
        {
            comPortProfileComboBox.Items.Clear();
            foreach (string name in NumerousControllerInterface.SettingsInstance.COMControllerSettings.Keys)
            {
                comPortProfileComboBox.Items.Add(name);
            }
        }

        private void SelectCOMControllerSettings(string settings)
        {
            if (settings == null)
            {
                comPortProfileComboBox.SelectedIndex = -1;
            } else
            {
                for (int i = 0; i < comPortProfileComboBox.Items.Count; i++)
                {
                    if (settings.Equals(comPortProfileComboBox.Items[i]))
                    {
                        comPortProfileComboBox.SelectedIndex = i;
                    }
                }
            }
            LoadFromCOMControllerSettings();
            SetComPortEnabled(true);
        }

        private void LoadFromCOMControllerSettings()
        {
            if (comPortProfileComboBox.SelectedIndex != -1)
            {
                COMControllerSettings settings = NumerousControllerInterface.SettingsInstance.COMControllerSettings[comPortProfileComboBox.SelectedItem.ToString()];
                comPortDtrCheckBox.Checked = settings.DtrEnable;
                comPortRtsCheckBox.Checked = settings.RtsEnable;
                comPortNotSupportedCheckBox.Checked = settings.IsNotSupported;
                SelectCOMPortBaudRate(settings.BaudRate);

                comPortDtrCheckBox.Enabled = true;
                comPortRtsCheckBox.Enabled = true;
                comPortBaudRateComboBox.Enabled = true;
                comPortNotSupportedCheckBox.Enabled = true;
            } else
            {
                comPortDtrCheckBox.Enabled = false;
                comPortRtsCheckBox.Enabled = false;
                comPortBaudRateComboBox.Enabled = false;
                comPortNotSupportedCheckBox.Enabled = false;
            }
        }

        private void SelectCOMPortBaudRate(int baudRate)
        {
            foreach (string rate in comPortBaudRateComboBox.Items)
            {
                if (rate == baudRate + "")
                {
                    comPortBaudRateComboBox.SelectedItem = baudRate + "";
                }
            }
        }

        private COMControllerSettings GetCOMControllerSettings()
        {
            if (!NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(comPortProfileComboBox.SelectedItem.ToString())) return null;
            return NumerousControllerInterface.SettingsInstance.COMControllerSettings[comPortProfileComboBox.SelectedItem.ToString()];
        }

        private ControllerProfile GetProfile()
        {
            if (!NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey(profileComboBox.Text)) return null;
            return NumerousControllerInterface.SettingsInstance.Profiles[profileComboBox.Text];
        }

        private NCIController GetController()
        {
            if (!_controllers.ContainsKey(controllerList.Text)) return null;
            return _controllers[controllerList.Text];
        }

        private string GetButtonName(int index)
        {
            string[] names = GetController().GetButtonNames();
            if (names.Length > index)
            {
                return names[index];
            }
            return index + "";
        }

        private int GetButtonIndex(string name)
        {
            string[] names = GetController().GetButtonNames();
            if (names.Length > 0)
            {
                for (int i = 0;i < names.Length; i++)
                {
                    if(names[i] == name)
                    {
                        return i;
                    }
                }
            }
            return Convert.ToInt32(name);
        }

        private void IsTwoHandleComboBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsTwoHandle = isTwoHandleComboBox.Checked;
            powerCenterPositionNumericUpDown.Enabled = isTwoHandleComboBox.Checked;
        }

        private void AddButtonButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            NCIController controller = GetController();
            ControllerProfile profile = GetProfile();
            addButtonButton.Text = "ボタンを押す...";
            List<int> buttons = profile.GetButtons(controller);
            Thread thread = new Thread(new ThreadStart(() => {
                int buttonIndex = -1;
                while (true)
                {
                    List<int> list = profile.GetButtons(controller);
                    foreach (int button in list)
                    {
                        if (!buttons.Contains(button))
                        {
                            buttonIndex = button;
                            goto loop;
                        }
                    }
                    Thread.Sleep(100);
                }
                loop:
                Invoke(new Action(() =>
                {
                    if (buttonIndex != -1)
                    {
                        if (buttonList.Items.Contains(GetButtonName(buttonIndex)))
                        {
                            buttonList.SelectedItem = GetButtonName(buttonIndex);
                        }
                        else
                        {
                            buttonList.Items.Add(GetButtonName(buttonIndex));
                            GetProfile().KeyMap.Add(buttonIndex, ButtonFeature.Ats0);
                        }
                    }
                    timer1.Start();
                    addButtonButton.Text = this.resources.GetString("addButtonButton.Text");
                }));
            }));
            thread.Start();
        }

        private void ButtonList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(buttonList.SelectedIndex < 0)
            {
                buttonFunctionComboBox.Enabled = false;
                holdToRepeatButtonCheckBox.Enabled = false;
                return;
            }
            buttonFunctionComboBox.Enabled = true;
            holdToRepeatButtonCheckBox.Enabled = true;
            var profile = GetProfile();
            var buttonIndex = GetButtonIndex((string)buttonList.SelectedItem);
            SelectDropDownList(buttonFunctionComboBox, profile.KeyMap[buttonIndex]);
            if (profile.HoldToRepeat.ContainsKey(buttonIndex))
            {
                holdToRepeatButtonCheckBox.Checked = profile.HoldToRepeat[buttonIndex];
            } else
            {
                holdToRepeatButtonCheckBox.Checked = false;
            }
            if (profile.HoldToRepeatTime.ContainsKey(buttonIndex))
            {
                timeToRepeatPressNumericUpDown.Value = (decimal) profile.HoldToRepeatTime[buttonIndex];
            }
            else
            {
                timeToRepeatPressNumericUpDown.Value = (decimal) 0.5;
            }
        }

        private void RemoveButtonButton_Click(object sender, EventArgs e)
        {
            if (buttonList.SelectedIndex >= 0)
            {
                GetProfile().KeyMap.Remove(GetButtonIndex((string)buttonList.SelectedItem));
                LoadFromProfile();
            }
        }

        private void ButtonFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buttonList.SelectedItem == null) return;
            if (GetProfile().KeyMap.ContainsKey(GetButtonIndex((string)buttonList.SelectedItem)))
            {
                GetProfile().KeyMap[GetButtonIndex((string)buttonList.SelectedItem)] = ButtonFeature.Features[_buttonFeatureIdIndex[buttonFunctionComboBox.SelectedIndex]];
            }
        }

        private void SettingPowerButton_Click(object sender, EventArgs e)
        {
            using (ControllerSetupForm = new ControllerSetupForm(
                _controllers[controllerList.Text],
                GetProfile(),
                true))
            {
                ControllerSetupForm.ShowDialog(this);
            }
        }

        private void SettingBrakeButton_Click(object sender, EventArgs e)
        {
            using (ControllerSetupForm = new ControllerSetupForm(
                _controllers[controllerList.Text], 
                GetProfile(), 
                false))
            {
                ControllerSetupForm.ShowDialog(this);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            NCIController controller = GetController();
            ControllerProfile profile = GetProfile();
            if (controller == null || profile == null) return;
            if (profile.HasBrake(controller))
            {
                var newText = this.resources.GetString("brakeLabel.Text") + profile.GetBrake(controller, profile.GetBrakeCount(controller));
                if (brakeLabel.Text != newText)
                {
                    brakeLabel.Text = newText;
                }
            } else
            {
                var newText = this.resources.GetString("brakeLabel.Text") + "なし";
                if (brakeLabel.Text != newText)
                {
                    brakeLabel.Text = newText;
                }
            }
            if (profile.HasPower(controller))
            {
                var newText = this.resources.GetString("powerLabel.Text") + profile.GetPower(controller, profile.GetPowerCount(controller));
                if (powerLabel.Text != newText)
                {
                    powerLabel.Text = newText;
                }
            } else
            {
                var newText = this.resources.GetString("powerLabel.Text") + "なし";
                if (powerLabel.Text != newText)
                {
                    powerLabel.Text = newText;
                }
            }
            var buttonText = this.resources.GetString("buttonLabel.Text");
            foreach (int i in profile.GetButtons(controller))
            {
                buttonText += GetButtonName(i) + " ";
                if (buttonList.Items.Contains(GetButtonName(i)))
                {
                    buttonList.SelectedItem = GetButtonName(i);
                    break;
                }
            }
            if (buttonLabel.Text != buttonText)
            {
                buttonLabel.Text = buttonText;
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            NumerousControllerInterface.IsMasterControllerUpdateRequested = true;

            // 設定を保存
            NumerousControllerInterface.SettingsInstance?.SaveToXml();
        }

        private void ProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFromProfile();
            if(NumerousControllerInterface.SettingsInstance.ProfileMap.ContainsKey(controllerList.Text))
            {
                NumerousControllerInterface.SettingsInstance.ProfileMap[controllerList.Text] = profileComboBox.Text;
            }
            else
            {
                NumerousControllerInterface.SettingsInstance.ProfileMap.Add(controllerList.Text, profileComboBox.Text);
            }
        }

        private void NewProfileButton_Click(object sender, EventArgs e)
        {
            string name = controllerList.Text;
            int i = 1;
            while(true)
            {
                if (NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey(name + i))
                {
                    i++;
                }else
                {
                    break;
                }
            }
            name = name + i;
            using(NewNameDialog dialog = new NewNameDialog(name, (s) =>
            {
                if(NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }else
                {
                    ControllerProfile newProfile = new ControllerProfile(s);
                    newProfile.Name = s;
                    NumerousControllerInterface.SettingsInstance.Profiles.Add(s, newProfile);
                }
                UpdateProfile();
                SelectProfile(s);
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void ChangeNameButton_Click(object sender, EventArgs e)
        {
            string oldName = profileComboBox.Text;
            using (NewNameDialog dialog = new NewNameDialog(profileComboBox.Text, (s) =>
            {
                if (NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    ControllerProfile profile = GetProfile();
                    profile.Name = s;
                    if (!NumerousControllerInterface.SettingsInstance.removeProfilesList.Contains(oldName)) NumerousControllerInterface.SettingsInstance.removeProfilesList.Add(oldName);
                    NumerousControllerInterface.SettingsInstance.Profiles.Remove(oldName);
                    NumerousControllerInterface.SettingsInstance.Profiles.Add(s, profile);
                    List<string> changeNames = new List<string>();
                    foreach (string key in NumerousControllerInterface.SettingsInstance.ProfileMap.Keys)
                    {
                        if (NumerousControllerInterface.SettingsInstance.ProfileMap[key].Equals(oldName))
                        {
                            changeNames.Add(key);
                        }
                    }
                    foreach (string key in changeNames)
                    {
                        NumerousControllerInterface.SettingsInstance.ProfileMap[key] = s;
                    }
                    NumerousControllerInterface.SettingsInstance.removeProfilesList.Remove(s);
                    UpdateProfile();
                    SelectProfile(s);
                }
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }

        }

        private void DuplicateProfileButton_Click(object sender, EventArgs e)
        {
            using (NewNameDialog dialog = new NewNameDialog(profileComboBox.Text, (s) =>
            {
                if (NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    ControllerProfile newProfile = GetProfile().Clone();
                    newProfile.Name = s;
                    NumerousControllerInterface.SettingsInstance.Profiles.Add(s, newProfile);
                    UpdateProfile();
                    SelectProfile(s);
                }
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void RemoveProfileButton_Click(object sender, EventArgs e)
        {
            string name = profileComboBox.Text;
            if (MessageBox.Show("本当に " + name + " を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                NumerousControllerInterface.SettingsInstance.Profiles.Remove(name);
                List<string> removeNames = new List<string>();
                foreach (string key in NumerousControllerInterface.SettingsInstance.ProfileMap.Keys)
                {
                    if (NumerousControllerInterface.SettingsInstance.ProfileMap[key].Equals(name))
                    {
                        removeNames.Add(key);
                    }
                }
                foreach (string key in removeNames)
                {
                    NumerousControllerInterface.SettingsInstance.ProfileMap.Remove(key);
                }
                if (!NumerousControllerInterface.SettingsInstance.removeProfilesList.Contains(name)) NumerousControllerInterface.SettingsInstance.removeProfilesList.Add(name);
                UpdateProfile();
                SelectProfile(null);
            }
        }

        private void IsEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!NumerousControllerInterface.SettingsInstance.IsEnabled.ContainsKey(controllerList.Text))
            {
                NumerousControllerInterface.SettingsInstance.IsEnabled.Add(controllerList.Text, isEnabledCheckBox.Checked);
            }
            else
            {
                NumerousControllerInterface.SettingsInstance.IsEnabled[controllerList.Text] = isEnabledCheckBox.Checked;
            }
        }

        private void AlertNoCountrollerFoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.SettingsInstance.AlertNoControllerFound = alertNoCountrollerFoundCheckBox.Checked;
        }

        private void OpenProfileInExplorer_Click(object sender, EventArgs e)
        {
            string filePath = NumerousControllerInterface.SettingsInstance.GetProfileSavePath(GetProfile());
            if (!File.Exists(filePath))
            {
                NumerousControllerInterface.SettingsInstance.SaveProfileToXml(GetProfile());
            }

            try
            {
                Process.Start("explorer.exe", "/select," + filePath);
            }
            catch (Exception) { }
        }

        private void RemovePowerButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("本当に力行を削除しますか？", "NumerousControllerInput", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetProfile().ResetPower();
            }
        }

        private void RemoveBreakButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("本当に制動を削除しますか？", "NumerousControllerInput", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetProfile().ResetBrake();
            }
        }

        private void CheckUpdatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.SettingsInstance.CheckUpdates = checkUpdatesCheckBox.Checked;
        }

        private void FlexiblePowerModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int key in ControllerProfile.FlexibleNotchModeStrings.Keys)
            {
                string value = ControllerProfile.FlexibleNotchModeStrings[key];
                if (value.Equals(flexiblePowerModeComboBox.Text))
                {
                    GetProfile().FlexiblePower = (FlexibleNotchMode)key;
                    break;
                }
            }
        }

        private void FlexibleBreakModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(int key in ControllerProfile.FlexibleNotchModeStrings.Keys)
            {
                string value = ControllerProfile.FlexibleNotchModeStrings[key];
                if(value.Equals(flexibleBrakeModeComboBox.Text))
                {
                    GetProfile().FlexibleBrake = (FlexibleNotchMode)key;
                    break;
                }
            }
        }

        private void ShowLoadedPlugins_Click(object sender, EventArgs e)
        {
            string str = "";
            foreach (NumerousControllerPlugin plugin in NumerousControllerInterface.Plugins)
            {
                str += plugin.GetName() + " " + plugin.GetVersion() + "\n";
            }
            MessageBox.Show(str, "NumerousControllerInterface");
        }

        private void PluginSettingButton_Click(object sender, EventArgs e)
        {
            if (pluginConfigComboBox.SelectedIndex < 0) return;
            NumerousControllerInterface.Plugins[pluginConfigComboBox.SelectedIndex].ShowConfigForm();
        }

        private void ComPortNotSupportedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = comPortNotSupportedCheckBox.Checked;
            comPortTabControl.Enabled = enabled;
        }

        private void ComportUseButton_Click(object sender, EventArgs e)
        {
            if (availableComPortList.SelectedIndex < 0) return;
            NumerousControllerInterface.SettingsInstance.EnabledComPorts.Add(availableComPortList.SelectedItem.ToString());
            if (!NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.ContainsKey(availableComPortList.SelectedItem.ToString()))
            {
                string name = availableComPortList.SelectedItem.ToString();
                if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(name))
                {
                    int i = 1;
                    while (true)
                    {
                        if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(name + i))
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    name = name + i;
                    COMControllerSettings settings = new COMControllerSettings();
                    settings.Name = name;
                    NumerousControllerInterface.SettingsInstance.COMControllerSettings.Add(name, settings);
                    NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Add(availableComPortList.SelectedItem.ToString(), name);
                }

            }
            UpdateControllers();
        }

        private void ComPortDeleteButton_Click(object sender, EventArgs e)
        {
            if (usingComPortList.SelectedIndex < 0) return;
            NumerousControllerInterface.SettingsInstance.EnabledComPorts.Remove(usingComPortList.SelectedItem.ToString());
            NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Remove(usingComPortList.SelectedItem.ToString());
            UpdateControllers();
        }

        private void ComPortNewProfileButton_Click(object sender, EventArgs e)
        {
            string name = "無名のプロファイル";
            int i = 1;
            while (true)
            {
                if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(name + i))
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            name = name + i;
            using (NewNameDialog dialog = new NewNameDialog(name, (s) =>
            {
                if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    COMControllerSettings settings = new COMControllerSettings();
                    settings.Name = s;
                    NumerousControllerInterface.SettingsInstance.COMControllerSettings.Add(s, settings);
                    NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Add(usingComPortList.Text, s);
                }
                UpdateCOMControllerSettings();
                SelectCOMControllerSettings(s);
                COMController.IsUpdateNeeded = true;
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void ComPortChangeProfileNameButton_Click(object sender, EventArgs e)
        {
            string oldName = comPortProfileComboBox.Text;
            using (NewNameDialog dialog = new NewNameDialog(comPortProfileComboBox.Text, (s) =>
            {
                if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    COMControllerSettings settings = GetCOMControllerSettings();
                    settings.Name = s;
                    if (!NumerousControllerInterface.SettingsInstance.removeCOMControllerProfilesList.Contains(oldName)) NumerousControllerInterface.SettingsInstance.removeCOMControllerProfilesList.Add(oldName);
                    NumerousControllerInterface.SettingsInstance.COMControllerSettings.Remove(oldName);
                    NumerousControllerInterface.SettingsInstance.COMControllerSettings.Add(s, settings);
                    List<string> changeNames = new List<string>();
                    foreach (string key in NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Keys)
                    {
                        if (NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[key].Equals(oldName))
                        {
                            changeNames.Add(key);
                        }
                    }
                    foreach (string key in changeNames)
                    {
                        NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[key] = s;
                    }
                    NumerousControllerInterface.SettingsInstance.removeCOMControllerProfilesList.Remove(s);
                    UpdateCOMControllerSettings();
                    SelectCOMControllerSettings(s);
                    COMController.IsUpdateNeeded = true;
                }
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void UsingComPortList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usingComPortList.SelectedIndex != -1)
            {
                SetComPortEnabled(true);
                if (NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.ContainsKey(usingComPortList.SelectedItem.ToString()))
                {
                    SelectCOMControllerSettings(NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[usingComPortList.SelectedItem.ToString()]);
                } else
                {
                    SelectCOMControllerSettings(null);
                }
            }
        }

        private void ComPortApplyButton_Click(object sender, EventArgs e)
        {
            if (usingComPortList.SelectedIndex != -1)
            {
                COMController.StopCOMPort(usingComPortList.SelectedItem.ToString());
            }
        }

        private void ComPortBaudRateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null && comPortBaudRateComboBox.SelectedItem != null)
            {
                try
                {
                    settings.BaudRate = Convert.ToInt32(comPortBaudRateComboBox.SelectedItem.ToString());
                }
                catch { }
            }
        }

        private void ComPortDtrCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null)
            {
                settings.DtrEnable = comPortDtrCheckBox.Checked;
            }
        }

        private void ComPortRtsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null)
            {
                settings.RtsEnable = comPortRtsCheckBox.Checked;
            }
        }

        private void ComPortOnInitTextBox_TextChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null)
            {
                settings.OnInit = comPortOnInitTextBox.Text;
            }
        }

        private void ComPortInputReplaceTextBox_TextChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null)
            {
                if (settings.InputCommandReplaceDictionary == null)
                {
                    settings.InputCommandReplaceDictionary = new Dictionary<string, string>();
                }
                settings.InputCommandReplaceDictionary.Clear();
                // 置換をパース
                foreach (string line in comPortInputReplaceTextBox.Lines)
                {
                    if (line.Length > 0 && line.Contains("="))
                    {
                        string target = line.Substring(0, line.IndexOf("="));
                        string value = line.Substring(line.IndexOf("=") + 1);
                        settings.InputCommandReplaceDictionary.Add(target, value);
                    }
                }
            }
        }

        private void ComPortOutputReplaceTextBox_TextChanged(object sender, EventArgs e)
        {
            COMControllerSettings settings = GetCOMControllerSettings();
            if (settings != null)
            {
                if (settings.OutputCommandReplaceDictionary == null)
                {
                    settings.OutputCommandReplaceDictionary = new Dictionary<string, string>();
                }
                settings.OutputCommandReplaceDictionary.Clear();
                // 置換をパース
                foreach (string line in comPortOutputReplaceTextBox.Lines)
                {
                    if (line.Length > 0 && line.Contains("="))
                    {
                        string target = line.Substring(0, line.IndexOf("="));
                        string value = line.Substring(line.IndexOf("=") + 1);
                        settings.OutputCommandReplaceDictionary.Add(target, value);
                    }
                }
            }
        }

        private void ComPortProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (availableComPortList.SelectedItem != null)
            {
                NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Add(availableComPortList.SelectedItem.ToString(), comPortProfileComboBox.SelectedItem.ToString());
                LoadFromCOMControllerSettings();
                SetComPortEnabled(true);
            }
        }

        private void ComPortDuplicateProfileButton_Click(object sender, EventArgs e)
        {
            if (comPortProfileComboBox.SelectedItem != null)
            {
                using (NewNameDialog dialog = new NewNameDialog(comPortProfileComboBox.SelectedItem.ToString(), (s) =>
                {
                    if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(s))
                    {
                        MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                        return false;
                    }
                    else
                    {
                        COMControllerSettings newProfile = GetCOMControllerSettings().Clone();
                        newProfile.Name = s;
                        NumerousControllerInterface.SettingsInstance.COMControllerSettings.Add(s, newProfile);
                        UpdateCOMControllerSettings();
                        SelectCOMControllerSettings(s);
                    }
                    return true;
                }))
                {
                    dialog.ShowDialog(this);
                }
            }
        }

        private void ComPortDeleteProfileButton_Click(object sender, EventArgs e)
        {
            if (comPortProfileComboBox.SelectedItem != null)
            {
                string name = comPortProfileComboBox.SelectedItem.ToString();
                if (MessageBox.Show("本当に " + name + " を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    NumerousControllerInterface.SettingsInstance.COMControllerSettings.Remove(name);
                    List<string> removeNames = new List<string>();
                    foreach (string key in NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Keys)
                    {
                        if (NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[key].Equals(name))
                        {
                            removeNames.Add(key);
                        }
                    }
                    foreach (string key in removeNames)
                    {
                        NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Remove(key);
                    }
                    if (!NumerousControllerInterface.SettingsInstance.removeCOMSettingsList.Contains(name)) NumerousControllerInterface.SettingsInstance.removeCOMSettingsList.Add(name);
                    UpdateCOMControllerSettings();
                    profileComboBox.SelectedIndex = -1;
                }
            }
        }

        private void PowerCenterPositionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            ControllerProfile profile = GetProfile();
            if (profile != null)
            {
                profile.PowerCenterPosition = (int)powerCenterPositionNumericUpDown.Value;
            }
        }

        private void DataOutputListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataOutputListBox.SelectedItem != null)
            {
                dataBveExValueComboBox.Items.Clear();
                _BveExValueKey.Clear();
                OutputType outputType = GetController().GetOutputs()[dataOutputListBox.SelectedItem.ToString()];
                foreach (var value in NumerousControllerInterface.BveExPluginAvailableValues)
                {
                    if ((outputType == OutputType.Int && (value[1] == typeof(int) || value[1] == typeof(double) || value[1] == typeof(float))) ||
                        (outputType == OutputType.Double && (value[1] == typeof(int) || value[1] == typeof(double) || value[1] == typeof(float))) ||
                        (outputType == OutputType.Bool && (value[1] == typeof(bool))))
                    {
                        dataBveExValueComboBox.Items.Add(value[2]);
                        _BveExValueKey.Add((string)value[0]);
                    }
                }
                if (GetProfile().BveExValue == null)
                {
                    GetProfile().BveExValue = new Dictionary<string, string>();
                }
                if (GetProfile().BveExValue.ContainsKey(dataOutputListBox.SelectedItem.ToString()))
                {
                    string valueKey = GetProfile().BveExValue[dataOutputListBox.SelectedItem.ToString()];
                    int index =_BveExValueKey.FindIndex(val => val == valueKey);
                    if (index >= 0)
                    {
                        dataBveExValueComboBox.SelectedIndex = index;
                    }
                } else
                {
                    dataBveExValueComboBox.SelectedIndex = -1;
                }
                dataBveExValueComboBox.Enabled = true;
            } else
            {
                dataBveExValueComboBox.Enabled = false;
            }
        }

        private void DataBveExValueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataOutputListBox.SelectedItem != null && dataBveExValueComboBox.SelectedIndex != -1)
            {
                if (GetProfile().BveExValue == null)
                {
                    GetProfile().BveExValue = new Dictionary<string, string>();
                }
                if (!GetProfile().BveExValue.ContainsKey(dataOutputListBox.SelectedItem.ToString()))
                {
                    GetProfile().BveExValue.Add(dataOutputListBox.SelectedItem.ToString(), _BveExValueKey[dataBveExValueComboBox.SelectedIndex]);
                } else
                {
                    GetProfile().BveExValue[dataOutputListBox.SelectedItem.ToString()] = _BveExValueKey[dataBveExValueComboBox.SelectedIndex];
                }
            }
        }

        private void BveExHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/kusaanko/BveNCIBveExPlugin");
        }

        private void CheckUpdateButton_Click(object sender, EventArgs e)
        {
            if (!NumerousControllerInterface.CheckUpdates(true))
            {
                MessageBox.Show("更新はありません", "NumerousControllerInterface");
            } else
            {
                NumerousControllerInterface.SettingsInstance.IgnoreUpdate = 0;
            }
        }

        private void HoldToRepeatButtonCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var buttonIndex = GetButtonIndex((string)buttonList.SelectedItem);
            var profile = GetProfile();
            if (profile.KeyMap.ContainsKey(buttonIndex))
            {
                profile.HoldToRepeat[buttonIndex] = holdToRepeatButtonCheckBox.Checked;
            }
            timeToRepeatPressNumericUpDown.Enabled = holdToRepeatButtonCheckBox.Checked;
        }

        private void TimeToRepeatPressNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var buttonIndex = GetButtonIndex((string)buttonList.SelectedItem);
            var profile = GetProfile();
            if (profile.KeyMap.ContainsKey(buttonIndex))
            {
                var value = (float)timeToRepeatPressNumericUpDown.Value;
                if (value < 0.1f)
                {
                    value = 0.1f;
                } else if (value > 1.0f)
                {
                    value = 1.0f;
                }
                profile.HoldToRepeatTime[buttonIndex] = value;
            }
        }
    }
}
