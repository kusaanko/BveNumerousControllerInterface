namespace Kusaanko.Bvets.NumerousControllerInterface
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.controllerAssignmentLabel = new System.Windows.Forms.Label();
            this.controllerList = new System.Windows.Forms.ListBox();
            this.selectControllerLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.powerLabel = new System.Windows.Forms.Label();
            this.breakLabel = new System.Windows.Forms.Label();
            this.buttonLabel = new System.Windows.Forms.Label();
            this.removeButtonButton = new System.Windows.Forms.Button();
            this.addButtonButton = new System.Windows.Forms.Button();
            this.buttonFunctionLabel = new System.Windows.Forms.Label();
            this.buttonFunctionComboBox = new System.Windows.Forms.ComboBox();
            this.buttonsAssignmentLabel = new System.Windows.Forms.Label();
            this.buttonList = new System.Windows.Forms.ListBox();
            this.isEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.profileToUseLabel = new System.Windows.Forms.Label();
            this.profileComboBox = new System.Windows.Forms.ComboBox();
            this.profileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openProfileInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.newProfileButton = new System.Windows.Forms.Button();
            this.removeProfileButton = new System.Windows.Forms.Button();
            this.duplicateProfileButton = new System.Windows.Forms.Button();
            this.controllerTypeLabel = new System.Windows.Forms.Label();
            this.changeNameButton = new System.Windows.Forms.Button();
            this.settingsOfNumerousControllerInterfaceLabel = new System.Windows.Forms.Label();
            this.alertNoCountrollerFoundCheckBox = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pluginSettingButton = new System.Windows.Forms.Button();
            this.showLoadedPlugins = new System.Windows.Forms.Button();
            this.pluginConfigComboBox = new System.Windows.Forms.ComboBox();
            this.settingsTabControl = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.flexibleBreakModeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.flexiblePowerModeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.removeBreakButton = new System.Windows.Forms.Button();
            this.removePowerButton = new System.Windows.Forms.Button();
            this.isTwoHandleComboBox = new System.Windows.Forms.CheckBox();
            this.settingBreakButton = new System.Windows.Forms.Button();
            this.settingPowerButton = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.panel1.SuspendLayout();
            this.profileContextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.settingsTabControl.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // controllerAssignmentLabel
            // 
            resources.ApplyResources(this.controllerAssignmentLabel, "controllerAssignmentLabel");
            this.controllerAssignmentLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.controllerAssignmentLabel.Name = "controllerAssignmentLabel";
            // 
            // controllerList
            // 
            this.controllerList.FormattingEnabled = true;
            resources.ApplyResources(this.controllerList, "controllerList");
            this.controllerList.Name = "controllerList";
            this.controllerList.SelectedIndexChanged += new System.EventHandler(this.controllerList_SelectedIndexChanged);
            // 
            // selectControllerLabel
            // 
            resources.ApplyResources(this.selectControllerLabel, "selectControllerLabel");
            this.selectControllerLabel.Name = "selectControllerLabel";
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // powerLabel
            // 
            resources.ApplyResources(this.powerLabel, "powerLabel");
            this.powerLabel.Name = "powerLabel";
            // 
            // breakLabel
            // 
            resources.ApplyResources(this.breakLabel, "breakLabel");
            this.breakLabel.Name = "breakLabel";
            // 
            // buttonLabel
            // 
            resources.ApplyResources(this.buttonLabel, "buttonLabel");
            this.buttonLabel.Name = "buttonLabel";
            // 
            // removeButtonButton
            // 
            resources.ApplyResources(this.removeButtonButton, "removeButtonButton");
            this.removeButtonButton.Name = "removeButtonButton";
            this.removeButtonButton.UseVisualStyleBackColor = true;
            this.removeButtonButton.Click += new System.EventHandler(this.removeButtonButton_Click);
            // 
            // addButtonButton
            // 
            resources.ApplyResources(this.addButtonButton, "addButtonButton");
            this.addButtonButton.Name = "addButtonButton";
            this.addButtonButton.UseVisualStyleBackColor = true;
            this.addButtonButton.Click += new System.EventHandler(this.addButtonButton_Click);
            // 
            // buttonFunctionLabel
            // 
            resources.ApplyResources(this.buttonFunctionLabel, "buttonFunctionLabel");
            this.buttonFunctionLabel.Name = "buttonFunctionLabel";
            // 
            // buttonFunctionComboBox
            // 
            this.buttonFunctionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.buttonFunctionComboBox, "buttonFunctionComboBox");
            this.buttonFunctionComboBox.FormattingEnabled = true;
            this.buttonFunctionComboBox.Name = "buttonFunctionComboBox";
            this.buttonFunctionComboBox.SelectedIndexChanged += new System.EventHandler(this.buttonFunctionComboBox_SelectedIndexChanged);
            // 
            // buttonsAssignmentLabel
            // 
            resources.ApplyResources(this.buttonsAssignmentLabel, "buttonsAssignmentLabel");
            this.buttonsAssignmentLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.buttonsAssignmentLabel.Name = "buttonsAssignmentLabel";
            // 
            // buttonList
            // 
            this.buttonList.FormattingEnabled = true;
            resources.ApplyResources(this.buttonList, "buttonList");
            this.buttonList.Name = "buttonList";
            this.buttonList.Sorted = true;
            this.buttonList.SelectedIndexChanged += new System.EventHandler(this.buttonList_SelectedIndexChanged);
            // 
            // isEnabledCheckBox
            // 
            resources.ApplyResources(this.isEnabledCheckBox, "isEnabledCheckBox");
            this.isEnabledCheckBox.Name = "isEnabledCheckBox";
            this.isEnabledCheckBox.UseVisualStyleBackColor = true;
            this.isEnabledCheckBox.CheckedChanged += new System.EventHandler(this.isEnabledCheckBox_CheckedChanged);
            // 
            // profileToUseLabel
            // 
            resources.ApplyResources(this.profileToUseLabel, "profileToUseLabel");
            this.profileToUseLabel.Name = "profileToUseLabel";
            // 
            // profileComboBox
            // 
            this.profileComboBox.ContextMenuStrip = this.profileContextMenuStrip;
            this.profileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profileComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.profileComboBox, "profileComboBox");
            this.profileComboBox.Name = "profileComboBox";
            this.profileComboBox.SelectedIndexChanged += new System.EventHandler(this.profileComboBox_SelectedIndexChanged);
            // 
            // profileContextMenuStrip
            // 
            this.profileContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProfileInExplorer});
            this.profileContextMenuStrip.Name = "profileContextMenuStrip";
            resources.ApplyResources(this.profileContextMenuStrip, "profileContextMenuStrip");
            // 
            // openProfileInExplorer
            // 
            this.openProfileInExplorer.Name = "openProfileInExplorer";
            resources.ApplyResources(this.openProfileInExplorer, "openProfileInExplorer");
            this.openProfileInExplorer.Click += new System.EventHandler(this.openProfileInExplorer_Click);
            // 
            // newProfileButton
            // 
            resources.ApplyResources(this.newProfileButton, "newProfileButton");
            this.newProfileButton.Name = "newProfileButton";
            this.newProfileButton.UseVisualStyleBackColor = true;
            this.newProfileButton.Click += new System.EventHandler(this.newProfileButton_Click);
            // 
            // removeProfileButton
            // 
            resources.ApplyResources(this.removeProfileButton, "removeProfileButton");
            this.removeProfileButton.Name = "removeProfileButton";
            this.removeProfileButton.UseVisualStyleBackColor = true;
            this.removeProfileButton.Click += new System.EventHandler(this.removeProfileButton_Click);
            // 
            // duplicateProfileButton
            // 
            resources.ApplyResources(this.duplicateProfileButton, "duplicateProfileButton");
            this.duplicateProfileButton.Name = "duplicateProfileButton";
            this.duplicateProfileButton.UseVisualStyleBackColor = true;
            this.duplicateProfileButton.Click += new System.EventHandler(this.duplicateProfileButton_Click);
            // 
            // controllerTypeLabel
            // 
            resources.ApplyResources(this.controllerTypeLabel, "controllerTypeLabel");
            this.controllerTypeLabel.Name = "controllerTypeLabel";
            // 
            // changeNameButton
            // 
            resources.ApplyResources(this.changeNameButton, "changeNameButton");
            this.changeNameButton.Name = "changeNameButton";
            this.changeNameButton.UseVisualStyleBackColor = true;
            this.changeNameButton.Click += new System.EventHandler(this.changeNameButton_Click);
            // 
            // settingsOfNumerousControllerInterfaceLabel
            // 
            resources.ApplyResources(this.settingsOfNumerousControllerInterfaceLabel, "settingsOfNumerousControllerInterfaceLabel");
            this.settingsOfNumerousControllerInterfaceLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.settingsOfNumerousControllerInterfaceLabel.Name = "settingsOfNumerousControllerInterfaceLabel";
            // 
            // alertNoCountrollerFoundCheckBox
            // 
            resources.ApplyResources(this.alertNoCountrollerFoundCheckBox, "alertNoCountrollerFoundCheckBox");
            this.alertNoCountrollerFoundCheckBox.Checked = true;
            this.alertNoCountrollerFoundCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alertNoCountrollerFoundCheckBox.Name = "alertNoCountrollerFoundCheckBox";
            this.alertNoCountrollerFoundCheckBox.UseVisualStyleBackColor = true;
            this.alertNoCountrollerFoundCheckBox.CheckedChanged += new System.EventHandler(this.alertNoCountrollerFoundCheckBox_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkUpdatesCheckBox
            // 
            resources.ApplyResources(this.checkUpdatesCheckBox, "checkUpdatesCheckBox");
            this.checkUpdatesCheckBox.Checked = true;
            this.checkUpdatesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkUpdatesCheckBox.Name = "checkUpdatesCheckBox";
            this.checkUpdatesCheckBox.UseVisualStyleBackColor = true;
            this.checkUpdatesCheckBox.CheckedChanged += new System.EventHandler(this.checkUpdatesCheckBox_CheckedChanged);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.settingsTabControl);
            this.tabPage2.Controls.Add(this.controllerAssignmentLabel);
            this.tabPage2.Controls.Add(this.controllerList);
            this.tabPage2.Controls.Add(this.removeProfileButton);
            this.tabPage2.Controls.Add(this.selectControllerLabel);
            this.tabPage2.Controls.Add(this.changeNameButton);
            this.tabPage2.Controls.Add(this.statusLabel);
            this.tabPage2.Controls.Add(this.controllerTypeLabel);
            this.tabPage2.Controls.Add(this.powerLabel);
            this.tabPage2.Controls.Add(this.isEnabledCheckBox);
            this.tabPage2.Controls.Add(this.breakLabel);
            this.tabPage2.Controls.Add(this.duplicateProfileButton);
            this.tabPage2.Controls.Add(this.buttonLabel);
            this.tabPage2.Controls.Add(this.newProfileButton);
            this.tabPage2.Controls.Add(this.profileToUseLabel);
            this.tabPage2.Controls.Add(this.profileComboBox);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.alertNoCountrollerFoundCheckBox);
            this.tabPage1.Controls.Add(this.checkUpdatesCheckBox);
            this.tabPage1.Controls.Add(this.settingsOfNumerousControllerInterfaceLabel);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.pluginSettingButton);
            this.tabPage4.Controls.Add(this.showLoadedPlugins);
            this.tabPage4.Controls.Add(this.pluginConfigComboBox);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // pluginSettingButton
            // 
            resources.ApplyResources(this.pluginSettingButton, "pluginSettingButton");
            this.pluginSettingButton.Name = "pluginSettingButton";
            this.pluginSettingButton.UseVisualStyleBackColor = true;
            // 
            // showLoadedPlugins
            // 
            resources.ApplyResources(this.showLoadedPlugins, "showLoadedPlugins");
            this.showLoadedPlugins.Name = "showLoadedPlugins";
            this.showLoadedPlugins.UseVisualStyleBackColor = true;
            // 
            // pluginConfigComboBox
            // 
            this.pluginConfigComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pluginConfigComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.pluginConfigComboBox, "pluginConfigComboBox");
            this.pluginConfigComboBox.Name = "pluginConfigComboBox";
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Controls.Add(this.tabPage3);
            this.settingsTabControl.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.settingsTabControl, "settingsTabControl");
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.SelectedIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.flexibleBreakModeComboBox);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.flexiblePowerModeComboBox);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.removeBreakButton);
            this.tabPage3.Controls.Add(this.removePowerButton);
            this.tabPage3.Controls.Add(this.isTwoHandleComboBox);
            this.tabPage3.Controls.Add(this.settingBreakButton);
            this.tabPage3.Controls.Add(this.settingPowerButton);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.removeButtonButton);
            this.tabPage5.Controls.Add(this.buttonsAssignmentLabel);
            this.tabPage5.Controls.Add(this.addButtonButton);
            this.tabPage5.Controls.Add(this.buttonList);
            this.tabPage5.Controls.Add(this.buttonFunctionLabel);
            this.tabPage5.Controls.Add(this.buttonFunctionComboBox);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // flexibleBreakModeComboBox
            // 
            this.flexibleBreakModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flexibleBreakModeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.flexibleBreakModeComboBox, "flexibleBreakModeComboBox");
            this.flexibleBreakModeComboBox.Name = "flexibleBreakModeComboBox";
            this.toolTip1.SetToolTip(this.flexibleBreakModeComboBox, resources.GetString("flexibleBreakModeComboBox.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // flexiblePowerModeComboBox
            // 
            this.flexiblePowerModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flexiblePowerModeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.flexiblePowerModeComboBox, "flexiblePowerModeComboBox");
            this.flexiblePowerModeComboBox.Name = "flexiblePowerModeComboBox";
            this.toolTip1.SetToolTip(this.flexiblePowerModeComboBox, resources.GetString("flexiblePowerModeComboBox.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // removeBreakButton
            // 
            resources.ApplyResources(this.removeBreakButton, "removeBreakButton");
            this.removeBreakButton.Name = "removeBreakButton";
            this.removeBreakButton.UseVisualStyleBackColor = true;
            // 
            // removePowerButton
            // 
            resources.ApplyResources(this.removePowerButton, "removePowerButton");
            this.removePowerButton.Name = "removePowerButton";
            this.removePowerButton.UseVisualStyleBackColor = true;
            // 
            // isTwoHandleComboBox
            // 
            resources.ApplyResources(this.isTwoHandleComboBox, "isTwoHandleComboBox");
            this.isTwoHandleComboBox.Name = "isTwoHandleComboBox";
            this.isTwoHandleComboBox.UseVisualStyleBackColor = true;
            // 
            // settingBreakButton
            // 
            resources.ApplyResources(this.settingBreakButton, "settingBreakButton");
            this.settingBreakButton.Name = "settingBreakButton";
            this.settingBreakButton.UseVisualStyleBackColor = true;
            // 
            // settingPowerButton
            // 
            resources.ApplyResources(this.settingPowerButton, "settingPowerButton");
            this.settingPowerButton.Name = "settingPowerButton";
            this.settingPowerButton.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.profileContextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.settingsTabControl.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label controllerAssignmentLabel;
        private System.Windows.Forms.ListBox controllerList;
        private System.Windows.Forms.Label selectControllerLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label powerLabel;
        private System.Windows.Forms.Label breakLabel;
        private System.Windows.Forms.Label buttonLabel;
        private System.Windows.Forms.Button removeButtonButton;
        private System.Windows.Forms.Button addButtonButton;
        private System.Windows.Forms.Label buttonFunctionLabel;
        private System.Windows.Forms.ComboBox buttonFunctionComboBox;
        private System.Windows.Forms.Label buttonsAssignmentLabel;
        private System.Windows.Forms.ListBox buttonList;
        private System.Windows.Forms.CheckBox isEnabledCheckBox;
        private System.Windows.Forms.Label profileToUseLabel;
        private System.Windows.Forms.ComboBox profileComboBox;
        private System.Windows.Forms.Button newProfileButton;
        private System.Windows.Forms.Button removeProfileButton;
        private System.Windows.Forms.Button duplicateProfileButton;
        private System.Windows.Forms.Label controllerTypeLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button changeNameButton;
        private System.Windows.Forms.Label settingsOfNumerousControllerInterfaceLabel;
        private System.Windows.Forms.CheckBox alertNoCountrollerFoundCheckBox;
        private System.Windows.Forms.ContextMenuStrip profileContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openProfileInExplorer;
        private System.Windows.Forms.CheckBox checkUpdatesCheckBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button pluginSettingButton;
        private System.Windows.Forms.Button showLoadedPlugins;
        private System.Windows.Forms.ComboBox pluginConfigComboBox;
        private System.Windows.Forms.TabControl settingsTabControl;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox flexibleBreakModeComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox flexiblePowerModeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button removeBreakButton;
        private System.Windows.Forms.Button removePowerButton;
        private System.Windows.Forms.CheckBox isTwoHandleComboBox;
        private System.Windows.Forms.Button settingBreakButton;
        private System.Windows.Forms.Button settingPowerButton;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
    }
}