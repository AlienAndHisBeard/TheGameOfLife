using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TheGameOfLife.Utils
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class ColourConverter : IValueConverter
    {
        public SolidColorBrush BirthColour { get; set; }
        public SolidColorBrush AliveColour { get; set; }
        public SolidColorBrush DeadColour { get; set; }
        public SolidColorBrush DeathColour { get; set; }


        public ColourConverter(SolidColorBrush aliveColour, SolidColorBrush deadColour)
        {
            AliveColour = aliveColour;
            BirthColour = aliveColour;
            DeadColour = deadColour;
            DeathColour = deadColour;
        }
        public ColourConverter(SolidColorBrush birthColour, SolidColorBrush aliveColour, SolidColorBrush deadColour, SolidColorBrush deathColour)
        {
            BirthColour = birthColour;
            AliveColour = aliveColour;
            DeadColour = deadColour;
            DeathColour = deathColour;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int alive = 0;

            if (value is int val) alive = val;

            if (alive == 1) { return DeathColour; }
            if (alive == 2) { return AliveColour; }
            if (alive == 3) { return BirthColour; }
            return DeadColour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                if (brush == DeathColour) return 1;
                if (brush == AliveColour) return 2;
                if (brush == BirthColour) return 3;
            }

            return 0;
        }
    }
}
