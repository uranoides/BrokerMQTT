using System.Globalization;
using System.Windows.Data;

namespace MQTT.Sharing.Converters
{
    public class SecondsToMMSSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int totalSeconds)
            {
                TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

                return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
