using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kayno.AI.Studio
{

	/// <summary>
	/// ギャラリー用の BitmapImage + Path のクラス
	/// </summary>
	public class ImageItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public string FilePath { get; set; }

		public string FileName
		{
			get
			{
				return Path.GetFileName( FilePath );
			}
		}

		public BitmapImage Image { get; set; }

		public ImageItem()
		{
		}

		public ImageItem( string filePath )
		{
			FilePath = filePath;
			Image = BitmapImageHelper.CreateBitmapImageFromPath( filePath );

		}

	}


	/// <summary>
	/// ImageGallery.xaml の相互作用ロジック
	/// </summary>
	public partial class ImageGallery : UserControl, INotifyPropertyChanged
	{

		#region ## Properties

		public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnCurrentImageIndexPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
			imageMain.Source = CurrentImage;
            Debug.WriteLine( "OnPropertyChanged: " + propertyName );
        }

        /// <summary>
        /// ギャラリーのディレクトリのパス。
        /// </summary>
        public string SourceDirectory
        {
			get { return (string)GetValue( SourceDirectoryProperty ); }
			set 
			{
				SetValue( SourceDirectoryProperty, value ); 
				ResetImages();
			}
		}

		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceDirectoryProperty =
			DependencyProperty.Register(
				nameof(SourceDirectory),
				typeof( string ), 
				typeof( ImageGallery ), 
				new PropertyMetadata( "" ) );



		/// <summary>
		/// ファイル名のフィルタ。
		/// </summary>
		public string FilterFileName
        {
			get { return (string)GetValue( FilterFileNameProperty ); }
			set { SetValue( FilterFileNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FilterFileNameProperty =
			DependencyProperty.Register(
				nameof(FilterFileName),
				typeof( string ),
				typeof( ImageGallery ), 
				new PropertyMetadata( "*.*" ) );


		/// <summary>
		/// ファイルを再帰的に探索するかどうか。
		/// </summary>
		public bool UseRecursive { get; set; } = true;


		/// <summary>
		/// 監視対象のディレクトリ。
		/// </summary>
		//public string WatchDirectory
		//{
		//	get { return (string)GetValue( WatchDirectoryProperty ); }
		//	set { SetValue( WatchDirectoryProperty, value ); }
		//}
		//
		//// Using a DependencyProperty as the backing store for WatchDirectory.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty WatchDirectoryProperty =
		//	DependencyProperty.Register( nameof(WatchDirectory), typeof( string ), typeof( ImageGallery ), new PropertyMetadata( "" ) );


		/// <summary>
		/// ギャラリーの自動更新の是非。
		/// </summary>
		public bool AutoReload { get; set; } = true;


		private ObservableCollection<ImageItem> Imageitems { get; set; }

		public int CurrentImageIndex
		{
			get
			{
				return _currentImageIndex;
			}
			set
			{
				_currentImageIndex = value;
				OnCurrentImageIndexPropertyChanged( value.ToString() );
			}
		}

		public ImageItem CurrentImageItem
		{
            get
            {
                try
                {
                    return Imageitems?[ CurrentImageIndex ] ?? null;
                }
                catch ( Exception )
                {
                    return null;
                }
            }
        }

		private int _currentImageIndex;
		public BitmapImage CurrentImage
		{
			get
			{
				try
				{
					return Imageitems?[ CurrentImageIndex ].Image ?? null;
				}
				catch ( Exception )
				{
					return null;
				}
			}
		}
		#endregion

        public ImageGallery()
		{
			InitializeComponent();

			DataContext = CurrentImage;
			listView_ThumbList.DataContext = Imageitems;
			listView_ThumbList.ItemsSource = Imageitems;
			Loaded += ImageGallery_Loaded;
			Unloaded += ImageGallery_Unloaded;

			KeyDown += ImageGallery_KeyDown;
			paneMain.IsManipulationEnabled = false;
			paneMain.MouseDown += ImageMain_Click;
			paneMain.MouseMove += ImageMain_MouseMove;
			paneMain.MouseUp += ImageMain_MouseUp;

            //var inputs = new InputBindingCollection
            //{
            //    new KeyBinding(CMD_ImageGalleryRightInc, Key.Right, ModifierKeys.None)
            //    , new KeyBinding(CMD_ImageGalleryLeftDec, Key.Left, ModifierKeys.None)
			//	// add commands here
			//};
            //this.InputBindings.AddRange( inputs );

            //imageMain.MouseWheel += ImageMain_MouseWheel;
            //imageMain.TouchDown += ImageMain_TouchDown;
            //imageMain.ManipulationStarted += ImageMain_ManipulationStarted;
            //imageMain.ManipulationDelta += ImageMain_ManipulationDelta;
            //imageMain.ManipulationCompleted += ImageMain_ManipulationCompleted;
        }


		/// <summary>
		/// 画像のリストを更新。
		/// </summary>
        public void ResetImages()
        {
            if ( SourceDirectory == null ) return;
            if ( string.IsNullOrEmpty( SourceDirectory ) ) return;
            if ( !Directory.Exists( SourceDirectory ) ) return;

            var recursive = UseRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles( SourceDirectory, FilterFileName, recursive );

            if ( files == null ) return;
            if ( !files.Any() ) return;

			if (files.Count() > 1000 )
			{
				var msg = "ファイル件数が1000件を超えています。動作が重くなりますが続行しますか？"; 
				var res = MessageBox.Show( msg,"",MessageBoxButton.OKCancel);
				if ( res == MessageBoxResult.OK )
				{
				}
				else
				{
					return;
				}
			}

            Imageitems = new ObservableCollection<ImageItem>();
            foreach ( var file in files )
            {
                var item = new ImageItem( file );
                if ( item != null ) Imageitems.Add( item );
            }

            if ( Imageitems.Any() )
            {
                imageMain.Source = CurrentImage;
                listView_ThumbList.ItemsSource = Imageitems;

                CurrentImageIndex = 0;

            }

            //imageMain.Source = CurrentImage;

            // FileSystemWatcher の予定だったが、
            // シンボリックリンクでは機能しないためやっぱりなし
        }

		public void SetCurrentImageIndex(int index)
		{
			CurrentImageIndex = index;
		}

		public void SetCurrentImageIndexAsLast()
		{
			CurrentImageIndex = Imageitems.Count - 1;
		}


        #region ## Init
        private void ImageGallery_Loaded( object sender, RoutedEventArgs e )
		{
			if ( SourceDirectory == null ) return;
			ResetImages();
		}

		private void ImageGallery_Unloaded( object sender, RoutedEventArgs e )
		{
		}
		#endregion


		#region ## Manipulations

		double Manipu_deltaInit = -1;
		double Manipu_deltaVector = 0;
		double Manipu_delta = 0;
		double Manipu_deltaThreshold = 60;
		private void ImageMain_Click( object sender, MouseButtonEventArgs e )
		{
			//CaptureMouse();

			Manipu_deltaInit = e.GetPosition( this ).X;
			Manipu_delta = 0;
			imageMain.Opacity = 1;
			var x = imageMain.RenderTransform as TranslateTransform;
			if (x == null)
			{
				x = new TranslateTransform();
			}
			x.X = 0;

			if ( e.ClickCount == 1 )
			{
				var IsVisible = pane_Controls.Visibility == Visibility.Visible;
				pane_Controls.Visibility = IsVisible ? Visibility.Hidden : Visibility.Visible;
			}

			if ( e.ClickCount == 2 )
			{ 
				FullScreen();
			}
		}

		private void ImageMain_MouseMove(object sender, MouseEventArgs e)
		{
			if (Manipu_deltaInit == -1) return;

			Manipu_deltaVector = e.GetPosition( this ).X - Manipu_deltaInit;
			Manipu_delta = Math.Abs(Manipu_deltaVector);

			if (0 == CurrentImageIndex && 0 < Manipu_deltaVector )
			{

			}
			else if (CurrentImageIndex == Imageitems.Count-1 && Manipu_deltaVector < 0)
			{

			}
			else
			{
				var opacity = 1.0 - ( Manipu_delta / Manipu_deltaThreshold);
				imageMain.Opacity = opacity;
			}
			var x = imageMain.RenderTransform as TranslateTransform;
			if ( x == null ) return;
			x.X = Math.Clamp(Manipu_deltaVector, -100, 100);
		}

		private void ImageMain_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if ( Manipu_delta > (Manipu_deltaThreshold*0.8) )
			{
				var IsLeftIncrease = Manipu_deltaVector < 0;
				ChangeIndex( IsLeftIncrease );
			}

			Manipu_deltaInit = -1;
			Manipu_delta = 0;
			imageMain.Opacity = 1;
			var x = imageMain.RenderTransform as TranslateTransform;
			x.X = 0;

			//ReleaseMouseCapture();
		}

		private void ImageMain_MouseWheel( object sender, MouseWheelEventArgs e )
		{
			// 画像の拡大縮小のための変換
			var zoom = e.Delta > 0 ? 1.1 : 0.9; // ホイールの回転に基づいて倍率を決定
			var zoomTrans = imageMain.RenderTransform as ScaleTransform;

			// ScaleTransformがnullの場合、初期化する
			if ( zoomTrans == null )
			{
				zoomTrans = new ScaleTransform( 1, 1 );  // 初期の倍率を1に設定
				imageMain.RenderTransform = zoomTrans;
			}

			// マウスの位置を取得
			var mousePos = e.GetPosition( imageMain );
			zoomTrans.CenterX = mousePos.X;
			zoomTrans.CenterY = mousePos.Y;

			// 新しい倍率を設定
			zoomTrans.ScaleX *= zoom;
			zoomTrans.ScaleY *= zoom;

		}

		private void ImageGallery_KeyDown( object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Right)
			{
                ChangeIndex( true );
            }
			else if (e.Key == Key.Left)
			{
                ChangeIndex( false );
            }
        }

		private void CMD_ImageGalleryRightInc_Executed( object sender, EventArgs e )
		{
		}

        private void CMD_ImageGalleryLeftDec_Executed( object sender, EventArgs e )
        {
        }
        #endregion


        #region ##Events

        private void Button_Reload_Click( object sender, RoutedEventArgs e )
		{
			ResetImages();
		}

		private void Button_FullScreen_Click( object sender, RoutedEventArgs e )
		{
			FullScreen();
		}

		void FullScreen()
		{
			var wind = new Window();
			var img = new Image { Source = CurrentImage };
			img.Stretch = System.Windows.Media.Stretch.Uniform;
			wind.Content = img;

			wind.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			wind.WindowState = WindowState.Maximized;
			//wind.WindowStyle = WindowStyle.None;
			//wind.Topmost = true;

			wind.MouseDown += ( s, e ) => { if (e.ClickCount == 2) wind.Close(); };
			wind.TouchDown += ( s, e ) => { wind.Close(); };
			wind.KeyDown += ( s, e ) => { wind.Close(); };
			wind.ShowDialog();

			wind = null;
		}

		private void Button_IncDec_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			if ( btn != null )
			{
				var txt = btn.Content as string;
				if ( txt != null )
				{
					ChangeIndex( txt is "›" );
				}
			}

			imageMain.Source = Imageitems[ CurrentImageIndex ].Image;
			//e.Handled = true;
		}

		void ChangeIndex( bool IsIncreasing )
		{
			if ( IsIncreasing )
			{
				CurrentImageIndex = CurrentImageIndex < Imageitems.Count - 1 ? CurrentImageIndex + 1 : CurrentImageIndex;
			}
			else
			{
				CurrentImageIndex = 0 < CurrentImageIndex ? CurrentImageIndex - 1 : CurrentImageIndex;
			}

			imageMain.Source = CurrentImage;
			imageMain.Opacity = 1;
			var x = imageMain.RenderTransform as TranslateTransform;
			x.X = 0;
		}


		private void listView_ThumbList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			CurrentImageIndex = listView_ThumbList.SelectedIndex;
			imageMain.Source = CurrentImage;
		}
        #endregion


	}









}
