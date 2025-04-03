using System;
using System.Globalization;
using System.Windows.Data;

public class MathRoundConverter : IValueConverter
{
	public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
	{
		int v = 1;
		if (parameter != null) 
			v = (int)parameter;

		if ( value is double doubleValue )
		{
			return Math.Round( doubleValue, v, MidpointRounding.AwayFromZero );
		}
		return value;
	}

	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
	{
		int v = 1;
		if ( parameter != null )
			v = (int)parameter;

		if ( value is double doubleValue )
		{
			return Math.Round( doubleValue, v ); // 小数第1位に丸める
		}
		return value;
	}
}