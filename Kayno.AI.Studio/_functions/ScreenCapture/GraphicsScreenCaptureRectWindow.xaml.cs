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









	}




}
