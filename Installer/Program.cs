using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Installer
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Dictionary<string, string> arg = new Dictionary<string, string>();
            for (int i = 0;i < args.Length;i++)
            {
                if (args[i].StartsWith("--") && i + 1 < args.Length)
                {
                    arg.Add(args[i].Substring(2), args[i + 1]);
                    i++;
                }
            }
            if (arg.Count > 0 && arg.ContainsKey("installDir") && arg.ContainsKey("exeFile"))
            {
                // Update
                InstallResult result = Install(arg["exeFile"]);
                if (result.IsSuccess)
                {
                    MessageBox.Show("正常にNumerousControllerInterfaceのアップデートが完了しました。");
                    // 管理者権限を取って実行
                    Process.Start("explorer.exe", arg["exeFile"]);
                    Application.Exit();
                }
            } else
            {
                Application.Run(new Form1());
            }
        }

        public static InstallResult Install(string bveTsExe)
        {
            InstallResult result = new InstallResult();
            bool installSucceeded = false;
            bool unsupportedVersion = false;
            string errorMsg = "";
            // インストール開始
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(bveTsExe);
            string version = info.FileVersion;
            string bveInstallDir = new DirectoryInfo(bveTsExe).Parent.FullName;
            string installDir = Path.Combine(bveInstallDir, "Input Devices");
            if (version.Contains("."))
            {
                try
                {
                    int majorVersion = Convert.ToInt32(version.Substring(0, version.IndexOf(".")));
                    if (majorVersion != 5 && majorVersion != 6)
                    {
                        unsupportedVersion = true;
                    }
                    else if (majorVersion == 5)
                    {
                        string[] installFiles = new string[] { 
                            "Installer.Kusaanko.NumerousControllerInterface.dll", Path.Combine(installDir, "Kusaanko.NumerousControllerInterface.dll"),
                            "Installer.LibUsbDotNet.dll", Path.Combine(bveInstallDir, "LibUsbDotNet.dll"),
                        };
                        ExtractFiles(installFiles);
                        installSucceeded = true;
                    }
                    else if (majorVersion == 6)
                    {
                        string[] installFiles = new string[] {
                            "Installer.Kusaanko.NumerousControllerInterface.NET4.dll", Path.Combine(installDir, "Kusaanko.NumerousControllerInterface.NET4.dll"),
                            "Installer.LibUsbDotNet.dll", Path.Combine(bveInstallDir, "LibUsbDotNet.dll"),
                        };
                        ExtractFiles(installFiles);
                        installSucceeded = true;
                    }
                }
                catch (FormatException)
                {
                    unsupportedVersion = true;
                }
            }
            else
            {
                unsupportedVersion = true;
            }
            if (unsupportedVersion)
            {
                errorMsg = "非対応のバージョンです。(" + version + ")";
            }
            result.IsSuccess = installSucceeded;
            result.errorMsg = errorMsg;
            return result;
        }

        private static void ExtractFiles(string[] files)
        {
            for (int i = 0; i < files.Length; i+=2)
            {
                using (FileStream writer = new FileStream(files[i + 1], FileMode.Create))
                using (Stream reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(files[i]))
                {
                    byte[] buffer = new byte[8192];
                    int len;
                    while((len = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, len);
                    }
                }
            }
        }
    }

    public class InstallResult
    {
        public bool IsSuccess;
        public string errorMsg;
    }
}
