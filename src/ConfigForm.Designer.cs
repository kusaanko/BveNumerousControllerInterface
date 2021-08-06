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
            this.settingPowerButton = new System.Windows.Forms.Button();
            this.settingsGroup = new System.Windows.Forms.GroupBox();
            this.isFlexibleNotchCheckBox = new System.Windows.Forms.CheckBox();
            this.removeButtonButton = new System.Windows.Forms.Button();
            this.addButtonButton = new System.Windows.Forms.Button();
            this.buttonFunctionLabel = new System.Windows.Forms.Label();
            this.buttonFunctionComboBox = new System.Windows.Forms.ComboBox();
            this.buttonsAssignmentLabel = new System.Windows.Forms.Label();
            this.isMasterControllerCheckBox = new System.Windows.Forms.CheckBox();
            this.buttonList = new System.Windows.Forms.ListBox();
            this.isTwoHandleComboBox = new System.Windows.Forms.CheckBox();
            this.settingBreakButton = new System.Windows.Forms.Button();
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
            this.panel1.SuspendLayout();
            this.settingsGroup.SuspendLayout();
            this.profileContextMenuStrip.SuspendLayout();
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
            // settingPowerButton
            // 
            resources.ApplyResources(this.settingPowerButton, "settingPowerButton");
            this.settingPowerButton.Name = "settingPowerButton";
            this.settingPowerButton.UseVisualStyleBackColor = true;
            this.settingPowerButton.Click += new System.EventHandler(this.settingPowerButton_Click);
            // 
            // settingsGroup
            // 
            this.settingsGroup.Controls.Add(this.isFlexibleNotchCheckBox);
            this.settingsGroup.Controls.Add(this.removeButtonButton);
            this.settingsGroup.Controls.Add(this.addButtonButton);
            this.settingsGroup.Controls.Add(this.buttonFunctionLabel);
            this.settingsGroup.Controls.Add(this.buttonFunctionComboBox);
            this.settingsGroup.Controls.Add(this.buttonsAssignmentLabel);
            this.settingsGroup.Controls.Add(this.isMasterControllerCheckBox);
            this.settingsGroup.Controls.Add(this.buttonList);
            this.settingsGroup.Controls.Add(this.isTwoHandleComboBox);
            this.settingsGroup.Controls.Add(this.settingBreakButton);
            this.settingsGroup.Controls.Add(this.settingPowerButton);
            resources.ApplyResources(this.settingsGroup, "settingsGroup");
            this.settingsGroup.Name = "settingsGroup";
            this.settingsGroup.TabStop = false;
            // 
            // isFlexibleNotchCheckBox
            // 
            resources.ApplyResources(this.isFlexibleNotchCheckBox, "isFlexibleNotchCheckBox");
            this.isFlexibleNotchCheckBox.Name = "isFlexibleNotchCheckBox";
            this.isFlexibleNotchCheckBox.UseVisualStyleBackColor = true;
            this.isFlexibleNotchCheckBox.CheckedChanged += new System.EventHandler(this.isFlexibleNotchCheckBox_CheckedChanged);
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
            // isMasterControllerCheckBox
            // 
            resources.ApplyResources(this.isMasterControllerCheckBox, "isMasterControllerCheckBox");
            this.isMasterControllerCheckBox.Name = "isMasterControllerCheckBox";
            this.isMasterControllerCheckBox.UseVisualStyleBackColor = true;
            this.isMasterControllerCheckBox.CheckedChanged += new System.EventHandler(this.isMasterControllerCheckBox_CheckedChanged);
            // 
            // buttonList
            // 
            this.buttonList.FormattingEnabled = true;
            resources.ApplyResources(this.buttonList, "buttonList");
            this.buttonList.Name = "buttonList";
            this.buttonList.Sorted = true;
            this.buttonList.SelectedIndexChanged += new System.EventHandler(this.buttonList_SelectedIndexChanged);
            // 
            // isTwoHandleComboBox
            // 
            resources.ApplyResources(this.isTwoHandleComboBox, "isTwoHandleComboBox");
            this.isTwoHandleComboBox.Name = "isTwoHandleComboBox";
            this.isTwoHandleComboBox.UseVisualStyleBackColor = true;
            this.isTwoHandleComboBox.CheckedChanged += new System.EventHandler(this.isTwoHandleComboBox_CheckedChanged);
            // 
            // settingBreakButton
            // 
            resources.ApplyResources(this.settingBreakButton, "settingBreakButton");
            this.settingBreakButton.Name = "settingBreakButton";
            this.settingBreakButton.UseVisualStyleBackColor = true;
            this.settingBreakButton.Click += new System.EventHandler(this.settingBreakButton_Click);
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
            // ConfigForm
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.alertNoCountrollerFoundCheckBox);
            this.Controls.Add(this.settingsOfNumerousControllerInterfaceLabel);
            this.Controls.Add(this.changeNameButton);
            this.Controls.Add(this.controllerTypeLabel);
            this.Controls.Add(this.isEnabledCheckBox);
            this.Controls.Add(this.duplicateProfileButton);
            this.Controls.Add(this.removeProfileButton);
            this.Controls.Add(this.newProfileButton);
            this.Controls.Add(this.profileComboBox);
            this.Controls.Add(this.profileToUseLabel);
            this.Controls.Add(this.settingsGroup);
            this.Controls.Add(this.buttonLabel);
            this.Controls.Add(this.breakLabel);
            this.Controls.Add(this.powerLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.selectControllerLabel);
            this.Controls.Add(this.controllerList);
            this.Controls.Add(this.controllerAssignmentLabel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.settingsGroup.ResumeLayout(false);
            this.settingsGroup.PerformLayout();
            this.profileContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button settingPowerButton;
        private System.Windows.Forms.GroupBox settingsGroup;
        private System.Windows.Forms.Button removeButtonButton;
        private System.Windows.Forms.Button addButtonButton;
        private System.Windows.Forms.Label buttonFunctionLabel;
        private System.Windows.Forms.ComboBox buttonFunctionComboBox;
        private System.Windows.Forms.Label buttonsAssignmentLabel;
        private System.Windows.Forms.CheckBox isMasterControllerCheckBox;
        private System.Windows.Forms.ListBox buttonList;
        private System.Windows.Forms.CheckBox isTwoHandleComboBox;
        private System.Windows.Forms.Button settingBreakButton;
        private System.Windows.Forms.CheckBox isEnabledCheckBox;
        private System.Windows.Forms.Label profileToUseLabel;
        private System.Windows.Forms.ComboBox profileComboBox;
        private System.Windows.Forms.Button newProfileButton;
        private System.Windows.Forms.Button removeProfileButton;
        private System.Windows.Forms.Button duplicateProfileButton;
        private System.Windows.Forms.Label controllerTypeLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button changeNameButton;
        private System.Windows.Forms.CheckBox isFlexibleNotchCheckBox;
        private System.Windows.Forms.Label settingsOfNumerousControllerInterfaceLabel;
        private System.Windows.Forms.CheckBox alertNoCountrollerFoundCheckBox;
        private System.Windows.Forms.ContextMenuStrip profileContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openProfileInExplorer;
    }
}