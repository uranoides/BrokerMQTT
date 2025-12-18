using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MQTT.Sharing.Converters
{
    public class LedColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)values[0];
            if (status == null)
                return null;

            bool? isPresent = (bool?)values[1];

            var color = new RadialGradientBrush(Color.FromRgb(0, 0, 0), Color.FromRgb(0, 0, 0));        // Colore Sfondo NERO se Status != OFFLINE e != VALID

            if (status.ToUpper() == "OFFLINE")
            {
                color = new RadialGradientBrush(Color.FromRgb(160, 160, 160), Color.FromRgb(80, 80, 80));
            }
            else if (status.ToUpper() == "VALID")
            {
                if (isPresent == null)
                {
                    color = new RadialGradientBrush(Color.FromRgb(160, 160, 160), Color.FromRgb(80, 80, 80));
                }
                else
                {
                    if (isPresent == true)
                    {
                        color = new RadialGradientBrush(Color.FromRgb(0, 150, 136), Color.FromRgb(0, 105, 92));
                    }
                    else
                    {
                        color = new RadialGradientBrush(Color.FromRgb(244, 67, 54), Color.FromRgb(183, 28, 28));
                    }
                }
            }
            return color;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported");
        }
    }
}
