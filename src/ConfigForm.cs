using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System.IO;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ConfigForm : Form
    {
        private Dictionary<string, NCIController> _controllers = new Dictionary<string, NCIController>();
        private List<string> _buttonFeatureIdIndex = new List<string>();
        private System.ComponentModel.ComponentResourceManager resources;
        public ControllerSetupForm ControllerSetupForm;

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
            flexibleBreakModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.None]);
            flexibleBreakModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.EBFixed]);
            flexibleBreakModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.FlexibleWithoutEB]);
            flexibleBreakModeComboBox.Items.Add(ControllerProfile.FlexibleNotchModeStrings[(int)FlexibleNotchMode.Flexible]);
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
            updateControllers();
            timer1.Start();
            alertNoCountrollerFoundCheckBox.Checked = NumerousControllerInterface.SettingsInstance.AlertNoControllerFound;
            checkUpdatesCheckBox.Checked = NumerousControllerInterface.SettingsInstance.CheckUpdates;
        }

        public void updateControllers()
        {
            timer1.Stop();
            controllerList.Items.Clear();
            _controllers.Clear();
            foreach (NCIController controller in ControllerProfile.controllers)
            {
                controllerList.Items.Add(controller.GetName());
                _controllers.Add(controller.GetName(), controller);
            }
            updateProfile();
            setEnabled(false);
            timer1.Start();
        }

        private void setEnabled(bool enabled)
        {
            isEnabledCheckBox.Enabled = enabled;
            profileComboBox.Enabled = enabled;
            newProfileButton.Enabled = enabled;
            changeNameButton.Enabled = enabled;
            duplicateProfileButton.Enabled = enabled;
            removeProfileButton.Enabled = enabled;
            settingsTabControl.Enabled = enabled;
        }

        private void selectDropDownList(ComboBox list, ButtonFeature assign)
        {
            list.SelectedIndex = _buttonFeatureIdIndex.IndexOf(assign.Id);
        }

        private void updateProfile()
        {
            profileComboBox.Items.Clear();
            foreach (string name in NumerousControllerInterface.SettingsInstance.Profiles.Keys)
            {
                profileComboBox.Items.Add(name);
            }
        }

        private void selectProfile(string profile)
        {
            for (int i = 0;i < profileComboBox.Items.Count;i++)
            {
                if (profile.Equals(profileComboBox.Items[i]))
                {
                    profileComboBox.SelectedIndex = i;
                }
            }
            loadFromProfile();
            setEnabled(true);
        }

        private void loadFromProfile()
        {
            ControllerProfile profile = NumerousControllerInterface.SettingsInstance.Profiles[profileComboBox.Text];
            isTwoHandleComboBox.Checked = profile.IsTwoHandle;
            flexiblePowerModeComboBox.SelectedItem = ControllerProfile.FlexibleNotchModeStrings[(int)profile.FlexiblePower];
            flexibleBreakModeComboBox.SelectedItem = ControllerProfile.FlexibleNotchModeStrings[(int)profile.FlexibleBreak];
            buttonList.Items.Clear();
            foreach (int i in profile.KeyMap.Keys)
            {
                buttonList.Items.Add(GetButtonName(i));
            }
            settingPowerButton.Enabled = GetController().GetPowerCount() == 0;
            settingBreakButton.Enabled = GetController().GetBreakCount() == 0;
            removePowerButton.Enabled = GetController().GetPowerCount() == 0;
            removeBreakButton.Enabled = GetController().GetBreakCount() == 0;
        }

        private void loadControllerEnabled()
        {
            isEnabledCheckBox.Checked = NumerousControllerInterface.SettingsInstance.GetIsEnabled(controllerList.Text);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void controllerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string controller = controllerList.Text;
            if (controller == null || controller.Equals("")) return;
            if (!NumerousControllerInterface.SettingsInstance.ProfileMap.ContainsKey(controller))
            {
                if (!NumerousControllerInterface.SettingsInstance.Profiles.ContainsKey("無名のプロファイル"))
                {
                    ControllerProfile profile = new ControllerProfile("無名のプロファイル");
                    NumerousControllerInterface.SettingsInstance.Profiles.Add("無名のプロファイル", profile);
                }
                NumerousControllerInterface.SettingsInstance.ProfileMap.Add(controller, "無名のプロファイル");
                updateProfile();
            }
            selectProfile(NumerousControllerInterface.SettingsInstance.ProfileMap[controller]);
            loadFromProfile();
            loadControllerEnabled();
            controllerTypeLabel.Text = this.resources.GetString("controllerTypeLabel.Text") + GetController().GetControllerType();
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

        private void isTwoHandleComboBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsTwoHandle = isTwoHandleComboBox.Checked;
        }

        private void addButtonButton_Click(object sender, EventArgs e)
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

        private void buttonList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(buttonList.SelectedIndex < 0)
            {
                buttonFunctionComboBox.Enabled = false;
                return;
            }
            buttonFunctionComboBox.Enabled = true;
            selectDropDownList(buttonFunctionComboBox, GetProfile().KeyMap[GetButtonIndex((string)buttonList.SelectedItem)]);
        }

        private void removeButtonButton_Click(object sender, EventArgs e)
        {
            if (buttonList.SelectedIndex >= 0)
            {
                GetProfile().KeyMap.Remove(GetButtonIndex((string)buttonList.SelectedItem));
                loadFromProfile();
            }
        }

        private void buttonFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buttonList.SelectedItem == null) return;
            if (GetProfile().KeyMap.ContainsKey(GetButtonIndex((string)buttonList.SelectedItem)))
            {
                GetProfile().KeyMap[GetButtonIndex((string)buttonList.SelectedItem)] = ButtonFeature.Features[_buttonFeatureIdIndex[buttonFunctionComboBox.SelectedIndex]];
            }
        }

        private void settingPowerButton_Click(object sender, EventArgs e)
        {
            using (ControllerSetupForm = new ControllerSetupForm(
                _controllers[controllerList.Text],
                GetProfile(),
                true))
            {
                ControllerSetupForm.ShowDialog(this);
            }
        }

        private void settingBreakButton_Click(object sender, EventArgs e)
        {
            using (ControllerSetupForm = new ControllerSetupForm(
                _controllers[controllerList.Text], 
                GetProfile(), 
                false))
            {
                ControllerSetupForm.ShowDialog(this);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NCIController controller = GetController();
            ControllerProfile profile = GetProfile();
            if (controller == null || profile == null) return;
            breakLabel.Text = this.resources.GetString("breakLabel.Text") + profile.GetBreak(controller, profile.GetBreakCount(controller) + 1);
            powerLabel.Text = this.resources.GetString("powerLabel.Text") + profile.GetPower(controller, profile.GetPowerCount(controller) + 1);
            buttonLabel.Text = this.resources.GetString("buttonLabel.Text");
            foreach (int i in profile.GetButtons(controller))
            {
                buttonLabel.Text += GetButtonName(i) + " ";
                if (buttonList.Items.Contains(GetButtonName(i)))
                {
                    buttonList.SelectedItem = GetButtonName(i);
                    break;
                }
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            NumerousControllerInterface.IsMasterControllerUpdateRequested = true;
        }

        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadFromProfile();
            if(NumerousControllerInterface.SettingsInstance.ProfileMap.ContainsKey(controllerList.Text))
            {
                NumerousControllerInterface.SettingsInstance.ProfileMap[controllerList.Text] = profileComboBox.Text;
            }
            else
            {
                NumerousControllerInterface.SettingsInstance.ProfileMap.Add(controllerList.Text, profileComboBox.Text);
            }
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            string name = "無名のプロファイル";
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
                updateProfile();
                selectProfile(s);
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void changeNameButton_Click(object sender, EventArgs e)
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
                    updateProfile();
                    selectProfile(s);
                }
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }

        }

        private void duplicateProfileButton_Click(object sender, EventArgs e)
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
                    updateProfile();
                    selectProfile(s);
                }
                return true;
            }))
            {
                dialog.ShowDialog(this);
            }
        }

        private void removeProfileButton_Click(object sender, EventArgs e)
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
                updateProfile();
                profileComboBox.SelectedIndex = 0;
            }
        }

        private void isEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
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

        private void alertNoCountrollerFoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.SettingsInstance.AlertNoControllerFound = alertNoCountrollerFoundCheckBox.Checked;
        }

        private void openProfileInExplorer_Click(object sender, EventArgs e)
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

        private void removePowerButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("本当に力行を削除しますか？", "NumerousControllerInput", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetProfile().ResetPower();
            }
        }

        private void removeBreakButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("本当に制動を削除しますか？", "NumerousControllerInput", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetProfile().ResetBreak();
            }
        }

        private void checkUpdatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.SettingsInstance.CheckUpdates = checkUpdatesCheckBox.Checked;
        }

        private void flexiblePowerModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
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

        private void flexibleBreakModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(int key in ControllerProfile.FlexibleNotchModeStrings.Keys)
            {
                string value = ControllerProfile.FlexibleNotchModeStrings[key];
                if(value.Equals(flexibleBreakModeComboBox.Text))
                {
                    GetProfile().FlexibleBreak = (FlexibleNotchMode)key;
                    break;
                }
            }
        }

        private void showLoadedPlugins_Click(object sender, EventArgs e)
        {
            string str = "";
            foreach (NumerousControllerPlugin plugin in NumerousControllerInterface.Plugins)
            {
                str += plugin.GetName() + " " + plugin.GetVersion() + "\n";
            }
            MessageBox.Show(str, "NumerousControllerInterface");
        }

        private void pluginSettingButton_Click(object sender, EventArgs e)
        {
            if (pluginConfigComboBox.SelectedIndex < 0) return;
            NumerousControllerInterface.Plugins[pluginConfigComboBox.SelectedIndex].ShowConfigForm();
        }
    }
}
