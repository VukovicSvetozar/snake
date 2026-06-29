using System.Globalization;
using System.Windows.Data;

namespace Snake.Converter
{
    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int speed)
            {
                return speed switch
                {
                    1 => "Slow",
                    2 => "Normal",
                    3 => "Fast",
                    _ => "Unknown"
                };
            }
            return "Invalid";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}