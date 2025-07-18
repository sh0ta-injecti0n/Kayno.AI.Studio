
### HowTo編  
**Q**　どうやって使えばいいですか？  

**A**　
[ReadMe.チュートリアル](ReadMe.チュートリアル.md) をご覧ください。

### 初心者編
<!-- 
**Q**　用語がわかりません。何か一覧などはありませんか？  
**A**　[こちら]をご覧ください。

**Q**　初心者です。Stable Diffusionの基本知識なども知りたいのですが、おすすめのサイトなどはありますか？  
**A**　よければ私の解説をご覧ください！
[【ゼロショタ】ゼロから始めるショタ向けAI]
※その他:
[【SD】WebUIの絶対に覚えておきたい超絶便利機能10選！！！！！！！！！](https://blogcake.net/webui-technique/)

**Q**　アニメーション制作のコツとかありますか？
**A**　アニメのコツ:
https://note.com/hanagasa_manya/n/n582b5fb724a5
-->

**Q**　ControlNetのSketchやLineArtモードでラフからうまく元のキャラの輪郭やイメージが保てません…  
**A**　SDXL用のモデルを使いましょう。※チュートリアルにも追記しました。  
> **🤔 うまくラフからAIに読み込ませるには？**  
> デフォルトのControlNetでは線画やラフに追従しないので、  
> **ラフから線画ならSDXL用のモデルがおすすめです！(青龍さんやkataragiさん等)**  
> ※リンクは準備編で紹介しています。  
> [→ReadMe.チュートリアル.SDWebUI準備](./ReadMe.チュートリアル.SDWebUI準備.md)

<br>

**Q**　狙った通りのイラスト・画像が出ないのですが、
どうすればいいですか？  

**A**　実写用のモデル、イラスト向きのモデル、
特定のキーワードや解像度設定など様々な条件が絡みます。
描きたいものを整理した上でChatGPTかCohereにたずねるのがおすすめです。

なお、PCが一度休止状態になるとStable Diffusion WebUIの出力が安定しなくなることがあるので、Stable Diffusionを再起動してみるのがおすすめです。

**・参考: Ponyでの各 ControlNet パラメーター変化図**  
※すべて PonyRealism  
✍ 全般的に、かかりすぎなくらいControlNetを効かせてから、  
Denoising Strength を1.0から下げていって元画像と変化のバランスを探求するのがコツです。

また、解像度が低すぎると全く追従しなくなることもあり、
SDXL、Pony、Illustrious系は1024前後はないと厳しいかも？

|          | おすすめモデル                               | おすすめ weight             | おすすめ値                           | Denoising Strength | 備考     |
| -------- | ------------------------------------- | ----------------------- | ------------------------------- | ------------------ | ------ |
| 例        | ~t2i, diffuser とか                     | 0.7-1.0                 | end 0.7-1.0, res. 512-1024px    |                    |        |
| Canny    | bdsqlsz                               | 1                       | s0 e0.5                         |                    |        |
| Depth    | bdsqlsz / midas                       | 1                       | s0 e0.5                         |                    |        |
| Openpose | bdsqlsz/dw                            | 1                       | s0 e0.5                         |                    |        |
| Lineart  | bdsqlsz/lineart anime denoise         | 1                       | s0 e0.5                         | 0.8                |        |
| Scribble | model: bdsqlsz<br>pp: xdog / pidinet  | xdog: 1<br>pidinet: 0.5 | xdog: s0 e0.5<br>pidinet: s0 e1 | 0.8-               |        |
| blur     | kohya / blur gaussian                 | 0.7                     | s0 e0.7                         |                    |        |
| tile     | bdsqlsz / tile anime β, tile colorfix | 0.7                     | s0 e0.7                         |                    | 画風の変更に |

<br>

### アプリ設定・機能編
**Q**　アプリの設定はどこに保存されていますか？  

**A**　起動後の右下の⚙アイコンから開けます。  
アプリの設定:
```
%LocalAppdata%\Kayno.AI.Studio\Kayno~~(長い文字)\(番号)\
```
～　以下の、user.config 
※デバッグ時はexeと同じディレクトリ  

生成用パラメーター(ペイロード):  
```
(アプリのexeがあるフォルダ)\Data\(プリセットスロット番号)\Payload.tsv
```
<br>

**Q**　ショートカットキーは何が使えますか？  

**A**　下記をご覧ください。

| コマンド名            | 修飾キー           | キー      | コメント         |
| ---------------- | -------------- | ------- | ------------ |
| **設定パネルのピン切り替え** | `Ctrl`         | `Space` |              |
| **スクリーンキャプチャ**   | `Alt`          | `X`     |              |
| **Payloadの送信**   | `Ctrl + Shift` | `F11`   |              |
| **生成処理実行 **      | `Ctrl + Shift` | `F12`   |              |
| **生成処理実行**(2)    | `Ctrl`         | `Enter` |              |
| **画像を貼り付け**      | `Ctrl + Shift` | `Enter` | ベータ　クリスタのみ対応 |
| **スクリプト実行**      | `Alt`          | `A`     | ベータ          |

<br>

**Q**　txt2img と img2img の切り替えはどうすればいいですか？  

**A**　まだimg2imgのみとなっています。  
将来的には切り替えられるようにする予定ですが、ご自身でXPathを調べてPayloadファイルをtxt2img用に書き換えると現在のバージョンでも使えるはず……です。
詳しくは[ガイド.Payloadフォーマット]の欄もご覧ください。

<br>

**Q**　Ctrl+↓↑のウェイト調整は可能ですか？　※例: {prompt:1.2} のような記法

**A**　あります。SDWebUIと違いKaynoではコンマ区切りで単語を自動認識するので、波カッコ{}がない状態でもキーワードを選択する必要はありません。

<br>


**Q**　プロンプトのタグ補完機能はありますか？

**A**　デフォルトでは all-in-one のyamlファイルに対応しています。
プロンプトエリアで文字を入力し始めると、該当するタグが表示されます。
Tabキーで自動で一番上の候補を送信します。↓↑で選択もできます。

<br>


**Q**　SDWebUIに拡張機能(Adetailer, Dynamic CFG など)を入れていて、それもKaynoから使いたいのですが、どうすればいいですか？

**A**　ペイロード(payload)に行を追加してください。
デフォルトではDynamic Thresholding (CFG Scale Fix)が追加されているので、それを参考にしてみてください。
詳しくは[ガイド.Payloadフォーマット]の欄もご覧ください。

<br>


**Q**　Payloadをカスタムして、通常の処理以外にimg2img以外のタブを選択して処理をすることはできますか？
(ExtrasのGFPGANで超解像するなど)

**A**　可能です。詳しくは[ガイド.Payloadフォーマット]をご覧ください。

<br>


**Q**　modelsやOutputフォルダを分けているのですが、対応していますか？

**A**　基本的にStable Diffusion WebUIのフォルダを参照しているので、フォルダの整理には**ディレクトリジャンクション**をお使いください。(シンボリックリンク(/d)ではなくジャンクション(/j)をおすすめしています)

```
cd stable-diffusion-webui
mklink /j models <リンク先>
```

### トラブルシューティング
**Q**　アプリが強制終了したらなんかChromeとコマンドウィンドウが残ってしまいました。

**A**　コマンドウィンドウをCtrl+Cで閉じてしまってOKです。
(Chromeも一緒に閉じます)

<br>


**Q**　DPI混在のマルチモニター環境ですが、実際の解像度が違うような気がします。

**A**　解像度の指定 x 現在のDPI倍率 = SDWebUI上の解像度表記 なら合っています。
キャプチャがそもそもズレる、などであれば再起動やDPI倍率のモニタ間での統一をお試しください。
