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

## GetPowerCount
力行の数です。力行が5段あるなら5です。

## GetPower
現在の力行の位置です。0が切です。

## GetBreakCount
ブレーキの数です。8段と非常なら9です。

## GetBreak
現在のブレーキの位置です。0が切です。8段あるなら9が非常です。

## Dispose
コントローラー終了時に呼ばれます。終了処理を書いてください。