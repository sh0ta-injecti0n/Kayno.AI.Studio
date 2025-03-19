using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Data;

namespace StylesGlobal
{
  public partial class StylesGlobal
  {
  }

	public class ToHalfConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double? val = (double?)value;

			val = val.HasValue ? val.Value/2 : double.MinValue;

			return val;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double? val = (double?)value;

			val = val.HasValue ? val.Value / 2 : double.MinValue;

			return val;
		}
	}


}
