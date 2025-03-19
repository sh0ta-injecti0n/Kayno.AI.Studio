using System.Diagnostics;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;  // WinFormsを使うために追加

namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
  {

		BitmapImage BitmapImageScreenCapture = new BitmapImage();
		bool IsDefiningRegion = false;

		/// <summary>
		/// 画面キャプチャに必要な準備を行います。(初期化)
		/// </summary>
		/// <returns></returns>
		public async Task InitScreenCapture()
		{
			InitScreenCaptureRenderTimer();
		}

		/// <summary>
		/// 画面キャプチャ領域をマウスで決定します。(初回およびリセット処理)
		/// </summary>
		/// <returns></returns>
		public async Task SetScreenCaptureRegion()
		{
			captureMouseEx();

			await Dispatcher.BeginInvoke
			(
				()
				=>
				{
					//HwndSource source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
					//source.AddHook( new HwndSourceHook( WndProc ) );
					// ScreenCapture 関連

					var _wind = new GraphicsScreenCaptureRectWindow();
					_wind.Left	= Pref.Default.ScreenCaptureRectX;
					_wind.Top	= Pref.Default.ScreenCaptureRectY;
					_wind.Width	= Pref.Default.ScreenCaptureRectWidth;
					_wind.Height= Pref.Default.ScreenCaptureRectHeight;

					var isResize = CurrentPayloadCollection.First( i => i.PropertyName == "res_autofit" ).PropertyValue;
					if ( isResize is true )
					{
						var w = int.Parse( payloads_All.First( i => i.PropertyName == "res_w" ).PropertyValue.ToString() );
						var h = int.Parse( payloads_All.First( i => i.PropertyName == "res_h" ).PropertyValue.ToString() );
						_wind.Width = w;
						_wind.Height = h;
					}
					var res = _wind.ShowDialog();
					if (res == true)
					{ 
						//config.AppSettings.Settings[ nameof( Pref.Default.ScreenCaptureRectX ) ].Value = _wind.RectCaptureResult.X.ToString();
						//config.AppSettings.Settings[ nameof( Pref.Default.ScreenCaptureRectY ) ].Value = _wind.RectCaptureResult.Y.ToString();
						//config.AppSettings.Settings[ nameof( Pref.Default.ScreenCaptureRectWidth ) ].Value = _wind.RectCaptureResult.Width.ToString();
						//config.AppSettings.Settings[ nameof( Pref.Default.ScreenCaptureRectHeight ) ].Value = _wind.RectCaptureResult.Height.ToString();

                        Pref.Default.ScreenCaptureRectX = _wind.RectCaptureResult.X;
						Pref.Default.ScreenCaptureRectY = _wind.RectCaptureResult.Y;
						Pref.Default.ScreenCaptureRectWidth = _wind.RectCaptureResult.Width;
						Pref.Default.ScreenCaptureRectHeight = _wind.RectCaptureResult.Height;
					}

					releaseMouseEx();

					//if ( res == true )
					//{
					//}
					//else
					//{
					//	releaseMouseEx();
					//	// マウス以外で復帰した場合、IsCapturing がONのままなのでリセット
					//}

					//if ( source != null )
					//{
					//	source.RemoveHook( WndProc );
					//  source.Dispose();
					//}
					//メモリリークになるのでWndProcの使用後は絶対WndProcは消すこと

					_wind.Close();
					_wind = null;

				}
			);

			//GC.WaitForPendingFinalizers();
			//GC.Collect();

			DoScreenCapture();

		}


		void captureMouseEx()
		{
			IsDefiningRegion = true;
			//Cursor = Cursors.Cross;
			//CaptureMouse();
		}

		void releaseMouseEx()
		{
			IsDefiningRegion = false;
			//Cursor = Cursors.Arrow;
			//ReleaseMouseCapture();
		}

		/// <summary>
		/// 画面をキャプチャします。定期的に実行するにはTimer等の中で実行してください。(要 初期化)
		/// </summary>
		/// <returns></returns>
		public BitmapImage DoScreenCapture(bool WillSaveToFile = true)
		{
			try
			{
				var _useAltImage = CurrentPayloadCollection.FirstOrDefault( i => i.PropertyName is "use_img2img_alt" ).PropertyValue;
				var useAltImage = bool.Parse( _useAltImage.ToString() );
				if ( useAltImage )
				{
					var _path = CurrentPayloadCollection.FirstOrDefault( i => i.PropertyName is "area_img2img_alt" ).PropertyValue.ToString();
					if ( !File.Exists( _path ) )
					{
						return new BitmapImage();
					}

					using var str = new FileStream( _path, FileMode.Open );

					var bi = new BitmapImage();
					bi.BeginInit();
					bi.CacheOption = BitmapCacheOption.OnLoad;
					bi.StreamSource = str;
					bi.EndInit();
					bi.Freeze();

					BitmapImageScreenCapture = bi;
				}
				else
				{

					var rectX = (int)Pref.Default.ScreenCaptureRectX;
					var rectY = (int)Pref.Default.ScreenCaptureRectY;
					var rectW = (int)Pref.Default.ScreenCaptureRectWidth;
					var rectH = (int)Pref.Default.ScreenCaptureRectHeight;
					if ( rectW < 2 || rectH < 2 )
					{
						return null;
					}

					var bmp = new DRAW.Bitmap( rectW, rectH );
					using var graphics = DRAW.Graphics.FromImage( bmp );

					graphics.CopyFromScreen( rectX, rectY, 0, 0, bmp.Size );
					graphics.DrawImage( bmp, 0, 0, rectW, rectH );

					var filename = CurrentPayloadCollection.First( i => i.PropertyName is "area_img2img" ).PropertyValue.ToString();
					var path = Path.Combine( CurrentPayloadDir, filename );

					var isResize = CurrentPayloadCollection.First( i => i.PropertyName == "res_autofit" ).PropertyValue;
					if ( isResize is true )
					{
						var w = int.Parse( payloads_All.First( i => i.PropertyName == "res_w" ).PropertyValue.ToString() );
						var h = int.Parse( payloads_All.First( i => i.PropertyName == "res_h" ).PropertyValue.ToString() );
						bmp = ResizeBitmap( bmp, w, h );
						//Debug.WriteLine( $"W x H: {bmp2.Width},{bmp2.Height}");

					}

					if (WillSaveToFile)
					{
						bmp.Save( path );
					}
					BitmapImageScreenCapture = bmp.ToWPFBitmapImage();

					bmp.Dispose();
				}

			}
			catch (Exception ex)
			{
				BitmapImageScreenCapture = new BitmapImage();

			}

			ImageCapture.Source = BitmapImageScreenCapture;

			return BitmapImageScreenCapture;
		}

		public Bitmap ResizeBitmap( Bitmap originalBitmap, int maxWidth, int maxHeight )
		{
			// オリジナルの画像のアスペクト比を計算
			float aspectRatio = (float)originalBitmap.Width / originalBitmap.Height;

			// 最大サイズに合わせてリサイズ
			int newWidth, newHeight;
			if ( originalBitmap.Width > originalBitmap.Height )
			{
				newWidth = maxWidth;
				newHeight = (int)( maxWidth / aspectRatio );
			}
			else
			{
				newHeight = maxHeight;
				newWidth = (int)( maxHeight * aspectRatio );
			}

			// リサイズされた画像を生成
			Bitmap resizedBitmap = new Bitmap( originalBitmap, newWidth, newHeight );
			return resizedBitmap;
		}




	}
}
