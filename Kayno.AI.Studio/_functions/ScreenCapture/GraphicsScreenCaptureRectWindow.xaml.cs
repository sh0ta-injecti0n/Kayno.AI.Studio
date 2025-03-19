using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kayno.AI.Studio
{
	/// <summary>
	/// GraphicsScreenCaptureRectWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class GraphicsScreenCaptureRectWindow : Window
    {

		bool IsDragging = false;

		public Rect RectCaptureResult
		{ get; set; }

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);


		public GraphicsScreenCaptureRectWindow()
    {
      InitializeComponent();

			RenderOptions.ProcessRenderMode = RenderMode.Default;
			

			//Loaded += GraphicsScreenCaptureRectWindow_Loaded;
			//PreviewMouseDown += GraphicsScreenCaptureRectWindow_MouseDown;
			//MouseMove += GraphicsScreenCaptureRectWindow_MouseMove;
			//MouseLeftButtonUp += GraphicsScreenCaptureRectWindow_MouseLeftButtonUp;
			KeyDown += (s, e) => releaseAndClose();

			this.Loaded += ( s, e ) =>
			{
				if ( Width < 128 )
				{
					Width = 512;
				}

				if ( Height < 128 )
				{
					Height = 512;
				}

				RectCaptureResult = new Rect( Left, Top, Width, Height );

			};

			this.Closing += ( s, e ) =>
			{
				// 2025-03-03
				// ウィンドウを動かして決めたほうがいいことがわかったので変更

				this.Hide();
				this.Content = null;
				this.Owner = null;
				//GC.Collect();
				//GC.WaitForPendingFinalizers();
				//GC.Collect();
			};
		}


		protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
		{
			base.OnMouseLeftButtonDown( e );

			// Begin dragging the window
			this.DragMove();
		}

		private void OK_Click( object sender, RoutedEventArgs e )
		{
            var MainWindow = Application.Current.MainWindow;

            var src = PresentationSource.FromVisual( MainWindow );
            var m = src.CompositionTarget.TransformToDevice;
            var dpiW = m.M11;
            var dpiH = m.M22;
            //PresentationSource.FromVisual と CompositionTarget.TransformToDevice を使用して、WPFのDPI対応機能を活用します。

            var p1 = new Point( Left*dpiW, Top*dpiH );
			var size = new Size( Width*dpiW, Height*dpiH );
            RectCaptureResult = new Rect( p1, size );
			
			//p1 = PointToScreen(p1);
			//var p2 = new Point( Left+Width, Top+Height );
			//p2 = PointToScreen(p2);
            //RectCaptureResult = new Rect( p1, p2 );

            this.DialogResult = true;
			this.Close();
		}


		void releaseAndClose()
		{
			Background = null;
			this.Content = null;

			ReleaseMouseCapture();
			ReleaseAllTouchCaptures();
			ReleaseStylusCapture();
			this.Close();
		}












		//private void GraphicsScreenCaptureRectWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		//{
		//	IsDragging = true;
		//
		//	var p = e.GetPosition(this);
		//
		//	//Canvas.SetLeft(rectMarker, p.X);
		//	//Canvas.SetTop(rectMarker, p.Y);
		//	
		//	RectCaptureResult = new Rect( p, new Size( 1, 1 ) );
		//}
		//
		//private void GraphicsScreenCaptureRectWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		//{
		//	var p = e.GetPosition((this));
		//
		//	if (IsDragging)
		//	{
		//		//rectMarker.Width = p.X - Canvas.GetLeft(rectMarker);
		//		//rectMarker.Height = p.Y - Canvas.GetTop(rectMarker);
		//	}
		//
		//}
		//
		//private void GraphicsScreenCaptureRectWindow_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		//{
		//	var p1 = new Point(RectCaptureResult.X, RectCaptureResult.Y);
		//	var p2 = e.GetPosition(this);
		//
		//	p1 = PointToScreen(p1);
		//	p2 = PointToScreen(p2);
		//
		//	RectCaptureResult = new Rect( p1, p2 );
		//	// HiDPI環境などのズレを吸収
		//
		//	DialogResult = true;
		//
		//	IsDragging = false;
		//	releaseAndClose();
		//}
		//
		//private void GraphicsScreenCaptureRectWindow_Loaded(object sender, RoutedEventArgs e)
		//{
		//	//MakeTransparent();
		//
		//	Left = SystemParameters.VirtualScreenLeft;
		//	Top = SystemParameters.VirtualScreenTop;
		//	
		//	var vWidth = SystemParameters.VirtualScreenWidth;
		//	var vHeight = SystemParameters.VirtualScreenHeight;
		//
		//	Width  = vWidth;
		//	Height = vHeight;
		//
		//	// @BUG 2025-02-10
		//	// でっかくするとメモリ消費量がとんでもないことになる
		//	// WORKAROUND: 
		//	// 設定値だけ保存してアプリ再起動
		//	
		//}

		[DllImport( "user32.dll", SetLastError = true )]
		private static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );
		[DllImport( "user32.dll", SetLastError = true )]
		private static extern int GetWindowLong( IntPtr hWnd, int nIndex );
		[DllImport( "user32.dll", SetLastError = true )]
		private static extern bool SetLayeredWindowAttributes( IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags );

		private const int GWL_EXSTYLE = -20;
		private const int WS_EX_LAYERED = 0x80000;
		private const int WS_EX_TRANSPARENT = 0x20;
		private const int LWA_COLORKEY = 0x1;
		private const int LWA_ALPHA = 0x2; 
		
		private void MakeTransparent()
		{
			IntPtr hwnd = new WindowInteropHelper( this ).Handle;
			int extendedStyle = GetWindowLong( hwnd, GWL_EXSTYLE );
			//SetWindowLong( hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT );
			SetWindowLong( hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED );
			//SetLayeredWindowAttributes( hwnd, 0, 200, LWA_ALPHA );
			SetLayeredWindowAttributes( hwnd, 0x000000, 80, LWA_ALPHA);

			// 80 = 透明度（0～255）

			//WS_EX_LAYERED を適用
			//これにより、ウィンドウの透過を可能にする（GPUアクセラレーションが維持される）。

			//WS_EX_TRANSPARENT を適用
			//ウィンドウが「マウスイベントを通す」ようになる（クリックが背後のアプリに届く）。
			//ただし、マウスイベントを取得したい場合はこのフラグを外すか、部分的に適用する。
		}



		//protected override void OnClosed( EventArgs e )
		//{
		//	base.OnClosed( e );
		//	HwndSource source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
		//	if ( source != null )
		//	{
		//		//source.RemoveHook( WndProc );
		//	}
		//}


	}




}
