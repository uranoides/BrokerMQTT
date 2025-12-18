using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MQTT.Sharing.Converters
{
    public class NetstateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return null;

            string state = (string)value;
            if (state == "ESTABLISHED")
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009688"));     // Green6
            }
            else if (state == "TIME_WAIT")
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEB3B"));     // Yellow6
            }
            else if (state == "LISTENING")
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E88E5"));     // Blue6
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1F6FF"));     // Azure1
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
