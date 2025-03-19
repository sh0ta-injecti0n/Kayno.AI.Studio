using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


/// <summary>
/// WinForm Bitmap ←→ WPF BitmapImage 間の変換を行います。 
/// </summary>
public static class BitmapConverter
{

	/// <summary>
	/// System.DrawingのBitmapから、WPFのBitmapImageへ変換します。
	/// </summary>
	/// <param name="bitmap"></param>
	/// <returns>System.Windows.Media.Imaging の BitmapImage</returns>
	public static BitmapImage ToWPFBitmapImage( this Bitmap bitmap )
	{

		using ( MemoryStream ms = new MemoryStream() )
		{

			// no using here! BitmapImage will dispose the stream after loading
			bitmap.Save( ms, System.Drawing.Imaging.ImageFormat.Bmp );

			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			bmp.StreamSource = ms;
			bmp.EndInit();

			bmp.Freeze();

			return bmp;
		}
	}

	/// <summary>
	/// WPFのBitmapImageから、System.Drawing の Bitmapへ変換します。
	/// </summary>
	/// <param name="data"></param>
	/// <returns>System.Drawing の Bitmap</returns>
	public static Bitmap ToDrawingBitmap( this byte[] data )
	{
		using ( var str = new MemoryStream( data ) )
		{
			Bitmap bmp = new Bitmap( str );
			return bmp;

		}


	}

	/// <summary>
	/// WPF BitmapSource to System.Drawing.Bitmap
	/// </summary>
	/// <param name="source"></param>
	/// <see cref="https://gist.github.com/nashby/916300"/>
	/// <returns></returns>
	public static Bitmap GetBitmap( this BitmapSource source )
	{
		Bitmap bmp = new Bitmap
		(
			source.PixelWidth,
			source.PixelHeight,
			System.Drawing.Imaging.PixelFormat.Format32bppPArgb
		);

		BitmapData data = bmp.LockBits
		(
			new System.Drawing.Rectangle( System.Drawing.Point.Empty, bmp.Size ),
			ImageLockMode.WriteOnly,
			System.Drawing.Imaging.PixelFormat.Format32bppPArgb
		);

		source.CopyPixels
		(
			Int32Rect.Empty,
			data.Scan0,
			data.Height * data.Stride,
			data.Stride
		);

		bmp.UnlockBits( data );

		return bmp;

	}

	/// <summary>
	/// System.Drawing の Bitmapから、MemoryStreamに変換します。
	/// </summary>
	/// <param name="img"></param>
	/// <returns></returns>
	public static MemoryStream ToStream( this System.Drawing.Image img )
	{
		MemoryStream ms = new MemoryStream();
		img.Save( ms, ImageFormat.MemoryBmp );

		return ms;
	}

	public static byte[] ToStream2( this System.Drawing.Image img )
	{
		using var ms = new MemoryStream();
		img.Save( ms, ImageFormat.Bmp );
		return ms.GetBuffer();
	}

	public static MemoryStream ToStreamPNG( this System.Drawing.Image img )
	{
		MemoryStream ms = new MemoryStream();
		img.Save( ms, ImageFormat.Png );

		return ms;
	}

}

