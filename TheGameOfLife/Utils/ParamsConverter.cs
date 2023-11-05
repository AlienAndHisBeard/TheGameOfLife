using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TheGameOfLife.Utils
{
    [ValueConversion(typeof(uint), typeof(string))]
    public class ParamsConverter : IValueConverter
    {
        private readonly uint _defaultValue;

        public ParamsConverter(uint defaultValue) 
        {
            _defaultValue = defaultValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (uint.TryParse(value as string, out uint result)) return result;

            return _defaultValue;
        }
    }
}
