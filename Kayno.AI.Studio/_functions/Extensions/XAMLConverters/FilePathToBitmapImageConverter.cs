using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


/// <summary>
/// ファイルパスからBitmapImageを取得します。
/// </summary>
public class FilePathToBitmapImageConverter : IValueConverter
{

	/// <summary>
	/// ファイルパスからBitmapImageを取得します。
	/// </summary>
	/// <param name="value"></param>
	/// <param name="targetType"></param>
	/// <param name="parameter">ParentDir if exists</param>
	/// <param name="culture"></param>
	/// <returns></returns>
	public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
	{
		if ( value is string fileName && !string.IsNullOrEmpty( fileName ) )
		{
			string directory = parameter as string ?? string.Empty;
			string fullPath = Path.Combine( directory, fileName );

			if ( File.Exists( fullPath ) )
			{
				try
				{
					using ( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
					{
						var bitmap = new BitmapImage();
						bitmap.BeginInit();
						bitmap.CacheOption = BitmapCacheOption.OnLoad; // ファイルをすぐに開放
						bitmap.StreamSource = new MemoryStream( ReadFully( stream ) ); // メモリ上にコピー
						bitmap.EndInit();
						bitmap.Freeze(); // マルチスレッド対応
						return bitmap;
					}

				}
				catch ( Exception ex )
				{
					Debug.WriteLine( $"Image load error: {ex.Message}" );
				}
			}
		}
		return DependencyProperty.UnsetValue;
	}

	// ファイルを MemoryStream にコピーするメソッド
	private byte[] ReadFully( Stream input )
	{
		using ( var ms = new MemoryStream() )
		{
			input.CopyTo( ms );
			return ms.ToArray();
		}
	}

	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
	{
		throw new NotImplementedException();
	}
}
