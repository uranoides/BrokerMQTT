using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MQTT.Sharing.Converters
{
    public class PresenceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return null;

            bool presence = (bool)value;
            if (presence)
            {
                return new LinearGradientBrush(Color.FromRgb(0, 150, 136), Color.FromRgb(0, 105, 92), new Point(0.5, 0), new Point(0.5, 1));
            }
            else
            {
                return new LinearGradientBrush(Color.FromRgb(244, 67, 54), Color.FromRgb(183, 28, 28), new Point(0.5, 0), new Point(0.5, 1));
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
