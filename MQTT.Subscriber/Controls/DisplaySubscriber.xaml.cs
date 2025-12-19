using MQTT.Sharing.Models;
using MQTT.Subscriber.ViewModels;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MQTT.Subscriber.Controls
{
    public partial class DisplaySubscriber : UserControl
    {
        #region DependencyProperty
        public SubscriberVM SubscriberVM
        {
            get { return (SubscriberVM)GetValue(SacmImolaVMProperty); }
            set { SetValue(SacmImolaVMProperty, value); }
        }
        public static readonly DependencyProperty SacmImolaVMProperty =
            DependencyProperty.Register(nameof(SubscriberVM), typeof(SubscriberVM), typeof(DisplaySubscriber), new PropertyMetadata(null));

        public string LastError
        {
            get { return (string)GetValue(LastErrorProperty); }
            set { SetValue(LastErrorProperty, value); }
        }
        public static readonly DependencyProperty LastErrorProperty =
            DependencyProperty.Register(nameof(LastError), typeof(string), typeof(DisplaySubscriber), new PropertyMetadata(null));

        public string LastTextLeftUpNotification
        {
            get { return (string)GetValue(LastTextLeftUpNotificationProperty); }
            set { SetValue(LastTextLeftUpNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftUpNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftUpNotification), typeof(string), typeof(DisplaySubscriber), new PropertyMetadata(null));

        public string LastTextLeftDownNotification
        {
            get { return (string)GetValue(LastTextLeftDownNotificationProperty); }
            set { SetValue(LastTextLeftDownNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftDownNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftDownNotification), typeof(string), typeof(DisplaySubscriber), new PropertyMetadata(null));

        public string LastTextRightUpNotification
        {
            get { return (string)GetValue(LastTextRightUpNotificationProperty); }
            set { SetValue(LastTextRightUpNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextRightUpNotificationProperty =
            DependencyProperty.Register(nameof(LastTextRightUpNotification), typeof(string), typeof(DisplaySubscriber), new PropertyMetadata(null));

        public string LastTextRightDownNotification
        {
            get { return (string)GetValue(LastTextRightDownNotificationProperty); }
            set { SetValue(LastTextRightDownNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextRightDownNotificationProperty =
            DependencyProperty.Register(nameof(LastTextRightDownNotification), typeof(string), typeof(DisplaySubscriber), new PropertyMetadata(null));
        #endregion

        #region Variables
        private Task LastErrorTask = null;
        private string LastErrorMessage = null;
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

        #region Error
        public async void OnSubscriberVMError(string Message, bool Silent)
        {
            await OnError(Message, Silent);
        }
        public async Task OnError(string errorMessage, bool Silent = false)
        {
            if (errorMessage.Length > 1024)
            {
                errorMessage = errorMessage.Substring(0, 1024);
            }
            if (!Silent)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    LastError = errorMessage;

                    LastErrorTask = Task.Delay(new TimeSpan(0, 0, Properties.Settings.Default.NotificationTime));
                    LastErrorTask.ContinueWith((t) =>
                    {
                        if (LastErrorTask == t) LastError = null;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                });
            }
            if (errorMessage != LastErrorMessage) { LastErrorMessage = errorMessage; }
        }
        #endregion

        #region
        public DisplaySubscriber()
        {
            InitializeComponent();
        }
        public async Task Start()
        {
            SubscriberVM = FindResource("vm") as SubscriberVM;
            if (SubscriberVM != null)
            {
                await SubscriberVM.LoadAsync();
            }
        }
        #endregion

        #region Commands
        private void ReadTopicsCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (SubscriberVM != null)
            {
                if (SubscriberVM.SelectedConnectionSettings != null)
                    e.CanExecute = true;
            }
        }

        private async void ReadTopicsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                if (SubscriberVM != null)
                    SubscriberVM.ToggleTimerBleb();
            }
            catch (Exception ex)
            {
                await OnError(ex.Message);
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
    }
}
