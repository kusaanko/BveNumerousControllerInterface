using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("C:\\Program Files\\mackoy\\BveTs6\\BveTs.exe"))
            {
                listBox1.Items.Add("C:\\Program Files\\mackoy\\BveTs6\\BveTs.exe");
            }
            if (File.Exists("C:\\Program Files (x86)\\mackoy\\BveTs5\\BveTs.exe"))
            {
                listBox1.Items.Add("C:\\Program Files (x86)\\mackoy\\BveTs5\\BveTs.exe");
            }
            listBox1.Items.Add("その他");
            label3.Text = "本体バージョン: 0.17 , インストーラーバージョン：" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string installTargetExe = listBox1.Text;
            if (listBox1.Text == "その他")
            {
                installTargetExe = null;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "BVE 実行ファイル|BveTs.exe";
                dialog.InitialDirectory = "C:\\";
                dialog.Title = "BVEの実行ファイルを選択してください";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    installTargetExe = Path.GetFullPath(dialog.FileName);
                }
            }
            if (installTargetExe != null)
            {
                InstallResult result = Program.Install(installTargetExe);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("インストールできませんでした。\n" + result.errorMsg);
                } else
                {
                    MessageBox.Show("正常にインストールできました。\n入力プラグイン設定からNumerousControllerInterfaceを有効にしてください。");
                    Application.Exit();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
