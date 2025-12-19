using MQTT.Sharing.Models;
using System.Windows;
using System.Windows.Controls;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayConnectionSettingsPanel : UserControl
    {
        #region DependencyProperty
        public ConnectionSettings ConnectionSettings
        {
            get { return (ConnectionSettings)GetValue(ConnectionSettingsProperty); }
            set { SetValue(ConnectionSettingsProperty, value); }
        }
        public static readonly DependencyProperty ConnectionSettingsProperty =
            DependencyProperty.Register(nameof(ConnectionSettings), typeof(ConnectionSettings), typeof(DisplayConnectionSettingsPanel),
                new PropertyMetadata(null));
        #endregion

        #region Builder
        public DisplayConnectionSettingsPanel()
        {
            InitializeComponent();
        }
        #endregion
    }
}
