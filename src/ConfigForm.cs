using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ConfigForm : Form
    {
        private string[] _reverserSwitchKeyTexts = { "リバーサー後退", "リバーサー中", "リバーサー前進" };
        private string[] _cabSwitchKeyTexts = { "電笛", "空笛", "低速運転", "乗降促進" };
        private string[] _gameControlKeyTexts = { "視点を上に移動", "視点を下に移動", "視点を左に移動", "視点を右に移動", 
            "視点をデフォルトに戻す", "視界をズームイン", "視界をズームアウト", "視点を切り替える", "時刻表示切り替え", "シナリオ再読み込み",
            "列車速度の変更", "早送り", "一時停止"};
        private string[] _atsKeyTexts = { "ATS0(ATS確認)(S)(Space)", "ATS1(警報維持)(A1)(Insert)", "ATS2(EBリセット)(A2)(Delete)", 
            "ATS3(復帰)(B1)(Home)", "ATS4(ATS-Pブレーキ解放)(B2)(End)", "ATS5(ATSに切り替え)(C1)(PageUp)", "ATS6(ATCに切り替え)(C2)(Next/PageDown)",
            "ATS7(D)(2)", "ATS8(E)(3)", "ATS9(F)(4)", "ATS10(G)(5)", "ATS11(H)(6)", "ATS12(I)(7)", "ATS13(J)(8)", "ATS14(K)(9)", "ATS15(L)(0)"};
        private string[] _notchControlTexts = { "非常にする", "全て切にする", 
            "ブレーキ切", "ブレーキ上げ", "ブレーキ下げ", "力行切", "力行上げ", "力行下げ", "ノッチ上げ", "ノッチ下げ"};
        private List<int[]> _keyCodeTable = new List<int[]>();
        private Dictionary<string, IController> _controllers = new Dictionary<string, IController>();
        private System.ComponentModel.ComponentResourceManager resources;
        public ControllerSetupForm ControllerSetupForm;

        public ConfigForm()
        {
            InitializeComponent();
            this.resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            for (int i = 0; i < _reverserSwitchKeyTexts.Length; i++)
            {
                addKey(_reverserSwitchKeyTexts[i]);
                _keyCodeTable.Add(new int[2] { 0, i });
            }
            for (int i = 0; i < _cabSwitchKeyTexts.Length; i++)
            {
                addKey(_cabSwitchKeyTexts[i]);
                _keyCodeTable.Add(new int[2] { -1, i });
            }
            for (int i = 0; i < _atsKeyTexts.Length; i++)
            {
                addKey(_atsKeyTexts[i]);
                _keyCodeTable.Add(new int[2] { -2, i });
            }
            for (int i = 0; i < _gameControlKeyTexts.Length; i++)
            {
                addKey(_gameControlKeyTexts[i]);
                _keyCodeTable.Add(new int[2] { -3, i });
            }
            for (int i = 0; i < _notchControlTexts.Length; i++)
            {
                addKey(_notchControlTexts[i]);
                _keyCodeTable.Add(new int[2] { 99, i });
            }
            ControllerProfile.GetAllControllers();
            updateControllers();
            timer1.Start();
            alertNoCountrollerFoundCheckBox.Checked = NumerousControllerInterface.SettingsInstance.AlertNoControllerFound;
        }

        public void updateControllers()
        {
            timer1.Stop();
            controllerList.Items.Clear();
            _controllers.Clear();
            foreach (IController controller in ControllerProfile.controllers)
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
            settingsGroup.Enabled = enabled;
        }

        private void addKey(string s)
        {
            buttonFunctionComboBox.Items.Add(s);
        }

        private void selectDropDownList(ComboBox list, int[] assign)
        {
            switch (assign[0])
            {
                case 0:
                    if (assign[1] >= 0 && assign[1] < _reverserSwitchKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1];
                    }
                    break;
                case -1:
                    if (assign[1] >= 0 && assign[1] < _cabSwitchKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + _reverserSwitchKeyTexts.Length;
                    }
                    break;
                case -2:
                    if (assign[1] >= 0 && assign[1] < _atsKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + _reverserSwitchKeyTexts.Length + _cabSwitchKeyTexts.Length;
                    }
                    break;
                case -3:
                    if (assign[1] >= 0 && assign[1] < _gameControlKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + _reverserSwitchKeyTexts.Length + _cabSwitchKeyTexts.Length + _atsKeyTexts.Length;
                    }
                    break;
                case 99:
                    if (assign[1] >= 0 && assign[1] < _notchControlTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + _reverserSwitchKeyTexts.Length + _cabSwitchKeyTexts.Length + _atsKeyTexts.Length + _gameControlKeyTexts.Length;
                    }
                    break;
            }
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
            isMasterControllerCheckBox.Checked = profile.IsMasterController;
            isTwoHandleComboBox.Checked = profile.IsTwoHandle;
            isFlexibleNotchCheckBox.Checked = profile.IsFlexibleNotch;
            buttonList.Items.Clear();
            foreach (int i in profile.KeyMap.Keys)
            {
                buttonList.Items.Add(i);
            }
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
                    ControllerProfile profile = new ControllerProfile();
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

        private IController GetController()
        {
            if (!_controllers.ContainsKey(controllerList.Text)) return null;
            return _controllers[controllerList.Text];
        }

        private void isMasterControllerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsMasterController = isMasterControllerCheckBox.Checked;
        }

        private void isTwoHandleComboBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsTwoHandle = isTwoHandleComboBox.Checked;
        }

        private void addButtonButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            IController controller = GetController();
            ControllerProfile profile = GetProfile();
            if (controller.Read().IsFailure) return;
            addButtonButton.Text = "ボタンを押す...";
            List<int> buttons = profile.GetButtons(controller);
            Thread thread = new Thread(new ThreadStart(() => {
                int buttonIndex = -1;
                while (true)
                {
                    if (controller.Read().IsFailure) break;
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
                        if (buttonList.Items.Contains(buttonIndex))
                        {
                            buttonList.SelectedItem = buttonIndex;
                        }
                        else
                        {
                            buttonList.Items.Add(buttonIndex);
                            GetProfile().KeyMap.Add(buttonIndex, new int[] { -2, 0 });
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
            selectDropDownList(buttonFunctionComboBox, GetProfile().KeyMap[(int)buttonList.SelectedItem]);
        }

        private void removeButtonButton_Click(object sender, EventArgs e)
        {
            if (buttonList.SelectedIndex >= 0)
            {
                GetProfile().KeyMap.Remove((int)buttonList.SelectedItem);
                loadFromProfile();
            }
        }

        private void buttonFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buttonList.SelectedItem == null) return;
            if (GetProfile().KeyMap.ContainsKey((int)buttonList.SelectedItem))
            {
                GetProfile().KeyMap[(int)buttonList.SelectedItem] = _keyCodeTable[buttonFunctionComboBox.SelectedIndex];
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
            IController controller = GetController();
            ControllerProfile profile = GetProfile();
            if (controller == null || profile == null) return;
            controller.Read();
            breakLabel.Text = this.resources.GetString("breakLabel.Text") + profile.GetBreak(controller, 10);
            powerLabel.Text = this.resources.GetString("powerLabel.Text") + profile.GetPower(controller, 6);
            buttonLabel.Text = this.resources.GetString("buttonLabel.Text");
            foreach (int i in profile.GetButtons(controller))
            {
                buttonLabel.Text += i + " ";
                if (buttonList.Items.Contains(i))
                {
                    buttonList.SelectedItem = i;
                    break;
                }
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
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
                    NumerousControllerInterface.SettingsInstance.Profiles.Add(s, new ControllerProfile());
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
                    NumerousControllerInterface.SettingsInstance.Profiles.Add(s, GetProfile().Clone());
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

        private void isFlexibleNotchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsFlexibleNotch = isFlexibleNotchCheckBox.Checked;
        }

        private void alertNoCountrollerFoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.SettingsInstance.AlertNoControllerFound = alertNoCountrollerFoundCheckBox.Checked;
        }
    }
}
