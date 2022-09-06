using Kusaanko.Bvets.NumerousControllerInterface;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NumerousControllerTestPlugin
{
    // 空っぽのプラグイン。何もしないコントローラーを追加します。
    public class TestPlugin : NumerousControllerPlugin
    {
        // プラグイン終了時の処理
        public void Dispose()
        {

        }

        // コントローラー追加
        public List<NCIController> GetAllControllers()
        {
            List<NCIController> controllers = new List<NCIController>();
            controllers.Add(new Controller());
            return controllers;
        }

        // プラグイン名
        public string GetName()
        {
            return "TestPlugin";
        }

        // バージョン名
        public string GetVersion()
        {
            return "1.0";
        }

        // プラグイン読み込み時
        public void Load()
        {

        }

        // プラグイン読み込み時の設定ファイルの読み込み。BveTs/Settingsフォルダが引数に渡されます。
        public void LoadConfig(string directory)
        {
            MessageBox.Show(directory);
        }

        // NumerousControllerInterfaceのプロパティ内の設定ボタンを押したときの動作です。
        public void ShowConfigForm()
        {
            new Form1().Show();
        }
    }
}
