このガイドでは上級ユーザー向けにPayloadファイルのフォーマットを紹介します。


## Payloadとは
payloadは送信するパラメーター等の設定ファイルです。
CSVファイルに似た、タブ区切りのファイルです。
厳密には Character Seperated ということでCSVファイルの仲間ですが、
わかりやすくするために拡張子をtsvにしています。

おすすめのTSVエディタは
https://edit-csv.net/
です。ブラウザで完結し、Excelライクな操作で行列の入れ替えも可能です。
VS Code拡張機能版も存在します。


> [!NOTE]+ なぜTSVか
> ❌️ CSV: プロンプトにカンマを使用するので
> 🔺 json: なんか使いづらい
> 🔺 yaml: 一覧性がきつい。エディタがない
> 🔵 TSV: 表形式で編集しやすい、エスケープも楽ちん、C#的にも扱いやすい


### 仕組み
基本的にはSeleniumというWebdriver(ブラウザを擬似的に操作する仕組み)を使い、

　Selenium のセレクタ(By)、セレクタ指定文字列(XPath)、送信内容、データと対応するUIの種類、ピン留め有無ほか

……のデータを使ってWebページを操作しています。
TSVファイルにはこれらの組み合わせが行ごとに書いてあり、
一行につきひとつのデータが対応しています。
リスト型のデータはリストとして表示するデータがあるのでさらに入れ子になっています。

### Tips
**Seleniumのテスト**
起動後▶の右にあるデバッグボタンでSeleniumのXPathをテスト可能です。
ChromeでXPathを取ったあと、デバッグボタンを押すとテキストボックスが現れるので、
そこに貼り付けてEnterキーでClickをエミュレートします。

**Payload以外の設定**
- Payload以外の設定は、FAQのconfigファイルを編集してsd指定のファイルパスを変更してください。
	- Stable Diffusion WebUI 以外のAIを使う場合
	- モデルフォルダパスの再設定や、モデルデータを追加で読み込む場合
	- など
- SDWebUI以外は動作を確認したわけではないので、動いたらラッキーでお願いします……

場所:
アプリの設定:
`%LocalAppdata%\Kayno.AI.Studio\Kayno~~(長い文字)\(番号)\user.config` 
※デバッグ時はexeと同じフォルダ

生成用パラメーター(ペイロード):
`(アプリのexeがあるフォルダ)\Data\(プリセットスロット番号)\Payload.tsv`


## パラメーターの一覧

| prop | type | desc | null |
| ---- | ---- | ---- | ---- |
|      |      |      |      |


string PropertyName
プログラムが扱う一意の識別子。
空欄不可

object PropertyValue
値。数値は0、1、……など。
空欄不可

メモ:
DropAreaの場合、画像ファイルのファイル名。
パスは自動的に現在のプリセットスロットのディレクトリから設定されるため、名前と拡張子だけでOK

string Label
翻訳後の表示名

string LabelSrc
ニュートラル言語の表示名

string Tag
タグや補助的な表示Tipsなど

※Prompt欄に使われています

enum WebSelectorKey
SeleniumのBy識別子判別用のプロパティ。
通常はXPathを使用します。

※これを書いた時点ではXPath以外 未対応

string WebSelectorValue
SeleniumのFindElement用の識別子
※Chromeで検証→要素を選択→右クリック→コピー→XPath　で取得

bool IsPinned
設定パネルにピン留めするかどうか。

bool IsVisible
設定パネルに表示するかどうか。

enum UI
どのUIを設定するか決めます。
CheckBox, TextBox, Slider, List(ComboBox), DropArea, SplitButton などがあります。

UI_Slider 関連
Slider用の最小値、最大値、スナップ単位を設定します。
⚠ 小数の場合、スナップ単位を設定しないと「0.1+0.2問題」につながるので設定推奨

string UI_ItemsSourcePath
↓のパス指定

object UI_ItemsSource
List系アイテムのリストの中身です。
後述します。

string Comment
コメント。Tooltip などに一部使われたりしますが、管理用などにお使いください。

---

### Payload Templateのリスト
ItemsSourceの中身です。

| prop           | type           | desc                | null |
| -------------- | -------------- | ------------------- | ---- |
| TPropertyName  | string         | 管理用                 | 不可   |
| TPropertyValue | object         | 値。リストのIndex。実質ほぼint | 不可   |
| TLabel         | string         | 翻訳後の表示名             |      |
| TCategory      | string         | カテゴリ                |      |
| TCategory2     | object         | カテゴリ2               |      |
| TPath          | string         | ファイルパス              |      |
| TParentDir     | string         | 親ディレクトリ             |      |
| TTags          | List of string | タグ。説明などに。           |      |
| TDescription   | string         | 説明                  |      |
| TParameter     | string         | LoRAなどで起動ワード等を想定    |      |
| TComment       | string         | コメント                |      |



---

## 上級ユーザー向けPayloadカスタムのFAQ

Q
Payloadをカスタムして、img2img以外のタブを選択して処理をすることはできますか？

A
可能です。たとえば、Extrasを選択してGFPGANで超解像する(高解像度化)方法ですが、

**Extrasタブをクリック　→　GFPGANのチェックボックスをクリック　→　設定値入力**

という形ですよね？それをそのままPayloadに記載します。
タブの選択は tab_img2img 、
チェックボックスはuse_~~ 系、
数値の場合はSliderを使っている設定の行をご参考ください。

これらの行をコピーし、名前を一意のものに変えます。
Labelをわかりやすく変更します。
そしてXPathの値をChromeの検証ツールで調べてコピペします。
``//*[@id="......... 
という形になるかと思います。具体的な値は取ってみないとわかりませんが、
``//div[@id='img2img_image']
こんな感じできれいにまとまることもあります。し、中にdivとか入ってるかもしれません。動かして動かなかったら変えてみてください。

※なおControlNetのボタン群(fieldsetsと言います)みたいなのは、XPathのうち
`label[1]`
を内部処理で置換しています。これはfieldsetsかどうか(`label[1]`という単語が含まれるか)を判断しているので、その点をご考慮ください。

これをそれぞれに適用し、うまくいけばたとえそれがどんな設定値でも拡張機能で追加されたものでもKaynoから操作できるはずです！



