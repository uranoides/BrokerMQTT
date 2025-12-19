using MQTT.Sharing.Models;
using System.Windows;
using System.Windows.Controls;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayBlebSensors : UserControl
    {
        #region DependencyProperty
        public List<BlebSensor> BlebSensors
        {
            get { return (List<BlebSensor>)GetValue(BlebSensorsProperty); }
            set { SetValue(BlebSensorsProperty, value); }
        }
        public static readonly DependencyProperty BlebSensorsProperty =
            DependencyProperty.Register("BlebSensors", typeof(List<BlebSensor>), typeof(DisplayBlebSensors),
                new PropertyMetadata(null, OnPropertiesChanged));

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(DisplayBlebSensors),
                new PropertyMetadata(0, OnPropertiesChanged));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(DisplayBlebSensors),
                new PropertyMetadata(0, OnPropertiesChanged));

        public double LedCircleSize
        {
            get { return (double)GetValue(LedCircleSizeProperty); }
            set { SetValue(LedCircleSizeProperty, value); }
        }
        public static readonly DependencyProperty LedCircleSizeProperty =
            DependencyProperty.Register(nameof(LedCircleSize), typeof(double), typeof(DisplayBlebSensors),
                new PropertyMetadata(60.0));

        public double LedLocationFontSize
        {
            get { return (double)GetValue(LedLocationFontSizeProperty); }
            set { SetValue(LedLocationFontSizeProperty, value); }
        }
        public static readonly DependencyProperty LedLocationFontSizeProperty =
           DependencyProperty.Register(nameof(LedLocationFontSize), typeof(double), typeof(DisplayBlebSensors),
               new PropertyMetadata(21.0));

        public double LedTimerFontSize
        {
            get { return (double)GetValue(LedTimerFontSizeProperty); }
            set { SetValue(LedTimerFontSizeProperty, value); }
        }
        public static readonly DependencyProperty LedTimerFontSizeProperty =
            DependencyProperty.Register(nameof(LedTimerFontSize), typeof(double), typeof(DisplayBlebSensors),
                new PropertyMetadata(12.0));

        public double LedSensorIdFontSize
        {
            get { return (double)GetValue(LedSensorIdFontSizeProperty); }
            set { SetValue(LedSensorIdFontSizeProperty, value); }
        }
        public static readonly DependencyProperty LedSensorIdFontSizeProperty =
            DependencyProperty.Register(nameof(LedSensorIdFontSize), typeof(double), typeof(DisplayBlebSensors),
                new PropertyMetadata(12.0));
        #endregion

        #region Builder
        public DisplayBlebSensors()
        {
            InitializeComponent();
        }
        #endregion

        #region PropertiesChanged
        private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplayBlebSensors control = d as DisplayBlebSensors;
            if (control != null)
            {
                control.UpdateLayout();
            }
        }
        #endregion
    }
}
