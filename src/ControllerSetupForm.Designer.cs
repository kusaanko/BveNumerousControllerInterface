
namespace Kusaanko.Bvets.NumerousControllerInterface
{
    partial class ControllerSetupForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.cacelButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.countLabel = new System.Windows.Forms.Label();
            this.nextButton = new System.Windows.Forms.Button();
            this.useAxisButton = new System.Windows.Forms.Button();
            this.useButtonAxisButton = new System.Windows.Forms.Button();
            this.InaccuracyModeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(128, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(461, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "コントローラーのセットアップ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.Location = new System.Drawing.Point(20, 208);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(285, 32);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "どちらの方法を使いますか？";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cacelButton
            // 
            this.cacelButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cacelButton.Location = new System.Drawing.Point(20, 322);
            this.cacelButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cacelButton.Name = "cacelButton";
            this.cacelButton.Size = new System.Drawing.Size(192, 45);
            this.cacelButton.TabIndex = 2;
            this.cacelButton.Text = "キャンセル";
            this.cacelButton.UseVisualStyleBackColor = true;
            this.cacelButton.Click += new System.EventHandler(this.cacelButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // countLabel
            // 
            this.countLabel.AutoSize = true;
            this.countLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countLabel.Location = new System.Drawing.Point(300, 150);
            this.countLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(27, 32);
            this.countLabel.TabIndex = 3;
            this.countLabel.Text = "0";
            // 
            // nextButton
            // 
            this.nextButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextButton.Location = new System.Drawing.Point(562, 322);
            this.nextButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(192, 45);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "次へ";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // useAxisButton
            // 
            this.useAxisButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useAxisButton.Location = new System.Drawing.Point(20, 279);
            this.useAxisButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.useAxisButton.Name = "useAxisButton";
            this.useAxisButton.Size = new System.Drawing.Size(373, 34);
            this.useAxisButton.TabIndex = 5;
            this.useAxisButton.Text = "軸をそのまま使う";
            this.useAxisButton.UseVisualStyleBackColor = true;
            this.useAxisButton.Click += new System.EventHandler(this.useAxisButton_Click);
            // 
            // useButtonAxisButton
            // 
            this.useButtonAxisButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useButtonAxisButton.Location = new System.Drawing.Point(403, 279);
            this.useButtonAxisButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.useButtonAxisButton.Name = "useButtonAxisButton";
            this.useButtonAxisButton.Size = new System.Drawing.Size(350, 34);
            this.useButtonAxisButton.TabIndex = 6;
            this.useButtonAxisButton.Text = "ボタン・軸の組み合わせを使う";
            this.useButtonAxisButton.UseVisualStyleBackColor = true;
            this.useButtonAxisButton.Click += new System.EventHandler(this.useButtonAxisButton_Click);
            // 
            // InaccuracyModeCheckBox
            // 
            this.InaccuracyModeCheckBox.AutoSize = true;
            this.InaccuracyModeCheckBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InaccuracyModeCheckBox.Location = new System.Drawing.Point(562, 212);
            this.InaccuracyModeCheckBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.InaccuracyModeCheckBox.Name = "InaccuracyModeCheckBox";
            this.InaccuracyModeCheckBox.Size = new System.Drawing.Size(143, 32);
            this.InaccuracyModeCheckBox.TabIndex = 7;
            this.InaccuracyModeCheckBox.Text = "不正確モード";
            this.InaccuracyModeCheckBox.UseVisualStyleBackColor = true;
            this.InaccuracyModeCheckBox.Visible = false;
            this.InaccuracyModeCheckBox.CheckedChanged += new System.EventHandler(this.InaccuracyModeCheckBox_CheckedChanged);
            // 
            // ControllerSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(773, 386);
            this.Controls.Add(this.InaccuracyModeCheckBox);
            this.Controls.Add(this.useButtonAxisButton);
            this.Controls.Add(this.useAxisButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.cacelButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControllerSetupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ControllerSetupForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button cacelButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button useAxisButton;
        private System.Windows.Forms.Button useButtonAxisButton;
        private System.Windows.Forms.CheckBox InaccuracyModeCheckBox;
    }
}