using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using Snake.Models;

namespace Snake.Converter
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Transparent;

            return value switch
            {
                Difficulty difficulty => difficulty switch
                {
                    Difficulty.Easy => Brushes.LightGreen,
                    Difficulty.Normal => Brushes.LightSeaGreen,
                    Difficulty.Hard => Brushes.DarkCyan,
                    _ => Brushes.Transparent
                },
                _ => Brushes.Transparent,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}