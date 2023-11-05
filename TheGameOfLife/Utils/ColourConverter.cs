using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TheGameOfLife.Utils
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class ColourConverter : IValueConverter
    {
        public SolidColorBrush AliveColour { get; private set; }
        public SolidColorBrush DeadColour { get; private set; }

        public ColourConverter(SolidColorBrush aliveColour, SolidColorBrush deadColour)
        {
            AliveColour = aliveColour;
            DeadColour = deadColour;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool alive = false;

            if (value is bool val) alive = val;

            return alive ? AliveColour : DeadColour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush) return (SolidColorBrush)value == AliveColour;
            return false;
        }
    }
}
