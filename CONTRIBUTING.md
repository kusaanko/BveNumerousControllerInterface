# コントリビュートについての規定

## Gitブランチモデル
GitHub Flowを採用します。  
一定のバージョンを保持したりせず、バグ修正などは次のリリース時に行うことにします。

## コーディング規約
こちらのコーディング規約に従います。[C# のコーディング規則](https://docs.microsoft.com/ja-jp/dotnet/csharp/fundamentals/coding-style/coding-conventions)


# このプラグインに対応コントローラーを追加する
LibUsbDotNetを使用してUSB接続のマスコンなどを追加できます。  
PS2DenshadeGoType2.csなどを参考にしてください。  
NCIControllerをインターフェイスにすることでコントローラーを制御するクラスを作成できます。  
通常のコントローラーと同じように、ボタンと軸の値を報告するか、GetPowerとGetBreakを実装して直接力行と制動を報告することができます。  
その後、ControllerProfile.csのGetAllControllers関数内の最後に以下の行を追加します。

```c#
controllers.AddRange(NewController.Get());
```

コントローラーの制御を行うクラスはControllerフォルダ内に作成し、名前空間は`Kusaanko.Bvets.NumerousControllerInterface.Controller`としておいてください。

また、NumerousInterface.NET4にも追加するのを忘れないでください。ファイルは既存の項目として **リンクとして追加** してください。

## デフォルトのプロファイルを追加する
デフォルトのプロファイルを用意する場合はSettings.cs内のコンストラクター内に記述します。  
JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)の例

```c#

{
    ControllerProfile profile = new ControllerProfile("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
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
    profile.KeyMap.Add(1, ButtonFeature.ReverserBackward);
    profile.KeyMap.Add(2, ButtonFeature.ReverserForward);
    profile.KeyMap.Add(3, ButtonFeature.Horn);
    profile.KeyMap.Add(9, ButtonFeature.Pause);
    profile.KeyMap.Add(8, ButtonFeature.Fastforward);
    Profiles.Add("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)", profile);
    ProfileMap.Add("JC-PS101U", "JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
}
```
ProfileMapはコントローラー名、プロファイル名です。これでコントローラーとプロファイルを紐付けます。
また、

```c#
profile.CalcDuplicated();
```
この記述を忘れないでください。これをすることで重複した組み合わせの計算を行っています。また、Null回避のためにも必ず最後に実行するようにしてください。

# NCIControllerクラス
## Get
戻り値:`System.Collections.Generic.List<NCIController>`

この関数を呼ぶことでコントローラーを登録します。特にオーバーライドする必要などはないのでstatic関数で作成して、ControllerProfileクラスから呼び出してください。

## コンストラクター
コントローラーの初期化処理を書いてください。

## GetName
コントローラー名を返します。コントローラー一覧にそのまま表示されます。

## GetControllerType
コントローラーの種類です。コントローラーの種類に表示されます。私はDirectInputやLibUsbDotNetとしていますがお好きにどうぞ。

## GetButtons
コントローラーのボタンの状態です。trueでオン、falseでオフです。ボタンの数と同じ数の配列を用意してください。

## GetSliders
コントローラーの軸の状態です。下限や上限はお好きにどうぞ。DirectInputでは-1000から1000を使用しています。

## GetSliderMinValue
スライダーの最小値です。デフォルトでは-1000です。

## GetSliderMaxValue
スライダーの最大値です。デフォルトでは1000です。

## GetPowerCount
力行の数です。力行が5段あるなら5です。

## GetPower
現在の力行の位置です。0が切です。

## GetBreakCount
ブレーキの数です。8段と非常なら9です。

## GetBreak
現在のブレーキの位置です。0が切です。8段あるなら9が非常です。

## HasReverser
リバーサーを持っているかどうかです。

## GetReverser
NCIController内のReverseを返します。

## Dispose
コントローラー終了時に呼ばれます。終了処理を書いてください。

## IsDisposed
コントローラーが終了済みかどうかを返します。

# プラグインを作成する
プラグインを作成するには、.NET Framework用のdllを作成します。

Kusaanko.NumerousControllerInterface.dllを参照に追加してください。

次に、NumerousControllerPluginをインターフェースに設定したpublicなクラスを用意してください。

GetAllControllers内でNCIControllerを継承したクラスのインスタンスを追加したリストを返してください。ここでは、毎回新しいインスタンスを作成する必要があります。

完成したdllは`BveTs\Settings\Kusaanko.NumerousControllerInterface.Plugins`フォルダ内に入れてください。

詳しいことは[サンプル](https://github.com/kusaanko/BveNumerousControllerInterface/tree/main/plugin_example)を御覧ください。
