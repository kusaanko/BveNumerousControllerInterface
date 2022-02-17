using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class UpdateForm : Form
    {
        private string _downloadPageURL;
        private string _downloadURL;
        private string _filePath;
        public UpdateForm(string version, string history, string downloadPageURL, string downloadURL, string filePath)
        {
            InitializeComponent();
            newVersionLabel.Text = "新しいバージョン:" + version;
            statusLabel.Text = "";
            updateHistoryTextBox.Text = history.Replace("\n", "\r\n");
            updateHistoryTextBox.Select(0, 0);
            _downloadPageURL = downloadPageURL;
            _downloadURL = downloadURL;
            _filePath = filePath;
        }

        private void ignoreButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openDownloadPageButton_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = _downloadPageURL,
                UseShellExecute = true,
            };
            Process.Start(info);
            Close();
        }

        private void autoDownloadButton_Click(object sender, EventArgs e)
        {
            autoDownloadButton.Visible = openDownloadPageButton.Visible = ignoreButton.Visible = false;
            statusLabel.Text = "ファイルをダウンロード中";
            Update();
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
                    client.Encoding = Encoding.UTF8;
                    Directory.CreateDirectory(Directory.GetParent(_filePath).FullName);
                    client.DownloadFile(_downloadURL, _filePath);
                    statusLabel.Text = "";
                    Update();

                    MessageBox.Show("ダウンロードしたファイルとインストールディレクトリが開かれるので、ファイルを開いて中のファイルをインストールディレクトリにコピーしてBveを再起動してください。");
                    Close();

                    Assembly assembly = Assembly.GetEntryAssembly();
                    string path = Directory.GetParent(assembly.Location).FullName;
                    try
                    {
                        Process.Start("explorer.exe", "/select," + _filePath);
                        Process.Start("explorer.exe", path);
                    }
                    catch (Exception) { }
                }
                catch(Exception)
                {
                    statusLabel.Text = "エラーが発生しました";
                    ignoreButton.Visible = true;
                    ignoreButton.Text = "閉じる";
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UpdateForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "UpdateForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
    }
}
