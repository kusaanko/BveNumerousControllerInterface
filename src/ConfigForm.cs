using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class ConfigForm : Form
    {
        private string[] reverserSwitchKeyTexts = { "リバーサー後退", "リバーサー中", "リバーサー前進" };
        private string[] cabSwitchKeyTexts = { "電笛", "空笛", "低速運転", "乗降促進" };
        private string[] gameControlKeyTexts = { "視点を上に移動", "視点を下に移動", "視点を左に移動", "視点を右に移動", 
            "視点をデフォルトに戻す", "視界をズームイン", "視界をズームアウト", "視点を切り替える", "時刻表示切り替え", "シナリオ再読み込み",
            "列車速度の変更", "早送り", "一時停止"};
        private string[] atsKeyTexts = { "ATS0(ATS確認)(S)(Space)", "ATS1(警報維持)(A1)(Insert)", "ATS2(EBリセット)(A2)(Delete)", 
            "ATS3(復帰)(B1)(Home)", "ATS4(ATS-Pブレーキ解放)(B2)(End)", "ATS5(ATSに切り替え)(C1)(PageUp)", "ATS6(ATCに切り替え)(C2)(Next/PageDown)",
            "ATS7(D)(2)", "ATS8(E)(3)", "ATS9(F)(4)", "ATS10(G)(5)", "ATS11(H)(6)", "ATS12(I)(7)", "ATS13(J)(8)", "ATS14(K)(9)", "ATS15(L)(0)"};
        private string[] notchControlTexts = { "非常にする", "全て切にする", 
            "ブレーキ切", "ブレーキ上げ", "ブレーキ下げ", "力行切", "力行上げ", "力行下げ", "ノッチ上げ", "ノッチ下げ"};
        private List<int[]> keyCodeTable = new List<int[]>();
        private Dictionary<string, IController> controllers = new Dictionary<string, IController>();
        private System.ComponentModel.ComponentResourceManager resources;
        public ControllerSetupForm controllerSetupForm;

        public ConfigForm()
        {
            InitializeComponent();
            this.resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            for (int i = 0; i < reverserSwitchKeyTexts.Length; i++)
            {
                addKey(reverserSwitchKeyTexts[i]);
                keyCodeTable.Add(new int[2] { 0, i });
            }
            for (int i = 0; i < cabSwitchKeyTexts.Length; i++)
            {
                addKey(cabSwitchKeyTexts[i]);
                keyCodeTable.Add(new int[2] { -1, i });
            }
            for (int i = 0; i < atsKeyTexts.Length; i++)
            {
                addKey(atsKeyTexts[i]);
                keyCodeTable.Add(new int[2] { -2, i });
            }
            for (int i = 0; i < gameControlKeyTexts.Length; i++)
            {
                addKey(gameControlKeyTexts[i]);
                keyCodeTable.Add(new int[2] { -3, i });
            }
            for (int i = 0; i < notchControlTexts.Length; i++)
            {
                addKey(notchControlTexts[i]);
                keyCodeTable.Add(new int[2] { 99, i });
            }
            ControllerProfile.GetAllControllers();
            updateControllers();
            timer1.Start();
            alertNoCountrollerFoundCheckBox.Checked = NumerousControllerInterface.settings.AlertNoControllerFound;
        }

        public void updateControllers()
        {
            timer1.Stop();
            controllerList.Items.Clear();
            controllers.Clear();
            foreach (IController controller in ControllerProfile.controllers)
            {
                controllerList.Items.Add(controller.GetName());
                controllers.Add(controller.GetName(), controller);
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
                    if (assign[1] >= 0 && assign[1] < reverserSwitchKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1];
                    }
                    break;
                case -1:
                    if (assign[1] >= 0 && assign[1] < cabSwitchKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + reverserSwitchKeyTexts.Length;
                    }
                    break;
                case -2:
                    if (assign[1] >= 0 && assign[1] < atsKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + reverserSwitchKeyTexts.Length + cabSwitchKeyTexts.Length;
                    }
                    break;
                case -3:
                    if (assign[1] >= 0 && assign[1] < gameControlKeyTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + reverserSwitchKeyTexts.Length + cabSwitchKeyTexts.Length + atsKeyTexts.Length;
                    }
                    break;
                case 99:
                    if (assign[1] >= 0 && assign[1] < notchControlTexts.Length)
                    {
                        list.SelectedIndex = assign[1] + reverserSwitchKeyTexts.Length + cabSwitchKeyTexts.Length + atsKeyTexts.Length + gameControlKeyTexts.Length;
                    }
                    break;
            }
        }

        private void updateProfile()
        {
            profileComboBox.Items.Clear();
            foreach (string name in NumerousControllerInterface.settings.Profiles.Keys)
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
            ControllerProfile profile = NumerousControllerInterface.settings.Profiles[profileComboBox.Text];
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
            isEnabledCheckBox.Checked = NumerousControllerInterface.settings.GetIsEnabled(controllerList.Text);
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
            if (!NumerousControllerInterface.settings.ProfileMap.ContainsKey(controller))
            {
                if (!NumerousControllerInterface.settings.Profiles.ContainsKey("無名のプロファイル"))
                {
                    ControllerProfile profile = new ControllerProfile();
                    NumerousControllerInterface.settings.Profiles.Add("無名のプロファイル", profile);
                }
                NumerousControllerInterface.settings.ProfileMap.Add(controller, "無名のプロファイル");
                updateProfile();
            }
            selectProfile(NumerousControllerInterface.settings.ProfileMap[controller]);
            loadFromProfile();
            loadControllerEnabled();
            controllerTypeLabel.Text = this.resources.GetString("controllerTypeLabel.Text") + GetController().GetControllerType();
        }

        private ControllerProfile GetProfile()
        {
            if (!NumerousControllerInterface.settings.Profiles.ContainsKey(profileComboBox.Text)) return null;
            return NumerousControllerInterface.settings.Profiles[profileComboBox.Text];
        }

        private IController GetController()
        {
            if (!controllers.ContainsKey(controllerList.Text)) return null;
            return controllers[controllerList.Text];
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
                GetProfile().KeyMap[(int)buttonList.SelectedItem] = keyCodeTable[buttonFunctionComboBox.SelectedIndex];
            }
        }

        private void settingPowerButton_Click(object sender, EventArgs e)
        {
            using (controllerSetupForm = new ControllerSetupForm(
                controllers[controllerList.Text],
                GetProfile(),
                true))
            {
                controllerSetupForm.ShowDialog(this);
            }
        }

        private void settingBreakButton_Click(object sender, EventArgs e)
        {
            using (controllerSetupForm = new ControllerSetupForm(
                controllers[controllerList.Text], 
                GetProfile(), 
                false))
            {
                controllerSetupForm.ShowDialog(this);
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
            if(NumerousControllerInterface.settings.ProfileMap.ContainsKey(controllerList.Text))
            {
                NumerousControllerInterface.settings.ProfileMap[controllerList.Text] = profileComboBox.Text;
            }
            else
            {
                NumerousControllerInterface.settings.ProfileMap.Add(controllerList.Text, profileComboBox.Text);
            }
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            string name = "無名のプロファイル";
            int i = 1;
            while(true)
            {
                if (NumerousControllerInterface.settings.Profiles.ContainsKey(name + i))
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
                if(NumerousControllerInterface.settings.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }else
                {
                    NumerousControllerInterface.settings.Profiles.Add(s, new ControllerProfile());
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
                if (NumerousControllerInterface.settings.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    ControllerProfile profile = GetProfile();
                    if (!NumerousControllerInterface.settings.removeProfilesList.Contains(oldName)) NumerousControllerInterface.settings.removeProfilesList.Add(oldName);
                    NumerousControllerInterface.settings.Profiles.Remove(oldName);
                    NumerousControllerInterface.settings.Profiles.Add(s, profile);
                    List<string> changeNames = new List<string>();
                    foreach (string key in NumerousControllerInterface.settings.ProfileMap.Keys)
                    {
                        if (NumerousControllerInterface.settings.ProfileMap[key].Equals(oldName))
                        {
                            changeNames.Add(key);
                        }
                    }
                    foreach (string key in changeNames)
                    {
                        NumerousControllerInterface.settings.ProfileMap[key] = s;
                    }
                    NumerousControllerInterface.settings.removeProfilesList.Remove(s);
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
                if (NumerousControllerInterface.settings.Profiles.ContainsKey(s))
                {
                    MessageBox.Show(s + "はすでに存在します。別の名前にして下さい。");
                    return false;
                }
                else
                {
                    NumerousControllerInterface.settings.Profiles.Add(s, GetProfile().Clone());
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
                NumerousControllerInterface.settings.Profiles.Remove(name);
                List<string> removeNames = new List<string>();
                foreach (string key in NumerousControllerInterface.settings.ProfileMap.Keys)
                {
                    if (NumerousControllerInterface.settings.ProfileMap[key].Equals(name))
                    {
                        removeNames.Add(key);
                    }
                }
                foreach (string key in removeNames)
                {
                    NumerousControllerInterface.settings.ProfileMap.Remove(key);
                }
                if (!NumerousControllerInterface.settings.removeProfilesList.Contains(name)) NumerousControllerInterface.settings.removeProfilesList.Add(name);
                updateProfile();
                profileComboBox.SelectedIndex = 0;
            }
        }

        private void isEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!NumerousControllerInterface.settings.IsEnabled.ContainsKey(controllerList.Text))
            {
                NumerousControllerInterface.settings.IsEnabled.Add(controllerList.Text, isEnabledCheckBox.Checked);
            }
            else
            {
                NumerousControllerInterface.settings.IsEnabled[controllerList.Text] = isEnabledCheckBox.Checked;
            }
        }

        private void isFlexibleNotchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GetProfile().IsFlexibleNotch = isFlexibleNotchCheckBox.Checked;
        }

        private void alertNoCountrollerFoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            NumerousControllerInterface.settings.AlertNoControllerFound = alertNoCountrollerFoundCheckBox.Checked;
        }
    }
}
