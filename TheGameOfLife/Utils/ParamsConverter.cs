using System;
using System.Globalization;
using System.Windows.Data;

namespace TheGameOfLife.Utils
{
    [ValueConversion(typeof(uint), typeof(string))]
    public class ParamsConverter : IValueConverter
    {
        private readonly uint _defaultValue;
        private readonly int _minValue;
        private readonly int _maxValue;


        public ParamsConverter(uint defaultValue, int minValue = 0, int maxValue = 100) 
        {
            _defaultValue = defaultValue;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value as string, out int result)) return _defaultValue;

            if (result < _minValue)
            {
                if (result * 10 < _maxValue) return result*10;
                return (uint)_minValue;
            }

            if(result > _maxValue) return (uint)_maxValue;

            return (uint)result;
            
        }
    }
}
