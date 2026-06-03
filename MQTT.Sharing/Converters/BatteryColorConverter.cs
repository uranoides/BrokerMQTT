using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MQTT.Sharing.Converters
{
    public class BatteryColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            double battery;
            try { battery = System.Convert.ToDouble(value); }
            catch { return Brushes.Transparent; }

            if (battery >= 3.0 && battery <= 3.6)
                return new SolidColorBrush(Color.FromRgb(76, 175, 80));    // Verde
            else if (battery >= 2.7 && battery < 3.0)
                return new SolidColorBrush(Color.FromRgb(255, 152, 0));    // Arancione
            else if (battery >= 0 && battery < 2.7)
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));    // Rosso

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
