using MQTT.Pubblisher.Imola.ViewModels;
using MQTT.Sharing.Models;
using System.Windows;
using System.Windows.Controls;

namespace MQTT.Pubblisher.Imola.Controls
{
    public partial class DisplayMain : UserControl
    {
        #region DependencyProperty
        public MainVM MainVM
        {
            get { return (MainVM)GetValue(MainVMProperty); }
            set { SetValue(MainVMProperty, value); }
        }
        public static readonly DependencyProperty MainVMProperty =
            DependencyProperty.Register(nameof(MainVM), typeof(MainVM), typeof(DisplayMain), new PropertyMetadata(null));

        public string LastTextLeftUpNotification
        {
            get { return (string)GetValue(LastTextLeftUpNotificationProperty); }
            set { SetValue(LastTextLeftUpNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftUpNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftUpNotification), typeof(string), typeof(DisplayMain), new PropertyMetadata(null));

        public string LastTextLeftDownNotification
        {
            get { return (string)GetValue(LastTextLeftDownNotificationProperty); }
            set { SetValue(LastTextLeftDownNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastTextLeftDownNotificationProperty =
            DependencyProperty.Register(nameof(LastTextLeftDownNotification), typeof(string), typeof(DisplayMain), new PropertyMetadata(null));
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

                var delay = TimeSpan.FromSeconds(5);
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
        #endregion

        #region Variables
        private readonly NotificationState _leftUpState = new();
        private readonly NotificationState _leftDownState = new();
        #endregion

        #region Builder
        public DisplayMain()
        {
            InitializeComponent();
        }
        public async Task Start()
        {
            MainVM = FindResource("vm") as MainVM;
            if (MainVM != null)
            {
                await MainVM.LoadAsync();
            }
        }
        #endregion

        #region Commands
        private void WriteTopicsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                if (MainVM != null)
                    MainVM.ToggleTimerLogic();
            }
            catch (Exception ex)
            {
                UpdateTextLeftDownNotification(ex.Message);
            }
        }

        private void WriteTopicsCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
    }
}
