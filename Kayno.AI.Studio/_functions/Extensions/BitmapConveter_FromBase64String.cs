using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

public static class BitmapConveter_FromBase64String
{

	/// <summary>
	/// Convert base64 string -> BitmapImage(WPF).
	/// </summary>
	/// <param name="base64String"></param>
	/// <returns></returns>
	public static BitmapImage Base64StringToBitmapImage( this string base64String )
	{
		var bytes = Convert.FromBase64String( base64String );

		using ( var stream = new MemoryStream( bytes ) )
		{
			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			bmp.StreamSource = stream;
			bmp.EndInit();
			bmp.Freeze();

			return bmp;
		}

	}

	/// <summary>
	/// Convert base64 string -> bytes array.
	/// </summary>
	/// <param name="base64String"></param>
	/// <returns></returns>
	public static byte[] Base64StringToBytes( this string base64String )
	{
		var bytes = Convert.FromBase64String( base64String );
		return bytes;
	}

	public static string BitmapImageToBase64String( this BitmapImage bmpimg )
	{
		var bmp = bmpimg.GetBitmap();
		ImageConverter cnv = new ImageConverter();
		byte[] b = (byte[])cnv.ConvertTo( bmp, typeof( byte[] ) );

		var result = Convert.ToBase64String( b );
		//var base64 = Convert.ToBase64String(b).Base64StringToBytes();
		//var utf8decode = UTF8Encoding.UTF8.GetDecoder();
		//byte[] buffer = new byte[base64.Length];
		//
		//utf8decode.Convert(base64, buffer);

		// 1. BitmapImage を Bitmap に変換
		// 2. ImageConverter で byte[] に変換
		// 3. Convert でBase64化

		return result;

		// ↓これはだめだった
		//using (var memoryStream = new MemoryStream())
		//{
		//	var encoder = new BmpBitmapEncoder();
		//	encoder.Frames.Add(BitmapFrame.Create(bmpimg));
		//	encoder.Save(memoryStream);
		//
		//	//memoryStream.Position = 0;
		//	byte[] imageBytes = memoryStream.GetBuffer();
		//	var str = Convert.ToBase64String(imageBytes);
		//
		//	return str;
		//
		//}

	}


}
