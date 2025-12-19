using System.Windows;
using System.Windows.Controls;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayStatusIndicator : UserControl
    {
        #region DependencyProperty
        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register(nameof(IsConnected), typeof(bool), typeof(DisplayStatusIndicator),
                new PropertyMetadata(false));
        #endregion

        #region Builder
        public DisplayStatusIndicator()
        {
            InitializeComponent();
        }
        #endregion
    }
}
