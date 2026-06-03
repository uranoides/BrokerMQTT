using MQTT.Sharing.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MQTT.Sharing.Controls
{
    public partial class DisplayBlebLed : UserControl
    {
        #region Variables
        private DispatcherTimer updateTimer;
        #endregion

        #region DependencyProperty
        public BlebSensor BlebSensor
        {
            get { return (BlebSensor)GetValue(BlebSensorProperty); }
            set { SetValue(BlebSensorProperty, value); }
        }
        public static readonly DependencyProperty BlebSensorProperty =
            DependencyProperty.Register("BlebSensor", typeof(BlebSensor), typeof(DisplayBlebLed), new PropertyMetadata(null, OnBlebSensorChanged));

        public int ElapsedSeconds
        {
            get { return (int)GetValue(ElapsedSecondsProperty); }
            set { SetValue(ElapsedSecondsProperty, value); }
        }
        public static readonly DependencyProperty ElapsedSecondsProperty =
            DependencyProperty.Register("ElapsedSeconds", typeof(int), typeof(DisplayBlebLed), new PropertyMetadata(0));

        public bool IsSubscribed
        {
            get { return (bool)GetValue(IsSubscribedProperty); }
            set { SetValue(IsSubscribedProperty, value); }
        }
        public static readonly DependencyProperty IsSubscribedProperty =
            DependencyProperty.Register("IsSubscribed", typeof(bool), typeof(DisplayBlebLed), new PropertyMetadata(false, OnIsSubscribedChanged));

        public double CircleSize
        {
            get { return (double)GetValue(CircleSizeProperty); }
            set { SetValue(CircleSizeProperty, value); }
        }
        public static readonly DependencyProperty CircleSizeProperty =
            DependencyProperty.Register("CircleSize", typeof(double), typeof(DisplayBlebLed), new PropertyMetadata(60.0));

        public double LocationFontSize
        {
            get { return (double)GetValue(LocationFontSizeProperty); }
            set { SetValue(LocationFontSizeProperty, value); }
        }
        public static readonly DependencyProperty LocationFontSizeProperty =
            DependencyProperty.Register("LocationFontSize", typeof(double), typeof(DisplayBlebLed), new PropertyMetadata(21.0));

        public double TimerFontSize
        {
            get { return (double)GetValue(TimerFontSizeProperty); }
            set { SetValue(TimerFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TimerFontSizeProperty =
            DependencyProperty.Register("TimerFontSize", typeof(double), typeof(DisplayBlebLed), new PropertyMetadata(12.0));

        public double SensorIdFontSize
        {
            get { return (double)GetValue(SensorIdFontSizeProperty); }
            set { SetValue(SensorIdFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SensorIdFontSizeProperty =
            DependencyProperty.Register("SensorIdFontSize", typeof(double), typeof(DisplayBlebLed), new PropertyMetadata(12.0));
        #endregion

        #region Builder
        public DisplayBlebLed()
        {
            InitializeComponent();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1);
            updateTimer.Tick += UpdateTimer_Tick;
        }
        #endregion

        private static void OnIsSubscribedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DisplayBlebLed)d;

            if (!(bool)e.NewValue)
            {
                control.Dispatcher.Invoke(control.StopUpdateTimer, DispatcherPriority.Normal);
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (BlebSensor != null && BlebSensor.IsTimerRunning && BlebSensor.TimerStartedAt.HasValue)
                ElapsedSeconds = (int)(DateTime.Now - BlebSensor.TimerStartedAt.Value).TotalSeconds;
            else
                ElapsedSeconds++;
        }
        private void StartUpdateTimer()
        {
            if (BlebSensor != null && BlebSensor.TimerStartedAt.HasValue)
                ElapsedSeconds = (int)(DateTime.Now - BlebSensor.TimerStartedAt.Value).TotalSeconds;
            else
                ElapsedSeconds = 0;
            TimerText.Visibility = Visibility.Visible;
            if (!updateTimer.IsEnabled)
                updateTimer.Start();
        }

        private void StopUpdateTimer()
        {
            if (updateTimer != null && updateTimer.IsEnabled)
            {
                updateTimer.Stop();
            }
            if (TimerText != null)
            {
                TimerText.Visibility = Visibility.Collapsed;
            }
            ElapsedSeconds = 0;
        }

        #region Methods
        private static void OnBlebSensorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DisplayBlebLed)d;

            if (e.OldValue is INotifyPropertyChanged oldSensor)
            {
                oldSensor.PropertyChanged -= control.BlebSensor_PropertyChanged;
            }

            if (e.NewValue is INotifyPropertyChanged newSensor)
            {
                newSensor.PropertyChanged += control.BlebSensor_PropertyChanged;
            }

            if (e.NewValue is BlebSensor sensor && sensor.IsTimerRunning && sensor.TimerStartedAt.HasValue)
            {
                control.ElapsedSeconds = (int)(DateTime.Now - sensor.TimerStartedAt.Value).TotalSeconds;
                control.TimerText.Visibility = Visibility.Visible;
                if (!control.updateTimer.IsEnabled)
                    control.updateTimer.Start();
            }
        }
        private void BlebSensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsSubscribed)
            {
                this.Dispatcher.Invoke(StopUpdateTimer, DispatcherPriority.Normal);
                return;
            }
            if (sender is BlebSensor sensor)
            {
                if (e.PropertyName == nameof(BlebSensor.Sensor_Status) && sensor.Sensor_Status == "Offline")
                {
                    this.Dispatcher.Invoke(StopUpdateTimer, DispatcherPriority.Normal);
                    return;
                }
                if (e.PropertyName == nameof(BlebSensor.IsTimerRunning))
                {
                    if (sensor.IsTimerRunning)
                        this.Dispatcher.Invoke(StartUpdateTimer, DispatcherPriority.Render);
                    else
                        this.Dispatcher.Invoke(StopUpdateTimer, DispatcherPriority.Normal);
                }
            }
        }
        private void StartBlinkAnimation()
        {
            if (BlebSensor == null || LocationText == null) return;

            StartUpdateTimer();
        }
        #endregion
    }
}
