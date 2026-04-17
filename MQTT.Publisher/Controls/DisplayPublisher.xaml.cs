using MQTT.Publisher.ViewModels;
using MQTT.Sharing.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MQTT.Publisher.Controls
{
    public partial class DisplayPublisher : UserControl
    {
        #region DependencyProperty
        public PublisherVM PublisherVM
        {
            get { return (PublisherVM)GetValue(SacmImolaVMProperty); }
            set { SetValue(SacmImolaVMProperty, value); }
        }
        public static readonly DependencyProperty SacmImolaVMProperty =
            DependencyProperty.Register(nameof(PublisherVM), typeof(PublisherVM), typeof(DisplayPublisher), new PropertyMetadata(null));

        public string LastTextLeftUpNotification
        {
            get { return (string)GetValue(LastTextLeftUpNotificationProperty); }
            set { SetValue(LastTextLeftUpNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftUpNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftUpNotification), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));

        public string LastTextLeftDownNotification
        {
            get { return (string)GetValue(LastTextLeftDownNotificationProperty); }
            set { SetValue(LastTextLeftDownNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftDownNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftDownNotification), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));

        public string LastTextRightUpNotification
        {
            get { return (string)GetValue(LastTextRightUpNotificationProperty); }
            set { SetValue(LastTextRightUpNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextRightUpNotificationProperty =
            DependencyProperty.Register(nameof(LastTextRightUpNotification), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));

        public string LastTextRightDownNotification
        {
            get { return (string)GetValue(LastTextRightDownNotificationProperty); }
            set { SetValue(LastTextRightDownNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextRightDownNotificationProperty =
            DependencyProperty.Register(nameof(LastTextRightDownNotification), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));
        #endregion

        #region Variables
        private readonly NotificationState _leftUpState = new();
        private readonly NotificationState _leftDownState = new();
        private readonly NotificationState _rightUpState = new();
        private readonly NotificationState _rightDownState = new();
        #endregion

        #region Progress
        private async Task UpdateNotificationGeneric(string message, Action<string> setter, NotificationState state)
        {
            if (Application.Current == null) return;

            state.CTS?.Cancel();
            state.CTS = new CancellationTokenSource();
            var token = state.CTS.Token;

            try
            {
                string formattedMessage = $"{DateTime.Now.ToLongTimeString()} - {message}";

                await Application.Current.Dispatcher.InvokeAsync(() => setter(formattedMessage));

                var delay = TimeSpan.FromSeconds(Properties.Settings.Default.NotificationTime);
                await Task.Delay(delay, token);

                await Application.Current.Dispatcher.InvokeAsync(() => setter(null));
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Notifica sovrascritta.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore: {ex.Message}");
            }
        }
        private void UpdateTextLeftUpNotification(string m) =>
            _ = UpdateNotificationGeneric(m, v => LastTextLeftUpNotification = v, _leftUpState);

        private void UpdateTextLeftDownNotification(string m) =>
            _ = UpdateNotificationGeneric(m, v => LastTextLeftDownNotification = v, _leftDownState);

        private void UpdateTextRightUpNotification(string m) =>
            _ = UpdateNotificationGeneric(m, v => LastTextRightUpNotification = v, _rightUpState);

        private void UpdateTextRightDownNotification(string m) =>
            _ = UpdateNotificationGeneric(m, v => LastTextRightDownNotification = v, _rightDownState);
        #endregion

        #region Builder
        public DisplayPublisher()
        {
            InitializeComponent();
        }
        public async Task Start()
        {
            PublisherVM = FindResource("vm") as PublisherVM;
            if (PublisherVM != null)
            {
                await PublisherVM.LoadAsync();
            }
        }
        #endregion

        #region Commands
        private void WriteTopicsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                if (PublisherVM != null)
                    PublisherVM.ToggleTimerLogic();
            }
            catch (Exception ex)
            {
                UpdateTextLeftDownNotification(ex.Message);
            }
        }

        private void WriteTopicsCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (PublisherVM != null)
            {
                if (PublisherVM.SelectedConnectionSettings != null)
                    e.CanExecute = true;
            }
        }
        private void WriteSingleTopicCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (PublisherVM != null)
            {
                if (PublisherVM.SelectedConnectionSettings != null && !string.IsNullOrEmpty(PublisherVM.SensorNumber))
                    e.CanExecute = true;
            }
        }
        private async void WriteSingleTopicExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                if (PublisherVM != null)
                    await PublisherVM.WriteSingleTopicAsync();
            }
            catch (Exception ex)
            {
                UpdateTextLeftDownNotification(ex.Message);
            }
        }
        #endregion

        #region ToggleButtons
        private void ConnSett_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeIn");
            if (Anima != null) { Anima.Begin(); }
            UpdateTextLeftUpNotification("Connection Settings Panel Open..");
        }

        private void ConnSett_Unchecked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeOut");
            if (Anima != null) { Anima.Begin(); }
            UpdateTextLeftUpNotification("Connection Settings Panel Close..");
        }

        #endregion

        #region Clicks
        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (PublisherVM != null)
                PublisherVM.BlebSensorsPayloads = new List<BlebSensor>();
        }
        #endregion
    }
}
