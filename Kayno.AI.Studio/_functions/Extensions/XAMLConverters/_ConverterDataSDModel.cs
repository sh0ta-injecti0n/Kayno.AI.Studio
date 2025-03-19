using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kayno.AI.Studio
{
	public class ConverterDataSDModel : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;

		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() != typeof(Data_SDModel))
			{
				return null;
			}

			var data = (Data_SDModel)value;
			return data.model_name;
		}
	}


}
