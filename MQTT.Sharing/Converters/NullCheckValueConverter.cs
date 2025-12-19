using System.Windows;
using System.Windows.Data;

namespace MQTT.Sharing.Converters
{
    public class NullCheckValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            if (targetType == typeof(bool))
            {
                bool nullValue;
                bool notNullValue;

                if (!bool.TryParse(parameter as string, out nullValue))
                {
                    nullValue = true;
                }
                notNullValue = !nullValue;
                if (value is int)
                {
                    return ((int)value == 0) ? nullValue : notNullValue;
                }
                else
                {
                    return (value == null) ? nullValue : notNullValue;
                }
            }
            if (targetType == typeof(Visibility))
            {
                Visibility nullValue;
                Visibility notNullValue;

                if (designTime) return Visibility.Visible; // Per poter visualizzare, in design time, tutti i controlli

                if (!Enum.TryParse<Visibility>(parameter as string, out nullValue))
                {
                    nullValue = Visibility.Collapsed;
                }
                if (nullValue == Visibility.Visible)
                {
                    notNullValue = Visibility.Collapsed;
                }
                else
                {
                    notNullValue = Visibility.Visible;
                }
                if (value is int)
                {
                    return ((int)value == 0) ? nullValue : notNullValue;
                }
                else
                {
                    if (value is string)
                    {
                        return String.IsNullOrWhiteSpace(value as string) ? nullValue : notNullValue;
                    }
                    else
                        return (value == null) ? nullValue : notNullValue;
                }
            }
            return !(value == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
