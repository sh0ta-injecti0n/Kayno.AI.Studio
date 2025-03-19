using Kayno.AI.Studio.Functions.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kayno.AI.Studio
{
	public class WebSenderSelenium
	{

		public IWebDriver webdriver;


		IWebElement tab_img2img;
		string tab_img2img_XPath = "";
		int progress = 0;

		IWebElement button_Generate;
		string GenerateButtonXPath = "";

		IWebElement image_img2img;
		string image_img2img_FullPath = "";

		public WebSenderSelenium() 
		{
		
		}

		public async Task InitWebData(string url)
		{
			url = url ?? "http://localhost:7860";

			webdriver = new ChromeDriver();
			await webdriver.Navigate().GoToUrlAsync(url);
		}

		public async void SendWebData(ObservableCollection<Payload> payloads)
		{
			var url = payloads.FirstOrDefault(i => i.PropertyName is "url").PropertyValue.ToString();
			url = url.EndsWith(@"/") ? url : url + @"/";
			if (webdriver == null)
			{
				webdriver = new ChromeDriver();
				await webdriver.Navigate().GoToUrlAsync(url);
			}

			if (!webdriver.Url.Equals(url))
			{
				Debug.WriteLine(webdriver.Url.ToString());

				// ↓もしPayloadを切り替えて複数URLで動かしたいとき用に、新しいタブで開く

                ( (IJavaScriptExecutor)webdriver ).ExecuteScript( "window.open();" );
                var windows = webdriver.WindowHandles;
                webdriver.SwitchTo().Window( windows.Last() );
                //( (IJavaScriptExecutor)webdriver ).ExecuteScript( "window.open('about:blank', '_blank');" );
				// about:blank 指定するか、あるいはwindow.open()してSwitchTo~~Last()するかのどっちか
                webdriver.Navigate().GoToUrl(url);
				// asyncだと先に↓以降が走っちゃうので。

				WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(15));
				//var result = wait.Until(drv => drv.FindElement(By.XPath(XPath)).Displayed);
				try
				{
					wait.Until(drv =>
					{
						try
						{
							var element = drv.FindElement(By.XPath("//*[@id=\"setting_sd_model_checkpoint\"]"));
							return element.Displayed ? element : null;
						}
						catch (NoSuchElementException)
						{
							// 要素が見つからない場合はnullを返し、待機を続ける
							return null;
						}
					});
				}
				catch (WebDriverTimeoutException)
				{
					// タイムアウトした場合の処理
					//throw new NoSuchElementException($"SDWebUI Timeout");
				}
				// せっかくwaitやってるのにタイムアウトを待たずにException出しちゃうのでそれはCatchしないこと
				
			}

			var selectors = payloads.Where
				(
					i => 
					i.WebSelectorKey != null 
					&& i.WebSelectorKey != SeleniumBySelector.None
					&& !string.IsNullOrEmpty(i.WebSelectorValue)
				)
				.ToList();
			// WebSelector なし を省く (アプリ独自設定など)


			for (var i = 0; i < selectors.Count; i++) 
			{
				var item = selectors[i];

				var XPath = item.WebSelectorValue.Replace( "\"\"", "\"" ).TrimStart( '"' ).TrimEnd( '"' );
				item.WebSelectorValue = XPath;
				// エスケープ必須

				var itemValueString = item.PropertyValue?.ToString();

				IWebElement ele = null;
				
				try
				{
					ele = webdriver.FindElement( By.XPath( XPath ) );
					//var x = ele.Enabled;
					//var x2 = ele.Displayed;

					( (IJavaScriptExecutor)webdriver ).ExecuteScript( "arguments[0].scrollIntoView(true);", ele );
					// アコーディオンパネルは画面外だと反応しないのでスクロール

					switch ( item.UI)
					{
						case UISelector.Button:
							if (item.PropertyName is "tab_img2img" )
							{
								tab_img2img_XPath = XPath;
								tab_img2img = ele;
							}

							if ( item.PropertyName == "button_generate" )
							{
								GenerateButtonXPath = XPath;
								button_Generate = ele;
								continue;

								// Generate ボタンはまだ押さない
							}

							ele.Click();
							//var actions1 = new Actions( webdriver );
							//actions1.MoveToElement( ele ).Click().Perform();
							break;

						case UISelector.CheckBox:
							var IsCheckedNow = ele.Selected;
							var toBeChecked = bool.Parse( itemValueString );

							if (toBeChecked != IsCheckedNow)
							{
								var actions = new Actions(webdriver);
								actions.MoveToElement( ele ).Click().Perform();
								// ele.Click() ではいかないことアリ
							}
							break;

						case UISelector.DropArea:
							//Thread.Sleep( 3000 );
							var fullpath = itemValueString;
							if ( !File.Exists( itemValueString) )
							{
								var filename = Path.Combine( MainWindow.CurrentPayloadDir, itemValueString );
								fullpath = Path.GetFullPath( filename );
							}
							image_img2img = ele;
							image_img2img_FullPath = fullpath;
							SendImage();
							// 疑似DnD (GradioはDnDを実装するために通常のinputの仕様を塞いでしまっているので)
							break;

						case UISelector.TextBox:

							var jsonEncodedString = JsonSerializer.Serialize( itemValueString );
							jsonEncodedString = jsonEncodedString.Trim( '"' ).TrimEnd( ',' );

                            //itemValueString = itemValueString.Replace( "\r\n", @"\r\n" );
                            //itemValueString = itemValueString
                            //	.Replace( "\\", "\\\\" ) // バックスラッシュのエスケープ
                            //	.Replace( "'", "\\'" )   // シングルクォートのエスケープ
                            //	.Replace( "\"", "\\\"" ); // ダブルクォートのエスケープ (必要に応じて)

                            // 改行があると↓でエラーになるので文字に変換
                            ( (IJavaScriptExecutor)webdriver ).ExecuteScript( "arguments[0].value = '';", ele );
							( (IJavaScriptExecutor)webdriver ).ExecuteScript( "arguments[0].value ='"+ jsonEncodedString + "';", ele );
							// 既存の値をクリアしてから代入
							//
							//( (IJavaScriptExecutor)webdriver ).ExecuteScript
							//( @"
							//		arguments[0].dispatchEvent(new Event('input', { bubbles: true }));
							//		arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
							//", ele 
							//);
							// `input`と`change`イベントを発火
							// Sendkeysではprompt-all-in-oneが反応して、タグ候補をクリックしてしまうことがあるので、
							// JSで疑似化するとよい

							//ele.Clear();
							//ele.SendKeys( itemValueString );
							ele.SendKeys( Keys.Escape );
							// フォーカスを外す

							break;

						case UISelector.List or UISelector.SplitButton:
							Debug.WriteLine( "ListValue" + item.PropertyName );
							Debug.WriteLine( "ListValue" + item.PropertyValue );

							var isfieldsets = XPath.Contains( @"label[1]" );
							// ボタンタイプのfieldsetsかどうか　→　labelでIndex操作可能

							if (isfieldsets)
							{
								var index = int.Parse(item.PropertyValue.ToString());
								XPath = XPath.Replace( @"label[1]", $"label[{index}]" );
								ele = webdriver.FindElement( By.XPath( XPath ) );
								ele.Click();

								// label[?] をインデックス指定。
								// 1スタートなので PropertyValue のインデックスはtsv側で繰り上げること
							}
							else
							{
								ele.Clear();

								var list = item.UI_ItemsSource.ToList<PayloadTemplate>();
								var val = list[ int.Parse(itemValueString) ].TLabel;
								ele.SendKeys( val.ToString() );
								
								Actions actions = new Actions( webdriver );
								actions.MoveToElement( ele ).SendKeys( Keys.Enter ).Perform();
								// 仮想的にリストから選択してEnterで補完決定する操作
							}
							break;

						default:
							ele.Clear();
							ele.SendKeys( item.PropertyValue.ToString() );
							// 一度クリアしないと値が追加で入ってしまうので注意
							break;
					}

				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					Debug.WriteLine( "↑@" + item.PropertyName + " " + itemValueString );
				}


				// 本当は XPath の固定指定も item.WebSelectorKey に応じさせるのがベストだが、
				// 今後きっといつか対応するかもしれない可能性はなくはないはず

			}


		}

		public void SendImage()
		{
			try
			{
				WebSenderSelenium_JSDropFile.DropFile( image_img2img, image_img2img_FullPath );
			}
			catch
			{

			}

		}

		public void SendGenerateCommand()
		{
			System.Threading.Thread.Sleep( 1000 );
			// 一定時間待機（デバッグ用）

			try
			{
				//var btn = webdriver.FindElement( By.XPath( GenerateButtonXPath ) );
				//( (IJavaScriptExecutor)webdriver ).ExecuteScript( "arguments[0].scrollIntoView(true);", btn );

				Debug.WriteLine( tab_img2img_XPath );
                Debug.WriteLine( GenerateButtonXPath );
				tab_img2img = webdriver.FindElement( By.XPath( tab_img2img_XPath ) );
				button_Generate = webdriver.FindElement( By.XPath( GenerateButtonXPath ) );
				// なぜかすでに代入したのに参照できないことがあり、再取得

				var actions = new Actions( webdriver );
				actions.MoveToElement( tab_img2img ).Click().Perform();
				// 先にタブアクティブ化しておく

				actions.MoveToElement( button_Generate ).Click().Perform();

				//Debug.WriteLine( "???" + button_Generate.Enabled );

            }
            catch (Exception ex)
			{

			}

			// 最後に画像生成ボタンをクリック
		}

		public void QuitSelenium()
		{
			if (webdriver != null)
			{
				webdriver?.Quit();
				webdriver?.Dispose();
			}
		}




	}















}
