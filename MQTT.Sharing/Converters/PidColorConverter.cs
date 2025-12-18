using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MQTT.Sharing.Converters
{
    public class PidColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return null;

            int pid = (int)value;
            if (pid > 0)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B4E7FF"));     // Azure2
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEB3B"));     // Yellow6
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
