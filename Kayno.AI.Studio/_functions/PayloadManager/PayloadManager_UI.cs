using Microsoft.Win32;
using OpenQA.Selenium;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
	{

		#region ## Preferences

		public static string KaynoDataDirectory
		{
			get
			{
				return Path.GetFullPath( AppSettings.Instance.Pref_Main_PrefPayload_Data );
            }
		}

		public static string KaynoDataGlobalDirectory
		{
			get
			{
				return Path.GetFullPath( AppSettings.Instance.Pref_Main_PrefPayload_DataGlobal );
            }
        }

		public static string CurrentPayloadDir
		{
			get;
			set;
		} = Path.Combine( KaynoDataDirectory, "000" );

        public static string CurrentPayloadPath
		{
			get
			{
				var path = Path.Combine( CurrentPayloadDir, AppSettings.Instance.Pref_Main_PrefPayload_FileName );
				return path;
			}
		}

		public ObservableCollection<Payload> CurrentPayloadCollection
		{
			get; set;
		}

		ObservableCollection<Payload> payloads_All
		{
			get
			{
				return CurrentPayloadCollection;
			}
		}

		ObservableCollection<Payload> payloads_PinnedOnly
		{
			get
			{
				var ls = CurrentPayloadCollection.Where( item => item.UI_IsPinned == true );
				return new ObservableCollection<Payload>( ls );
			}
		}

		public ObservableCollection<PayloadTemplate> ModelCollection
		{
			get; set;
		}

		public ObservableCollection<PayloadTemplate> PromptTagCollection
		{
			get; set;
		}

		#endregion


		/// <summary>
		/// モデルのリストを更新し、ペイロード(SDWebUIへ送信する設定データ)を読み込みます。
		/// </summary>
		public async Task LoadPayload()
		{
			ResetModelSourceFiles();
			ResetPromptSourceFiles();
			//先にこの2つを await なしで読み込むこと。非同期でComboBoxが空のまま読まれることを防止

			await Task.Run
			( 
				() =>
				{
					
					var loadedPayloads = TsvSerializer.LoadFromTsv<Payload>(CurrentPayloadPath);
					CurrentPayloadCollection = new ObservableCollection<Payload>(loadedPayloads);

					//CurrentPayloadCollection = new ObservableCollection<Payload>();
					//
					//ResetModelSourceFiles();
					//ResetPromptSourceFiles();
					//CurrentPayloadCollection = TsvSerializer.LoadFromTsv<Payload>( CurrentPayloadPath );
				}
			);
			
			listView_PinnedPane.DataContext = CurrentPayloadCollection;

		}

		public async void SavePayload()
		{

            var msg_save = Properties.Resources.Dialog_ComfirmPresetSave;
            var msgBox = MessageBox.Show( msg_save, "", MessageBoxButton.YesNo );
            if ( msgBox == MessageBoxResult.Yes )
            {
                TsvSerializer.SaveToTsv<Payload>( CurrentPayloadPath, CurrentPayloadCollection );
            }
            
			//TsvSerializer.SaveToTsv<Payload>( CurrentPayloadPath, CurrentPayloadCollection );
            //await Json_ClassToJson(CurrentPayloadCollection, CurrentPayloadPath);
		}


		/// <summary>
		/// 指定したコントロールに Payload から設定用UIを自動で表示します。
		/// </summary>
		/// <param name="listView"></param>
		/// <param name="payloads"></param>
		public void BuildUIFromPayload(ListView listView, ObservableCollection<Payload> payloads)
		{

			// 一時的に高さを固定しておく
			double previousHeight = listView.ActualHeight;
			listView.Height = previousHeight;

			// 既存のUIをクリア
			listView.Items.Clear();

			var itemMargin = new Thickness(16, 8, 16, 0);

			// Payloadごとに動的にUIを作成
			foreach (var payload in payloads)
			{
				Debug.WriteLine("Payload: " + payload.PropertyName);

				if (payload.UI_IsVisible == false) 
				{ 
					continue;
					// Visible が false 指定ならUI作成をスキップ
					// ⚠ payloads.Where ~~ として直接コレクションアイテムごとスキップすると、
					// 抜かれてTSVに返ってくることになるのでやらないこと。
					// payloadのコレクションにはいてほしいので、UI作成プロセスだけスキップしてTSVには居てもらう
				}

				// 一行のUIコンテナ
				var grid = new Grid();
				grid.Margin = itemMargin;

				// Gridの列を設定（4:6の比率）
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) });

				var NoLabel = false;
				if (payload.PropertyName.Contains("prompt"))
				{
					NoLabel = true;
					// prompt だけスパンして表示
				}

				var label = _createLabel(grid, NoLabel, payload);
				var control =	_createControl(grid, NoLabel, payload);
				//var promptHelper =	_createPromptHelper(grid, NoLabel, payload);
                var pinButton = _createPinButton(grid, payload);
			
				// 完成したUIをListViewに追加
				listView.Items.Add(grid);
			}

			var rect = new Border { Height = 100 };
			listView.Items.Add( rect );
			// AppBar オフセット用

			AnimatePanel(listView, previousHeight);
		}

        void AnimatePanel(ListView listView, double previousHeight)
		{

			// レイアウト更新が完了するタイミングで新しい高さを取得してアニメーション開始
			listView.Dispatcher.BeginInvoke(new Action(() =>
			{
				// 一時的に Height プロパティに値を設定
				listView.Height = previousHeight;

				double newHeight = getListViewItemsHeight(listView);

				DoubleAnimation heightAnimation = new DoubleAnimation
				{
					From = previousHeight,
					To = newHeight,
					Duration = TimeSpan.FromMilliseconds(300),
					EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
				};

				// アニメーション完了後に Auto に戻す処理も追加可能
				heightAnimation.Completed += (s, e) =>
				{
					listView.BeginAnimation(FrameworkElement.HeightProperty, null);
					listView.Height = Double.NaN; // Height=Auto を意味する
				};

				listView.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
				Debug.WriteLine("Animated: {0} > {1}", previousHeight, newHeight);

			}), System.Windows.Threading.DispatcherPriority.Loaded);


		}

		double getListViewItemsHeight(ListView listView)
		{
			double height = 0;

			foreach (var item in listView.Items)
			{
				ListViewItem listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(item);
				if (listViewItem != null)
				{
					height += listViewItem.ActualHeight;
				}
			}

			return height;
		}

		UIElement _createLabel(Grid grid, bool NoLabel, Payload payload)
		{
			if (NoLabel)
			{
				return null;
			}

			// PropertyName（ラベル）を表示
			var labelTextBlock = new TextBlock
			{
				Text = payload.Label ?? payload.PropertyName,
				Style = FindResource("TextBlock1") as Style,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Center,
			};
			Grid.SetColumn(labelTextBlock, 0);
			grid.Children.Add(labelTextBlock);

			return labelTextBlock;
		}

		UIElement _createControl(Grid grid, bool NoLabel, Payload payload)
		{
			// UISelectorを見て適切なコントロールを生成
			UIElement control = null;
			grid.Height = NoLabel ? 92 : 32;
			object tooltipContent = payload.Comments;

			switch (payload.UI)
			{
				// Bindingするにしても Text とか Value とか初期化しておくこと。
				// UIに表示されずバインディングされない場合、値が割り当てられない。

				// イベント・コマンド:
				// あらかじめCMD.csでコマンドを定義しておけばバインディング可能
				//	( (Button)control ).Command = CMD_Test;
				//control.SetValue(Button.CommandProperty, payload.PropertyValue);
				//BindingOperations.SetBinding(control, Button.CommandProperty, new Binding(nameof(Payload.PropertyValue)));

				case UISelector.TextBlock:
					control = new TextBlock
					{
						Text = payload.PropertyValue?.ToString(),
						Style = FindResource("TextBlock1") as Style,
						HorizontalAlignment = HorizontalAlignment.Right,
						VerticalAlignment = VerticalAlignment.Center,
						DataContext = payload,
					};
					BindingOperations.SetBinding(control, TextBlock.TextProperty, new Binding(nameof(Payload.PropertyValue)));
			break;

				case UISelector.TextBox:
					control = new Grid();
					// PromptHelper 用にグリッド用意

					var control_tb = new TextBox
					{
						Text = payload.PropertyValue?.ToString(),
						Style = FindResource( "TextBox1" ) as Style,
						Tag = payload.Tag,
						Height = NoLabel ? 92 : 32,
						Padding = NoLabel ? new Thickness( 18, 4, 18, 4 ) : new Thickness( 8, 0, 8, 0 ),
						AcceptsReturn = NoLabel ? true : false,
						ToolTip = payload.Comments,
						AllowDrop = true,
						DataContext = payload,
					};
                    ( (Grid)control ).Children.Add( control_tb );

                    BindingOperations.SetBinding( control_tb, TextBox.TextProperty, new Binding(nameof(Payload.PropertyValue)));

					control_tb.PreviewDragOver += ( s, e ) =>
					{
                        if ( e.Data.GetDataPresent( System.Windows.DataFormats.FileDrop, true ) )
                        {
                            e.Effects = System.Windows.DragDropEffects.Copy;
                        }
                        else
                        {
                            e.Effects = System.Windows.DragDropEffects.None;
                        }
                        e.Handled = true;
                    };
					control_tb.Drop += ( s, e ) =>
					{
						var textBox = (TextBox)s;
                        var dropFiles = e.Data.GetData( System.Windows.DataFormats.FileDrop ) as string[];
                        if ( dropFiles == null ) return;
                        textBox.Text = dropFiles[ 0 ];
                    };


                    #region ## For PromptHelper
                    if (payload.PropertyName.Contains("prompt"))
					{
						AttachPromptTextBox(payload, control, control_tb);
					}


					#endregion

					break;

				case UISelector.Slider:
					control = new Slider
					{
						Minimum = payload.UI_SliderMinVal ?? 0,
						Maximum = payload.UI_SliderMaxVal ?? 100,
						TickFrequency = payload.UI_SliderSnapValue ?? 0,
						//Value = Math.Round( Convert.ToDouble(payload.PropertyValue ?? 0 ), 3),
						// 0.1+0.2問題のため必ず丸めること
						Style = FindResource("Slider1") as Style,
						DataContext = payload,
						
					};
					var binding_slider = new Binding( nameof( Payload.PropertyValue ) )
					{
						Mode = BindingMode.TwoWay,
						UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
						Converter = new MathRoundConverter()
					};
					BindingOperations.SetBinding( control, Slider.ValueProperty, binding_slider );

					break;

				case UISelector.CheckBox:
					control = new CheckBox
					{
						Content = payload.Label ?? payload.PropertyName,
						IsChecked = bool.Parse( payload.PropertyValue.ToString() ),
						VerticalAlignment = VerticalAlignment.Center,
						Style = FindResource("CheckBox1") as Style,
						DataContext = payload,
					};
					BindingOperations.SetBinding(control, CheckBox.IsCheckedProperty, new Binding(nameof(Payload.PropertyValue)));
					break;

				case UISelector.Button:
					control = new Button
					{
						Content = payload.Label ?? payload.PropertyName,
						VerticalAlignment = VerticalAlignment.Center,
						Style = FindResource("ButtonNormal") as Style,
						DataContext = payload,
					};
					( (Button)control ).Command = CMD_Test;
					//control.SetValue(Button.CommandProperty, payload.Command);
					//BindingOperations.SetBinding(control, Button.CommandProperty, new Binding(nameof(Payload.Command)));
					break;

				case UISelector.List:
					control = new ComboBox
					{
						Style = FindResource("ComboBox1") as Style,
						DisplayMemberPath = "TLabel",
						SelectedValuePath = "TPropertyValue",
						MaxDropDownHeight = 400,
						Tag = payload.PropertyName,
						DataContext = payload,
					};
					BindingOperations.SetBinding(control, ComboBox.ItemsSourceProperty, new Binding(nameof(Payload.UI_ItemsSource)));
					BindingOperations.SetBinding(control, ComboBox.SelectedIndexProperty, new Binding(nameof(Payload.PropertyValue)));
					( (ComboBox)control ).SelectionChanged += MainWindow_ComboBoxSelectionChanged;


					break;

				case UISelector.SplitButton:
					control = new ListView
					{
						//SelectedIndex = int.Parse(payload.PropertyValue.ToString()),
						Style = FindResource("ListViewSplitButton") as Style,
						DisplayMemberPath = "TLabel",
						SelectedValuePath = "TPropertyValue",
						//ItemsSource = payload.UI_ItemsSource,
						DataContext = payload,
					};
					BindingOperations.SetBinding(control, ListView.ItemsSourceProperty, new Binding(nameof(Payload.UI_ItemsSource)));
					BindingOperations.SetBinding(control, ListView.SelectedIndexProperty, new Binding(nameof(Payload.PropertyValue)));

					break;

				case UISelector.Expander:
					control = new Expander
					{
						Style = FindResource("Expander1") as Style,
						DataContext = payload,
					};
					break;

				case UISelector.DropArea:

					var control1 = new Image
					{
						//Style = FindResource("Image") as Style,
						DataContext = payload,
						Width = 40,
						Height = 40,
						MaxHeight = 64,
					};
					var binding = new Binding( nameof( Payload.PropertyValue ) );
					binding.Converter = new FilePathToBitmapImageConverter();
					binding.ConverterParameter = CurrentPayloadDir;
					BindingOperations.SetBinding( control1, Image.SourceProperty, binding );

					var control2 = new TextBox
					{
						Style = FindResource( "TextBox1" ) as Style,
						DataContext = payload,
					};
					BindingOperations.SetBinding( control2, TextBox.TextProperty, new Binding( nameof( Payload.PropertyValue ) ) );

					control = new DockPanel
					{
						AllowDrop = true,
						Children = { control1, control2 }
					};
					DockPanel.SetDock( control1, Dock.Left );
				
					// ファイル変更監視を開始
					//targetImage = control as Image;
					//StartWatchingFile( payload.PropertyValue, control );

					control.PreviewDragOver += (s, e) =>
					{
						if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
						{
							e.Effects = System.Windows.DragDropEffects.Copy;
						}
						else
						{
							e.Effects = System.Windows.DragDropEffects.None;
						}
						e.Handled = true;
					};
					control.Drop += (s, e) =>
					{
						var dropFiles = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
						if (dropFiles == null) return;
						var path = dropFiles[0];
						if (!File.Exists(path)) return;
						payload.PropertyValue = path;
						//((Image)control).Source = new BitmapImage(new Uri($"{path}"));
					};
					// DropAreaのテキストボックスにドロップされるとファイルパスを更新する


					break;

				// 他のUISelectorケースも追加可能
				default:
					break;
			}


			// コントロールをGridの2列目に配置
			if (control != null)
			{
				grid.ToolTip = tooltipContent;

				if ( NoLabel)
				{
					Grid.SetColumn(control, 0);
					Grid.SetColumnSpan(control, 2);
					grid.Children.Add(control);
				}
				else
				{
					Grid.SetColumn(control, 1);
					grid.Children.Add(control);
				}

			}

			return control;
		}

		private void AttachPromptTextBox(Payload payload, UIElement control, TextBox control_tb)
		{
			var tb_popup = new Popup();
			tb_popup.Placement = PlacementMode.Bottom;
			tb_popup.IsOpen = false;
			tb_popup.StaysOpen = false;
			tb_popup.AllowsTransparency = true;

			var tb_popup_pane = new ListView();
			tb_popup_pane.Style = FindResource("ListView1") as Style;
			tb_popup_pane.Background = FindResource("GradientControl") as LinearGradientBrush;
			tb_popup_pane.ItemContainerStyle = FindResource("ListCommonItem1") as Style;
			tb_popup_pane.DataContext = payload.UI_ItemsSource;
			tb_popup_pane.ItemsSource = payload.UI_ItemsSource;
			//var PayloadTemplates = (ObservableCollection<PayloadTemplate>)DataContext;
			tb_popup_pane.Width = 368;
			tb_popup_pane.Height = 428;


			string keyword = "";
			int LastCaretIndex = -1;
			int LastCount = 0;
			var ClosePane = new Action(() =>
			{
				LastCaretIndex = -1;
				tb_popup_pane.SelectedIndex = -1;
				tb_popup.IsOpen = false;
			});


			#region ## AutoComplete


			var SendAutoComplete = new Action<RoutedEventArgs>((RoutedEventArgs e) =>
			{
				var item = tb_popup_pane.SelectedItem as PayloadTemplate;
				item = item ?? tb_popup_pane.Items[0] as PayloadTemplate;
				if (item != null)
				{
					var val = item.TPropertyValue as string;
					control_tb.Select(LastCaretIndex, LastCount);
					control_tb.SelectedText = val;
				}
				LastCaretIndex = -1;
				LastCount = 0;
				keyword = "";
				tb_popup_pane.SelectedIndex = -1;
				tb_popup.IsOpen = false;
				control_tb.Focus();
				e.Handled = true;
				return;
			});

			control_tb.PreviewKeyDown += (s, e) =>
			{
				//var control_tb = (TextBox)s;
				//var selected = textbox.SelectedText;
				//if ( string.IsNullOrEmpty( selected ) ) return;

				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					// 上矢印キーまたは下矢印キーが押されたかチェック
					if (e.Key == Key.Up || e.Key == Key.Down)
					{
						//ReplaceKeywordWithUpdatedValue(control_tb, e.Key == Key.Up);
						PromptWeightController(control_tb, e.Key == Key.Up);
						e.Handled = true;  // イベントの伝播を止める
						return;
					}
					// Ctrl+↓↑によるWeight調整

				}

				if (e.Key == Key.OemComma || e.Key == Key.Space || e.Key == Key.Enter || e.Key == Key.Escape)
				{
					LastCaretIndex = -1;
					LastCount = 0;
					keyword = "";
					tb_popup.IsOpen = false;
					return;
				}

				if (e.Key == Key.Back || e.Key == Key.Delete)
				{
					LastCount -= 1;
					if (LastCount < 1)
					{
						LastCaretIndex = -1;
						LastCount = 0;
						keyword = "";
						tb_popup.IsOpen = false;
					}
				}

				if (tb_popup.IsOpen)
				{
					if (e.Key == Key.Escape)
					{
						LastCaretIndex = -1;
						LastCount = 0;
						keyword = "";
						tb_popup.IsOpen = false;
						e.Handled = true;
						return;
					}

					if (e.Key == Key.Tab)
					{
						SendAutoComplete(e);
						e.Handled = true;
						return;
						//var item = tb_popup_pane.SelectedItem as PayloadTemplate;
						//item = item ?? tb_popup_pane.Items[ 0 ] as PayloadTemplate;
						//if ( item != null )
						//{
						//    var val = item.TPropertyValue as string;
						//	control_tb.Select( LastCaretIndex, LastCount );
						//    control_tb.SelectedText = val;
						//}
						//e.Handled = true;
						//LastCaretIndex = -1;
						//LastCount = 0;
						//keyword = "";
						//tb_popup.IsOpen = false;
						//return;
					}

					if (e.Key == Key.Up || e.Key == Key.Down)
					{
						tb_popup_pane.Focus();
						//tb_popup_pane.SelectedIndex = e.Key == Key.Up 
						//? 
						//tb_popup_pane.SelectedIndex - 1
						//:
						//tb_popup_pane.SelectedIndex + 1;
						e.Handled = true;  // イベントの伝播を止める
						return;
						// 候補表示を選択する。textboxのキャレット移動をキャンセル
					}
				}

				// Ctrl+↑↓は標準の操作とかぶるので、Previewのほうを使ってe.Handled=trueで動作を上書き
				// BackspaceやスペースなどはKeyDownでは取れない。
				// Previewを使う。
			};

			control_tb.KeyUp += (s, e) =>
			{
				if (
				(e.Key >= Key.A && e.Key <= Key.Z)
				|| (e.Key >= Key.D0 && e.Key <= Key.D9)
				|| (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
				 )
				{
					if (
						Keyboard.IsKeyDown(Key.LeftCtrl)
						|| Keyboard.IsKeyDown(Key.LeftAlt)
						)
					{
						return;
					}
						if (LastCaretIndex < 0)
					{
						LastCaretIndex = control_tb.CaretIndex - 1;
					}

					LastCount += 1;


					// 候補表示
				}

				if (LastCount > 0)
				{
					try
					{
						keyword = control_tb.Text.Substring(LastCaretIndex, LastCount);
						Debug.WriteLine(keyword);

						if (string.IsNullOrEmpty(keyword) || keyword.Equals(Environment.NewLine))
						{
							return;
						}

						var items =
						payload.UI_ItemsSource.Where
						(
							i =>
							i.TPropertyValue.ToString().ToLower().Contains(keyword)
						//i.TLabel.ToLower().Contains( keyword )
						//|| i.TCategory.Contains( keyword )
						//|| i.TCategory2.Contains( keyword )
						//|| i.TTags.ToString().Contains( keyword )

						).ToList();

						//tb_popup_pane.DataContext = items;
						tb_popup_pane.ItemsSource = items ?? new List<PayloadTemplate>();

						tb_popup.IsOpen = true;
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.Message.ToString());
					}
					// ⚠ タイピングが早すぎるとダメなときがある
				}
			};

			tb_popup_pane.PreviewKeyDown += (s, e) =>
			{
				if (e.Key == Key.Escape) 
				{ 
					ClosePane();
				}

				if (tb_popup.IsOpen)
				{
					if (e.Key == Key.Enter || e.Key == Key.Tab)
					{
						SendAutoComplete(e);
						return;
					}
				}

			};

			tb_popup_pane.PreviewMouseLeftButtonUp += (s, e) =>
			{
				if (tb_popup.IsOpen && tb_popup_pane.SelectedIndex > -1)
				{
					SendAutoComplete(e);
					return;
				}
				// Up... DownだとまだSelectedがNull
			};

			#endregion

			// AutoComplete ちょっと(やることが)重すぎて無効化

			/*
			
			control_tb.PreviewTextInput += (s, e) =>
			{
				previousCaretIndex = control_tb.CaretIndex;
			};

			control_tb.TextChanged += (s, e) =>
			{
				
				tb_popup.IsOpen = true;
			};

			tb_popup_pane.KeyDown += (s, e) =>
			{
				if (e.Key == Key.Escape)
				{
					ClosePane();

					e.Handled = true;
				}
			};

			tb_popup_pane.SelectionChanged += (s, e) =>
			{
				if (!tb_popup.IsOpen)
				{
					return;
				}

				if (tb_popup_pane != null)
				{
					if (tb_popup_pane.SelectedItem != null)
					{
						string selectedText = tb_popup_pane.SelectedItem.ToString();
						int currentCaretIndex = control_tb.CaretIndex;
						// 現在のカーソル位置を取得

						// 置換範囲を計算
						int startIndex = Math.Min(previousCaretIndex, currentCaretIndex);
						int length = Math.Abs(previousCaretIndex - currentCaretIndex);

						// テキストを置き換える
						control_tb.Text = control_tb.Text.Substring(0, startIndex)
						+ selectedText
						+ control_tb.Text.Substring(startIndex + length);

						control_tb.CaretIndex = startIndex + selectedText.Length;

					}
				}
				ClosePane();
			};


			 */


			//control_tb.PreviewKeyDown += (s, e) =>
			//{
			//	//var control_tb = (TextBox)s;
			//	//var selected = textbox.SelectedText;
			//	//if ( string.IsNullOrEmpty( selected ) ) return;
			//
			//	if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			//	{
			//		// 上矢印キーまたは下矢印キーが押されたかチェック
			//		if (e.Key == Key.Up || e.Key == Key.Down)
			//		{
			//			//ReplaceKeywordWithUpdatedValue(control_tb, e.Key == Key.Up);
			//			PromptWeightController(control_tb, e.Key == Key.Up);
			//			e.Handled = true;  // イベントの伝播を止める
			//			return;
			//		}
			//		// Ctrl+↓↑によるWeight調整
			//
			//	}
			//
			//
			//};


			tb_popup.Child = tb_popup_pane;
			((Grid)control).Children.Add(tb_popup);
		}


		#region ## プロンプトの{~:1.1}のやつ: Weight Controller
		private void PromptWeightController(TextBox textBox, bool IsIncreasing)
		{
			if (textBox == null) return;

			int caretIndex = textBox.CaretIndex;
			string text = textBox.Text;

			// TextをコンマとBREAKで分割
			string[] parts = SplitTextIntoParts(text);
			// 1cat, your keywo|rd set, BREAK, test test, test...
			// { "1cat", " {your keyword set:1.2}", " ", "", " test test", " test" ...  }

			string extractedText = FindPartByCaretIndex(parts, text, caretIndex);
			// CaretIndexが含まれる要素を特定
			// , {your keywo|rd set: 1.2} BREAK
			//  ^^^^^^^^^^^^^^^^^^^^^^^^^^ 

			extractedText = extractedText.Trim();

			if (string.IsNullOrEmpty(extractedText)) return;

			string keywordSet = ExtractText(extractedText);
			// 空白を含むアルファベットのみを抽出
			// your keyword set

			var currentValue = ParseCurrentValue(extractedText);
			// 既存の書式を解析して数字部分を取得
			// 1.2 

			// 数字の上げ下げ
			var newValue = AdjustValue(currentValue, IsIncreasing);
			// 1.2 -> 1.3 (or 1.1)

			// {keyword_set:newValue}という書式にする
			string result = $"{{{keywordSet}:{newValue}}}";
			// {your keyword set:1.x}

			textBox.Text = textBox.Text.Replace(extractedText, result);
			textBox.CaretIndex = caretIndex;

		}

		private string[] SplitTextIntoParts(string text)
		{
			// コンマとBREAKで分割
			string pattern = @",|BREAK";
			return Regex.Split(text, pattern)
			//.Select(part => part.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray()
			;
		}

		private string FindPartByCaretIndex(string[] parts, string originalText, int caretIndex)
		{
			int currentIndex = 0;
			foreach (string part in parts)
			{
				int partLength = part.Length;
				if (caretIndex >= currentIndex && caretIndex <= currentIndex + partLength)
				{
					return part;
				}

				// 区切り文字の長さを考慮してcurrentIndexを更新
				if (originalText.Substring(currentIndex + partLength).StartsWith(",") ||
					originalText.Substring(currentIndex + partLength).StartsWith("BREAK"))
				{
					int delimiterLength = originalText.Substring(currentIndex + partLength).StartsWith("BREAK") ? 5 : 1;
					currentIndex += partLength + delimiterLength;
				}
				else
				{
					currentIndex += partLength;
				}
				//currentIndex += partLength + 1; // 分割された部分の間にはコンマまたはBREAKがあるため+1
			}
			return string.Empty;
		}


		private string ExtractText(string text)
		{
			// {key:value}形式からkey部分を抽出
			Match match = Regex.Match(text, @"(.*):");
			if (match.Success)
			{
				// 抽出した文字列から英数字と空白のみをフィルタリング
				return new string(match.Groups[1].Value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
			}
			else
			{
				return text;
			}

			// {と:に囲まれた部分を抽出
			//Match match = Regex.Match(text, @"\{(.*?):");
			//if (match.Success)
			//{
			//	return match.Groups[1].Value;
			//}
			//else
			//{
			//	return text;
			//}
		}


		private decimal ParseCurrentValue(string text)
		{
			// 既存の書式から数字部分を抽出
			Match match = Regex.Match(text, @"{.*?:([\d.]+)}");
			if (match.Success)
			{
				return decimal.Parse(match.Groups[1].Value);
			}

			// 既存の数字がなければデフォルト値を返す
			return 1.0m;
		}

		private decimal AdjustValue(decimal currentValue, bool IsIncreasing)
		{
			// 0.1ずつ上げ下げ
			return IsIncreasing ? currentValue + 0.1m :  currentValue - 0.1m;
		}


		// 選択中のテキストの数値を増減させて更新するメソッド
		private void ReplaceKeywordWithUpdatedValue(TextBox myTextBox, bool increase )
        {
			// 2025-03-20
			// 改行のあと,がなかったり、正規的表現でないとNG


            int caretIndex = myTextBox.CaretIndex;
            string text = myTextBox.Text;

			caretIndex = caretIndex > 0 ? caretIndex - 1 : 0;

			int start0 = text.LastIndexOf( @"\r\n" );
            // キャレット位置より前の部分から最後のカンマまたは最初までのテキストを取得
            int start = text.LastIndexOf( ',', caretIndex );
            start = start0 > start ? start0 : start; //BREAK用
			start = ( start == -1 ) ? 0 : start + 1;  // カンマがなければ最初から

            // キャレット位置より後ろの部分から最初のカンマまたは最後までのテキストを取得
            int end = text.IndexOf( ',', caretIndex );
            end = ( end == -1 ) ? text.Length : end;

			if ( start > end ) return;

            // 前後のテキストを取り出す
            var selectedText = text.Substring( start, end - start );


            // 任意の文字列（例えば"keyword"）でなくても、数値があれば処理
            string pattern = @"(.*):\s*(\d+(\.\d+)?)";  // 任意の文字列:数値の形式にマッチ
            var match = Regex.Match( selectedText, pattern );

            if ( !match.Success )  // 数値の形式でなければ"任意の文字:1.0"の形式に変換
            {
                selectedText = $"{selectedText}:1.0";
            }
            else
            {
                // "任意の文字列:数字"の形式が見つかった場合
                string numberString = match.Groups[ 2 ].Value;
                Decimal number = Decimal.Parse( numberString );

                // 数値を増減させる
                number = increase ? number + 0.1m : number - 0.1m;  // mを付けてDecimalにする

                // 新しい数値で置き換え
                selectedText = $"{match.Groups[ 1 ].Value}:{number:F1}";  // 任意の文字列:新しい数値

            }

			selectedText = selectedText.Trim();
            selectedText = @"{" + selectedText + @"}";

            Debug.WriteLine( selectedText );

            // テキストボックス内で前後のテキストを置き換える
            myTextBox.Text = text.Substring( 0, start ) + selectedText + text.Substring( end );
			myTextBox.CaretIndex = start + 1;
			// キャレット位置を更新

            //return myTextBox;
            //return selectedText;
        }
        #endregion


        UIElement _createPromptHelper( Grid grid, bool noLabel, Payload payload )
        {
			var tb_popup = new Popup();
			tb_popup.Placement = PlacementMode.Bottom;
			tb_popup.IsOpen = true;
			tb_popup.StaysOpen = true;

			var tb_popup_pane = new ItemsControl1();
			tb_popup_pane.DataContext = payload;

			tb_popup.Child = tb_popup_pane;

			grid.Children.Add( tb_popup );

			return tb_popup;
        }


        UIElement _createPinButton(Grid grid, Payload payload)
		{

			// ピン留めステータスの処理
			ToggleButton pinButton;
			pinButton = new ToggleButton
			{
				Content = (bool)payload.UI_IsPinned ? "" : "",
				IsChecked = (bool)payload.UI_IsPinned,
				Style = FindResource("ButtonIcon") as Style,
				FontSize = 12,
				Width = 24,
				Height = 24,
				Padding = new Thickness(0)
			};

			pinButton.Click += (s, e) =>
			{
				var btn = ((ToggleButton)s);
				var t = btn.IsChecked;

				btn.Content = (bool)t ? "" : "";
				// pin : unpin

				payload.UI_IsPinned = pinButton.IsChecked;

				//BuildUIFromPayload(listView_PinnedPane, CurrentPayloadCollection);  // UIを再ビルドして反映
			};

			// ピン留めボタンをGridの3列目に配置
			Grid.SetColumn(pinButton, 2);
			grid.Children.Add(pinButton);

			return pinButton;
		}


		/// <summary>
		/// Pin設定の切り替え (Show=trueで全表示)
		/// </summary>
		/// <param name="Show"></param>
		public void TogglePinPane(bool Show = true)
		{
			if (Show)
			{
				listView_PinnedPane.MaxHeight = 560;
				BuildUIFromPayload(listView_PinnedPane, payloads_All);
			}
			else
			{
				listView_PinnedPane.MaxHeight = 360;
				BuildUIFromPayload(listView_PinnedPane, payloads_PinnedOnly);
			}
		}





	}















}
