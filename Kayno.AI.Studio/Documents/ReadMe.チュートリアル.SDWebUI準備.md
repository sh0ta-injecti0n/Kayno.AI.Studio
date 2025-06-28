## はじめてStableDiffusionを使う: 準備編

#### wingetの紹介
このあといろいろとインストールしますが、wingetという管理用ソフトがあると何かと便利です。wingetがあれば以降の作業がすべてコピペで済みます！
wingetは最新のWindowsなら最初から入っています。
https://learn.microsoft.com/ja-jp/windows/package-manager/winget/

#### Python, Git (winget)
PythonとGitをダウンロードします。
Pythonはプログラム環境、Gitはデータ管理用に使うソフトだと思ってください。
Windowsの検索ボックス(Windowsキー+S)からまず「コマンドプロンプト」を「管理者で」起動します。そして以下のコマンドをコピペしてください。

```
winget install -e -id Python.Python.3.10
winget install -e -id Git.Git
git lfs install
```
※3行目はこのあとHuggingFaceというサイトからまとめてダウンロードするのが楽になります

#### Stable Diffusion WebUI   
##### インストールと導入

https://github.com/AUTOMATIC1111/stable-diffusion-webui/releases/tag/v1.10.1

から「Source」のzipをダウンロードして展開します。今回はAIというフォルダを作って C:\AI\ の中に入れたものとします。
こうすると、C:\AI\stable-diffusion-webui\というパスになります。
今後これを **「sdwebuiのホーム」** と呼ぶことにします。初回インストールと起動はこのsdwebuiのホームにある

``webui-user.bat``

というファイルからできます。
初回インストールは時間がかかるので、先に必要なファイルをチェックしましょう。

##### モデル
※後半にまとめてダウンロードできるコマンドあります！

- おすすめのモデル:  
	- ダウンロードしたらホーム内の models > stable-diffusion 内に入れておきます。
	- **MiaoMiao Harem** 	 
		- #SDXL #キャラ #2次元～2・5次元  
		- https://civitai.com/models/934764?modelVersionId=1468334

- VAE  
	- 補完的なモデルとしてSDXL系のモデルには専用のVAEモデルが必要になります。以下のURLからダウンロードします。
	- ダウンロードしたらホーム内の models > vae 内に入れておきます。
	- **sdxl_vae**
		- https://huggingface.co/stabilityai/sdxl-vae/tree/main

- ControlNet用モデル
	- ControlNetという後述する拡張機能で使用します。いくつかありますが、今から使うならSDXL系モデルで使えるものがおすすめです。数が多いのでコマンドを使いましょう！使う際はControlnetの設定でEnding Stepを1.0から下げるのがおすすめです。
	- **「青龍聖者@bdsqlsz」系**  
		- https://huggingface.co/bdsqlsz/qinglong_controlnet-lllite/tree/main
	- **kataragi系**
		- https://civitai.com/models/136070?modelVersionId=520935
		- https://huggingface.co/kataragi/ControlNet-LineartXL
	- ※Controlnetまとめのページはこちら
		- https://civitai.com/models/136070?modelVersionId=520935

- 備考: 古いモデルを使う場合
	- モデル
		- SD1.5
		- SD2.0
	- VAE
		- ema系

	*...古いモデルは整備中...*


- 拡張機能のインストール
	- WebUIのインストールが終わったら拡張機能を入れます。正規の手段ではExtensionsからAvailableタブに行って、「Load From」ボタンを押します。ちょっとわかりづらいですがここが検索ボックスになっているので、ここからお目当ての拡張機能を入れる形が正攻法です。
	- ControlNet   
	- Dynamic CFG  
	- (↓はオプションで)  
	- prompt-all-in-one  
	- tag-autocomplete  
	- infinite-image-browsing  
	- civitai helper  


**ControlNet系はまとめてこちらのコマンドでダウンロードできます！** 

```
pushd c:\ai\stable-diffusion-webui-master\models\controlnet\
git clone https://huggingface.co/bdsqlsz/qinglong_controlnet-lllite
git clone https://huggingface.co/kataragi/ControlNet-LineartXL
```

```
pushd c:\ai\stable-diffusion-webui-master\extensions\
git clone https://github.com/Mikubill/sd-webui-controlnet.git
git clone https://github.com/mcmonkeyprojects/sd-dynamic-thresholding.git
```