using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BitmapImageHelper 
{

	/// <summary>
	/// BitmapImage をファイルパスから生成します。
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static BitmapImage CreateBitmapImageFromPath(string path, int PixelW = 0, int PixelH = 0)
	{
		var bmp = new BitmapImage();

		try
		{
			using var stream = new FileStream( path, FileMode.Open );
			bmp.BeginInit();
			bmp.StreamSource = stream;
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			if (PixelW != 0)
			{
				bmp.DecodePixelWidth = PixelW;
			}
			if (PixelH != 0)
			{
				bmp.DecodePixelHeight = PixelH;
			}
			bmp.EndInit();
			if ( bmp.CanFreeze )
			{
				bmp.Freeze();
			}

		}
		catch ( Exception ex )
		{
			return null;
		}
		
		return bmp;

	}


}
