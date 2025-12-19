using MQTT.Sharing.Models;
using System.Windows;
using System.Windows.Controls;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayBlebSensorListItem : UserControl
    {
        #region DependencyProperty
        public BlebSensor BlebSensor
        {
            get { return (BlebSensor)GetValue(BlebSensorProperty); }
            set { SetValue(BlebSensorProperty, value); }
        }
        public static readonly DependencyProperty BlebSensorProperty =
            DependencyProperty.Register(nameof(BlebSensor), typeof(BlebSensor), typeof(DisplayBlebSensorListItem), new PropertyMetadata(null));
        #endregion

        #region Builder
        public DisplayBlebSensorListItem()
        {
            InitializeComponent();
        }
        #endregion
    }
}
