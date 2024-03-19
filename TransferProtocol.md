# NumerousControllerInterface通信規格
NumerousControllerInterfaceでは、NumerousControllerInterface独自のフォーマットを用いてBVEを操作できます。

また、コントローラーごとに設定を用意していただくことでNumerousControllerInterfaceと違ったフォーマットのコントローラーも使用していただけます。

対応ボタン数、ノッチ数はともにint値の最大値である`2,147,483,647`までです。理論上の話ですが。

頑張れば運転台すべてをUSB一本で接続できるという感じです。

今後BVEからの情報取得にも対応予定。

# COMポートについて
COMポートではデフォルトの通信速度は`9600kbps`です。

# NumerousControllerInterface通信規格
すべてのコマンドの最後には`\n`を使用して区切ります。様々なコントローラーに対応するため、`\r`や`\r\n`にも対応していますが、基本`\n`でお願いします。

```
コマンド 引数
コマンド 引数1,引数2,...
```
基本的なコマンドです。コマンドは複数行同時に送信しても問題ありません。

リクエストはNumerousControllerInterfaceが送信するコマンド、レスポンスはコントローラーが送信するコマンドです。

## 初期化
リクエスト
```
NCIInit
```

レスポンス
```
NCIInit
NCIName コントローラー名
NCIPowerCount 力行の段数
NCIBreakCount 制動の段数
NCIHasReverser [True/False]
NCIButtons ボタン数
NCIButtonNames ボタン0の名前,ボタン1の名前,ボタン2の名前...
NCIButtonName ボタンのインデックス,ボタンの名前
NCINeedEmptyRequest [True/False]
NCIInitEnd
```
これらのコマンドを送信してください。
### NCIInit
初期化を開始するという合図です。

### NCIName
コントローラー名です。初期化時のみ使用できます。ここで指定した名前がコントローラー一覧に使用されます。

### NCIPowerCount
力行の段数です。0で力行は無効化されます。

### NCIBreakCount
制動の段数です。非常も含めてください。0で制動は無効化されます。

### NCIHasReverser
レバーサーがあるかどうかです。デフォルトではFalseです。レバーサーがある場合、Trueを送信してください。

### NCIButtons
ボタンの数を指定します。

### NCIButtonNames
ボタン名です。必須ではありません。ボタン0から順に名前を指定します。

### NCIButtonName
ボタン名です。必須ではありません。ボタンのインデックスを使用して名前を指定できます。

### NCINeedEmptyRequest
毎回フレーム要求を何か読み取る必要があるコントローラーの場合にNCINoneという意味のない文字列を毎フレーム送信します。Trueでこの機能を有効化します。

### NCIInitEnd
このコマンドでNumerousControllerInterfaceはコントローラーを認識します。このコマンドが一定時間送られないとコントローラーの認識プロセスを中断します。

## 力行
レスポンス
```
NCIPower 段数
```
事前に指定した段数を超える値を設定すると事前に指定した段数の最大値が使用されます。負の値は逆回しになります。

## 制動
レスポンス
```
NCIBreak 段数
```

## レバーサー
レスポンス
```
NCIReverser [F/C/B]
```
- F=前
- C=切
- B=後

## ボタン
レスポンス
```
NCIButton ボタンのインデックス/名前,[True/False]
```
```
NCIButton ボタンのインデックス/名前,[true/false]
```
```
NCIButton ボタンのインデックス/名前,[1/0]
```
ボタンのインデックスまたは名前を指定します。True/true/1でオン、False/false/0でオフです。C言語のbool値判定のように、0以外はすべてTrueとみなされます。

例
```
初期化時
NCIButtonCount 1
NCIButtonNames A
もしくは
NCIButtonCount 1
NCIButtonName 0,A
```
```
NCIButton 0,True
NCIButton A,False
```
ボタン0にAという名前を登録しているため、この操作でボタン0を一瞬押したことにできます。

## 切断
レスポンス
```
NCIExit
```
