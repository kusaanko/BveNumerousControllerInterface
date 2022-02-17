
namespace Kusaanko.Bvets.NumerousControllerInterface
{
    partial class UpdateForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.updateHistoryTextBox = new System.Windows.Forms.TextBox();
            this.newVersionLabel = new System.Windows.Forms.Label();
            this.openDownloadPageButton = new System.Windows.Forms.Button();
            this.autoDownloadButton = new System.Windows.Forms.Button();
            this.ignoreButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "NumerousControllerInterfaceに更新があります";
            // 
            // updateHistoryTextBox
            // 
            this.updateHistoryTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateHistoryTextBox.Location = new System.Drawing.Point(12, 74);
            this.updateHistoryTextBox.Multiline = true;
            this.updateHistoryTextBox.Name = "updateHistoryTextBox";
            this.updateHistoryTextBox.ReadOnly = true;
            this.updateHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.updateHistoryTextBox.Size = new System.Drawing.Size(582, 299);
            this.updateHistoryTextBox.TabIndex = 1;
            // 
            // newVersionLabel
            // 
            this.newVersionLabel.AutoSize = true;
            this.newVersionLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newVersionLabel.Location = new System.Drawing.Point(12, 39);
            this.newVersionLabel.Name = "newVersionLabel";
            this.newVersionLabel.Size = new System.Drawing.Size(120, 21);
            this.newVersionLabel.TabIndex = 2;
            this.newVersionLabel.Text = "新しいバージョン:";
            // 
            // openDownloadPageButton
            // 
            this.openDownloadPageButton.Location = new System.Drawing.Point(367, 392);
            this.openDownloadPageButton.Name = "openDownloadPageButton";
            this.openDownloadPageButton.Size = new System.Drawing.Size(146, 23);
            this.openDownloadPageButton.TabIndex = 3;
            this.openDownloadPageButton.Text = "ダウンロードページを開く";
            this.openDownloadPageButton.UseVisualStyleBackColor = true;
            this.openDownloadPageButton.Click += new System.EventHandler(this.openDownloadPageButton_Click);
            // 
            // autoDownloadButton
            // 
            this.autoDownloadButton.Location = new System.Drawing.Point(82, 392);
            this.autoDownloadButton.Name = "autoDownloadButton";
            this.autoDownloadButton.Size = new System.Drawing.Size(279, 23);
            this.autoDownloadButton.TabIndex = 4;
            this.autoDownloadButton.Text = "自動ダウンロード(ファイルの展開、コピーのみ手動)";
            this.autoDownloadButton.UseVisualStyleBackColor = true;
            this.autoDownloadButton.Click += new System.EventHandler(this.autoDownloadButton_Click);
            // 
            // ignoreButton
            // 
            this.ignoreButton.Location = new System.Drawing.Point(519, 392);
            this.ignoreButton.Name = "ignoreButton";
            this.ignoreButton.Size = new System.Drawing.Size(75, 23);
            this.ignoreButton.TabIndex = 5;
            this.ignoreButton.Text = "無視";
            this.ignoreButton.UseVisualStyleBackColor = true;
            this.ignoreButton.Click += new System.EventHandler(this.ignoreButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(12, 390);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(51, 21);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "status";
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 427);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.ignoreButton);
            this.Controls.Add(this.autoDownloadButton);
            this.Controls.Add(this.openDownloadPageButton);
            this.Controls.Add(this.newVersionLabel);
            this.Controls.Add(this.updateHistoryTextBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NumerousControllerInterface";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox updateHistoryTextBox;
        private System.Windows.Forms.Label newVersionLabel;
        private System.Windows.Forms.Button openDownloadPageButton;
        private System.Windows.Forms.Button autoDownloadButton;
        private System.Windows.Forms.Button ignoreButton;
        private System.Windows.Forms.Label statusLabel;
    }
}