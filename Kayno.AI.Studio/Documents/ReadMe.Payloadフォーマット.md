
このガイドでは上級ユーザー向けにPayloadファイルのフォーマットを紹介します。

## Payloadとは
payloadは送信するパラメーター等の設定ファイルです。  
CSVファイルに似た、タブ区切りのファイルです。  
厳密には Character Seperated ということでCSVファイルの仲間ですが、  
わかりやすくするために拡張子をtsvにしています。  

おすすめのTSVエディタは  
https://edit-csv.net/
です。  
ブラウザで完結し、Excelライクな操作で行列の入れ替えも可能です。  
VS Code拡張機能版も存在します。  


> **❔️ なぜTSVか?**  
> ❌️ CSV: プロンプトにカンマを使用するので  
> 🔺 json: なんか使いづらい  
> 🔺 yaml: 一覧性がきつい。エディタがない  
> 🔵 TSV: 表形式で編集しやすい、エスケープも楽ちん、C#的にも扱いやすい  


### 仕組み
基本的にはSeleniumというWebdriver(ブラウザを擬似的に操作する仕組み)を使い、

　` Selenium のセレクタ(By)、セレクタ指定文字列(XPath)、送信内容、データと対応するUIの種類、ピン留め有無ほか `

……のデータを使ってWebページを操作しています。  
TSVファイルにはこれらの組み合わせが行ごとに書いてあり、  
一行につきひとつのデータが対応しています。  
リスト型のデータはリストとして表示するデータがあるので
さらに入れ子になっています。

## Tips
### **Seleniumのテスト**
起動後▶の右にあるデバッグボタンでSeleniumのXPathをテスト可能です。
ChromeでXPathを取ったあと、デバッグボタンを押すとテキストボックスが現れるので、
そこに貼り付けてEnterキーでClickをエミュレートします。

### **Payload以外の設定**
- Payload以外の設定は、FAQのconfigファイルを編集してsd指定のファイルパスを変更してください。
	- Stable Diffusion WebUI 以外のAIを使う場合
	- モデルフォルダパスの再設定や、モデルデータを追加で読み込む場合
	- など
- SDWebUI以外は動作を確認したわけではないので、動いたらラッキーでお願いします……

### 場所:
アプリの設定:  
`%LocalAppdata%\Kayno.AI.Studio\Kayno~~(長い文字)\(番号)\user.config` 
※デバッグ時はexeと同じフォルダ

生成用パラメーター(ペイロード):  
`(アプリのexeがあるフォルダ)\Data\(プリセットスロット番号)\Payload.tsv`


## パラメーターの一覧


### クラスのプロパティ一覧

| プロパティ名               | 説明                               | タイプ                                      | IsNullable |
| -------------------- | -------------------------------- | ---------------------------------------- | ---------- |
| PropertyName         | プロパティ名                           | `string`                                 | No         |
| PropertyValue        | 値。                               | `object?`                                | Yes        |
| Label                | 翻訳後の表記名                          | `string?`                                | Yes        |
| LabelSrc             | ↑の元(英文)                          | `string?`                                | Yes        |
| Tag                  | 一部のUIで使用(Promptの👍️など)           | `string?`                                | Yes        |
| WebSelectorKey       | SeleniumのByセレクタ                  | `SeleniumBySelector?`                    | Yes        |
| WebSelectorValue     | Seleniumの↑の中身<br>※ベータ現在はXPathのみ  | `string?`                                | Yes        |
| UI                   | 使用するUI<br>(後述)                   | `UISelector?`                            | Yes        |
| UI_IsPinned          | ピン留めの有無                          | `bool?`                                  | Yes        |
| UI_IsVisible         | UIへの表示有無                         | `bool?`                                  | Yes        |
| UI_SliderMinVal      | -                                | `double?`                                | Yes        |
| UI_SliderMaxVal      | -                                | `double?`                                | Yes        |
| UI_SliderSnapValue   | スライダーの場合の動き幅                     | `double?`                                | Yes        |
| UI_ItemsSourcePath   | リスト型UIの場合のソース                    | `string?`                                | Yes        |
| UI_ItemsSource       | ↑の実体(後述)                         | `ObservableCollection<PayloadTemplate>?` | Yes        |
| UI_ItemsSourceFilter | -                                | `string?`                                | Yes        |
| Command              | C#のRoutedUICommand<br>※ベータ現在テスト中 | `string`                                 | Yes        |
| Comments             | コメント。Tooltip表示されます               | `string?`                                | Yes        |


### Enum: UISelector

| 値           | Enum上の数値 | 備考        |
| ----------- | -------- | --------- |
| CheckBox    | 10       |           |
| Button      | 11       |           |
| Slider      | 20       |           |
| TextBlock   | 30       |           |
| TextBox     | 31       |           |
| SplitButton | 40       | 左右に別れたボタン |
| List        | 41       |           |
| Expander    | 90       | ※未使用      |
| DropArea    | 91       | ファイル参照    |
| None        | 0        |           |

### Enum: SeleniumBySelector

| 値               | 数値  |            |
| --------------- | --- | ---------- |
| ID              | 10  |            |
| Class           | 11  |            |
| LinkText        | 30  |            |
| LinkTextPartial | 31  |            |
| CSS_Selector    | 40  |            |
| TagName         | 41  |            |
| XPath           | 90  | ←ベータ現在これのみ |
| None            | 0   |            |


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


<br>


## 上級ユーザー向けPayloadカスタムのFAQ

**Q**  
Payloadをカスタムして、img2img以外のタブを選択して処理をすることはできますか？

**A**  
可能です。たとえば、Extrasを選択してGFPGANで超解像する(高解像度化)方法ですが、

**Extrasタブをクリック　→　GFPGANのチェックボックスをクリック　→　設定値入力**

という形ですよね？それをそのままPayloadに記載します。  
タブの選択は tab_img2img 、  
チェックボックスはuse_~~ 系、  
数値の場合はSliderを使っている設定の行をご参考ください。  

これらの行をコピーし、名前を一意のものに変えます。  
Labelをわかりやすく変更します。  
そしてXPathの値をChromeの検証ツールで調べてコピペします。  
``//*[@id="......... ``  
という形になるかと思います。具体的な値は取ってみないとわかりませんが、
``//div[@id='img2img_image'] ``  
こんな感じできれいにまとまることもあります。
し、中にdivとか入ってるかもしれません。
動かして動かなかったら変えてみてください。 

※なおControlNetのボタン群(fieldsetsと言います)みたいなのは、
XPathのうち
``label[1]``  
を内部処理で置換しています。  
これはfieldsetsかどうか(`label[1]`という単語が含まれるか)を判断しているので、その点をご考慮ください。
  
これをそれぞれに適用し、うまくいけばたとえそれがどんな設定値でも、  
拡張機能で追加されたものでもKaynoから操作できるはずです！



