# BveNumerousControllerInterface
Bve5、6用のコントローラー入力プラグイン  
コントローラーごとに入力プラグインが不要になり、このプラグインだけであらゆるコントローラーが使えるようにするのが目標です。  
このプラグインは次のコントローラーに対応しています

* TCPP-20001 電車でGO!コントローラー ワンハンドタイプ
* SLPH-00051 電車でGO!コントローラー ツーハンドタイプ
* TCPP-20001 電車でGO!コントローラー TYPE2
* マルチトレインコントローラー P4B6/P4B7/P5B5/P5B8
* Xbox用のコントローラー
* その他DirectInputに対応したコントローラー

また、マスコンだけでなくATS操作用のコントローラーも使用できます。

対応コントローラーの追加要望にも対応します。[Issues](https://github.com/kusaanko/BveNumerousControllerInterface/issues)ページで報告していただけるとありがたいです。[Twitter](https://twitter.com/kusaanko)での報告でも可能です。

※「電車で GO！」は、日本およびその他の国における株式会社 タイトーの商標または登録商標です。

※その他記載されている商品名は各社の商標または登録商標です。
# 機能
* 電車でGO!コントローラー等のマスコンをBveで使用可能にする
* Bve起動中にコントローラが切断後、再接続された場合自動復帰します
* 各ボタンに機能が割り当てられます
* 複数のコントローラーが接続中でも動作します(ただし、同じ名前のコントローラーが複数接続中だとうまくコントローラーを選択できません)
* ATC操作専用コントローラーが使えます

# インストール
[Releases](https://github.com/kusaanko/BveNumerousControllerInterface/releases)ページから最新版をダウンロードします。

Bve5.8以前なら`NumerousControllerInterface_Bve5.zip`、Bve6.0以降なら`NumerousControllerInterface_Bve6.zip`をダウンロードして下さい。

Bve5.8以前なら`C:\Program Files (x86)\mackoy\BveTs5`、Bve6.0以降なら`C:\Program Files\mackoy\BveTs6`を開き(もしくはBveをインストールしたディレクトリ)、ダウンロードしたzipファイルを展開し、中身を配置します。

配置したdllファイルを右クリックしてプロパティを開きます。(Newtonsoft.Json.dll、LibUsbDotNet.dll、Input Devices\Kusaanko.NumerousControllerInterface.dll)  
セキュリティを許可して下さい。

![許可](pic/1.jpg)  
Bveを起動し、設定画面を開き、入力デバイスを開きます。

NumerousControllerInterfaceにチェックを入れ、その他の不要な入力プラグインを無効化します。

# ドライバーをインストールする
USB接続でコントローラーとして認識されないデバイスにはドライバーを当てる必要があります。[Zadig](https://zadig.akeo.ie/)を使用してドライバーを当ててください。ドライバーにはWinUSBを使用してください。

# マスコンを使用する
Bveの設定画面を開き、入力デバイスを開きます。

NumerousControllerInterfaceを選択してプロパティーをクリックして下さい。

コントローラーから使いたいコントローラーを選択して、出てきた画面の指示に従って下さい 。

コントローラーを使用するには、コントローラーを有効にするにチェックを入れる必要があります。

# コーディング規約
こちらのコーディング規約に従います。[C# のコーディング規則](https://docs.microsoft.com/ja-jp/dotnet/csharp/fundamentals/coding-style/coding-conventions)

# このプラグインに対応コントローラーを追加する
LibUsbDotNetを使用してUSB接続のマスコンなどを追加できます。  
仕組みとしては、コントローラーをボタンと軸のコントローラーとして認識させてあとは他のコントローラーと同じように処理します。  
PS2DenshadeGoType2.csなどを参考にしてください。  
IControllerをインターフェイスにすることでコントローラーを制御するクラスを作成できます。  
その後、ControllerProfile.csのGetAllControllers関数内の最後に以下の行を追加します。

```c#
controllers.AddRange(NewController.Get());
```

また、NumerousInterface.NET4にも追加するのを忘れないでください。ファイルは既存の項目として **リンクとして追加** してください。

## デフォルトのプロファイルを追加する
デフォルトのプロファイルを用意する場合はSettings.cs内のコンストラクター内に記述します。  
JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)の例

```c#

{
    ControllerProfile profile = new ControllerProfile();
    profile.IsTwoHandle = true;
    profile.IsMasterController = true;
    profile.PowerAxises = new int[] { 21 };
    profile.PowerAxisStatus = new int[,] {
        { -1000 },
        { 1000 },
        { 1000 },
        { -1000 },
        { -1000 },
        { -8 }
    };
    profile.PowerButtons = new int[] { 0 };
    profile.PowerButtonStatus = new bool[,] {
        { false },
        { true },
        { false },
        { true },
        { false },
        { true }
    };
    profile.BreakButtons = new int[] { 4, 5, 6, 7};
    profile.BreakButtonStatus = new bool[,] { 
        { true, true, false, true },
        { false, true, true, true },
        { false, true, false, true },
        { true, true, true, false },
        { true, true, false, false },
        { false, true, true, false },
        { false, true, false, false },
        { true, false, true, true },
        { true, false, false, true },
        { false, false, false, false }
    };
    profile.CalcDuplicated();
    profile.KeyMap = new Dictionary<int, int[]>();
    profile.KeyMap.Add(1, new int[] { 0, 2});
    profile.KeyMap.Add(2, new int[] { 0, 0 });
    profile.KeyMap.Add(3, new int[] { -1, 1 });
    profile.KeyMap.Add(9, new int[] { -3, 12 });
    profile.KeyMap.Add(8, new int[] { -3, 11 });
    Profiles.Add("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)", profile);
    ProfileMap.Add("JC-PS101U", "JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
}
```
ここで、プロファイル名はファイル名であるため、ファイル名に使えない文字は使わないでください。エラーを起こします。  
ProfileMapはコントローラー名、プロファイル名です。これでコントローラーとプロファイルを紐付けます。
また、

```c#
profile.CalcDuplicated();
```
この記述を忘れないでください。これをすることで重複した組み合わせの計算を行っています。また、Null回避のためにも必ず最後に実行するようにしてください。

# 協力
サハ209 - [@saha209](https://github.com/saha209)

# ライセンス
[SlimDX](https://github.com/SlimDX/slimdx) - Copyright (c) 2007-2012 SlimDX Group [MIT License](https://github.com/SlimDX/slimdx/blob/master/License.txt)

[Json.NET](https://github.com/JamesNK/Newtonsoft.Json) - Copyright (c) 2007 James Newton-King [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)

[LibUsbDotNet](https://github.com/LibUsbDotNet/LibUsbDotNet) - [GNU Lesser General Public License v3.0](https://github.com/LibUsbDotNet/LibUsbDotNet/blob/master/LICENSE)
