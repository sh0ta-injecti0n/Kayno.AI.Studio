
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Configuration;


namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
  {

		/*
		 * Commands をあなたのプロジェクトでもかんたんに使う方法:
		 * 0: Extensions にある CommandHelper.cs をコピー
		 * 1: CommandHelper.CreateCommand(～) で RoutedUICommand を宣言
		 * 2: ↑の「宣言名+_Executed」 でメソッドを実装
		 * 3: XAML 上のボタンなどに追加
		 *	   Command="{x:Static this:MainWindow.CMD_Sample}"
		 *	   
		 * +: お好みでキーボードショートカットなど InitializeCommandBindings() を カスタム
		 * +: InitializeCommandBindings() を MainWindow の Loaded などで実行
		 * 
		 */

		public static RoutedUICommand CMD_Test = CommandHelper.CreateCommand(nameof(CMD_Test));
		public static RoutedUICommand CMD_PresetSlot = CommandHelper.CreateCommand(nameof(CMD_PresetSlot));
		public static RoutedUICommand CMD_ResetModels = CommandHelper.CreateCommand(nameof(CMD_ResetModels));
		public static RoutedUICommand CMD_SwitchPinEditor = CommandHelper.CreateCommand(nameof(CMD_SwitchPinEditor));
		
		public static RoutedUICommand CMD_ScreenCapture_RegionDefine = CommandHelper.CreateCommand(nameof(CMD_ScreenCapture_RegionDefine));
		public static RoutedUICommand CMD_Dock_SendDataOnly = CommandHelper.CreateCommand(nameof( CMD_Dock_SendDataOnly ) );
		public static RoutedUICommand CMD_Dock_DoGeneration = CommandHelper.CreateCommand(nameof(CMD_Dock_DoGeneration));
        public static RoutedUICommand CMD_PasteImage0 = CommandHelper.CreateCommand( nameof( CMD_PasteImage0 ) );

        public static RoutedUICommand CMD_Script = CommandHelper.CreateCommand( nameof( CMD_Script ) );

        public static RoutedUICommand CMD_Settings = CommandHelper.CreateCommand(nameof(CMD_Settings));

        private void InitializeCommandBindings()
		{
			var inputs = new InputBindingCollection
			{
				//new KeyBinding(CMD_PresetSlot, Key.Space, ModifierKeys.Control)
				 new KeyBinding(CMD_SwitchPinEditor, Key.Space, ModifierKeys.Control)
				, new KeyBinding(CMD_ScreenCapture_RegionDefine, Key.X, ModifierKeys.Alt)
				, new KeyBinding(CMD_Dock_SendDataOnly, Key.F11, ModifierKeys.Control | ModifierKeys.Shift)
				, new KeyBinding(CMD_Dock_DoGeneration, Key.F12, ModifierKeys.Control | ModifierKeys.Shift)
				, new KeyBinding(CMD_Dock_DoGeneration, Key.Enter, ModifierKeys.Control)
                , new KeyBinding(CMD_PasteImage0, Key.Enter, ModifierKeys.Control | ModifierKeys.Shift)
                , new KeyBinding(CMD_Script, Key.A, ModifierKeys.Alt)
				// add commands here
			};
			this.InputBindings.AddRange(inputs);
			// キーボードショートカットへの登録 (任意)
		}

        private async void CMD_Test_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Debug.WriteLine("Event: " + e.ToString());
			MessageBox.Show( "test" );
		}

		private async void CMD_PresetSlot_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var msg_save = Properties.Resources.Dialog_ComfirmPresetSave;
			var msgBox = MessageBox.Show( msg_save, "", MessageBoxButton.YesNo );
			if ( msgBox == MessageBoxResult.Yes )
			{
                TsvSerializer.SaveToTsv( CurrentPayloadPath, CurrentPayloadCollection );
            }

            var msg = Properties.Resources.PresetSlotManager_InitialMessage;
			MessageBox.Show( msg );
			var dialog = new OpenFolderDialog();
			dialog.InitialDirectory = KaynoDataDirectory;
			dialog.Title = msg;

            var res = dialog.ShowDialog();

			if ( res == true )
			{
				CurrentPayloadDir = dialog.FolderName;
				await LoadPayload();

				//listView_PinnedPane.GetBindingExpression( ListView.ItemsSourceProperty ).UpdateTarget();
                //listView_PinnedPane.GetBindingExpression( ListView.DataContextProperty ).UpdateTarget();
				//TogglePinPane();

			}

		}

		private async void CMD_ResetModels_Executed( object sender, RoutedEventArgs e )
        {
            var msg = Properties.Resources.Dialog_ResetModelsConfirm;
            var res = MessageBox.Show( msg, "", MessageBoxButton.YesNo );
            if ( res == MessageBoxResult.Yes )
            {
                await LoadPayload();

            }
        }

        private async void CMD_SwitchPinEditor_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var btn = toggleButton_SwitchPinEditor;
			TogglePinPane((bool)btn.IsChecked);
			// do 
		}

		private async void CMD_ScreenCapture_RegionDefine_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var msg = Properties.Resources.ScreenCapture_ConfirmDialog_Msg;
			var dialog = MessageBox.Show( msg, "？", MessageBoxButton.YesNoCancel, MessageBoxImage.Question );
			// 画面キャプチャ領域を再設定しますか？

			if ( dialog == MessageBoxResult.Yes )
			{
				await SetScreenCaptureRegion();
			}
			else if ( dialog == MessageBoxResult.No )
			{
				await InitScreenCapture();
				DoScreenCapture();
				// 前回のキャプチャ領域でキャプチャを開始する
			}
			else
			{
				// do nothing
			}
		}


		private async void CMD_Dock_SendDataOnly_Executed( object sender, ExecutedRoutedEventArgs e )
		{
			//DoScreenCapture( true );
			// 2025-03-17 バグ？なんか挙動がおかしい。GPUを浪費し始める

			PaneProgress1.Visibility = Visibility.Visible;
            if ( (bool)toggleButton_Use_txt2img.IsChecked )
            {
                foreach ( var p in CurrentPayloadCollection )
                {
                    p.WebSelectorValue = p.WebSelectorValue.Replace( "img2img", "txt2img" );
                }

            }
			await Task.Run( () =>
			{
				webSenderSelenium1.SendWebData( CurrentPayloadCollection );
			} );
			PaneProgress1.Visibility = Visibility.Collapsed;

		}


		private async void CMD_Dock_DoGeneration_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//webSenderSelenium1.SendWebData(CurrentPayloadCollection);
			PaneProgress1.Visibility = Visibility.Visible;
			StartProgress();
            await Task.Run( () =>
            {
				webSenderSelenium1.SendImage();
				webSenderSelenium1.SendGenerateCommand();
            } );
			PaneProgress1.Visibility = Visibility.Collapsed;

		}

		private bool IsGenerationInProgress = false;
		private int GenerationProgressEmptyCount = 0;
        private void StartProgress()
        {
			GenerationProgressEmptyCount = 0;
            IsGenerationInProgress = true;
			// DispatcherTimer の中でProgress処理開始
        }


        private void CMD_PasteImage0_Executed( object sender, ExecutedRoutedEventArgs e )
		{

			//var appname = listView_AppsList.SelectedValue as string;
			var applist = GetRunningApps();
			var apppathlist = applist.Select( i => i.ProcessPath ).ToList();
			var appname = apppathlist.Where(i => i.ToLower().Contains("clipstudiopaint")).FirstOrDefault();
			//var appname = applist.Where(i => i.DisplayName.ToLower().Contains( "clipstudiopaint" ) ).First().ProcessPath;
			var filePath = imageGallery1.CurrentImageItem.FilePath;

			Process.Start( appname, filePath );

        }

        private void CMD_Script_Executed( object sender, ExecutedRoutedEventArgs e )
		{
			var dialog = new OpenFileDialog();
			var res = dialog.ShowDialog();
			if (res == true)
			{
				InitPythonEngine();
				ExecutePythonScript( dialog.FileName );
			}
		}

        private async void CMD_Settings_Executed( object sender, ExecutedRoutedEventArgs e )
		{
			//string path = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal ).FilePath;
			//string path = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
			//path = Path.Combine( path, "Kayno.AI.Studio" );

			var path = AppSettings.SettingsFilePath;
            Process.Start( @"explorer.exe", path );
		}




        private void MainWindow_ComboBoxSelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			Debug.WriteLine( "SelectionChanged" );
			var cb = (ComboBox)sender;

			if ( cb.Tag is "res_preset" )
			{
				
				var list = cb.ItemsSource as ObservableCollection<PayloadTemplate>;
				var label = list[ cb.SelectedIndex ].TLabel;
				if ( label is "Custom..." )
				{
					return;
				}

				try
				{
					var wxh = label.Split( "x" );
					var w = wxh[ 0 ];
					var h = wxh[ 1 ];

					var pw = CurrentPayloadCollection.First( i => i.PropertyName == "res_w" );
					pw.PropertyValue = w;
					var ph = CurrentPayloadCollection.First( i => i.PropertyName == "res_h" );
					ph.PropertyValue = h;

				}
				catch ( Exception ex )
				{

				}

				// OnPropertyChangedをつけてデータバインディングしているため、
				// データへの代入で自動的にUIが更新される
			}

			// 各コンボボックスをここで判定してそれぞれでやりたいことをここに書く
		}







	}
}
