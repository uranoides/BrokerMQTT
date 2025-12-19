using MQTT.Publisher.ViewModels;
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

        public string LastError
        {
            get { return (string)GetValue(LastErrorProperty); }
            set { SetValue(LastErrorProperty, value); }
        }
        public static readonly DependencyProperty LastErrorProperty =
            DependencyProperty.Register(nameof(LastError), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));

        public string LastStatusNotification
        {
            get { return (string)GetValue(LastStatusNotificationProperty); }
            set { SetValue(LastStatusNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastStatusNotificationProperty =
            DependencyProperty.Register(nameof(LastStatusNotification), typeof(string), typeof(DisplayPublisher), new PropertyMetadata(null));
        #endregion

        #region Variables
        private Task LastErrorTask = null;
        private Task LastStatusNotificationTask = null;
        private string LastErrorMessage = null;
        #endregion

        #region Progress
        private async void UpdateProgressNotification(string Message)
        {
            if (Application.Current == null) return;

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LastStatusNotification = $"{DateTime.Now.ToLongTimeString()} - {Message}";
                });

                var currentTask = Task.Delay(TimeSpan.FromSeconds(5));
                LastStatusNotificationTask = currentTask;

                await currentTask;

                if (Application.Current != null && LastStatusNotificationTask == currentTask)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LastStatusNotification = null;
                    });
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("Notifica interrotta dalla chiusura dell'applicazione.");
            }
        }
        private void OnPublisherVMProgress(string Message)
        {
            UpdateProgressNotification(Message);
        }
        #endregion

        #region Error
        public async void OnPublisherVMError(string Message, bool Silent)
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

        #region Builder
        public DisplayPublisher()
        {
            InitializeComponent();

            PublisherVM = FindResource("vm") as PublisherVM;
        }
        #endregion

        #region Commands
        private async void WriteTopicsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                //if (PublisherVM != null)
                //    PublisherVM.ToggleTimerLogic();
            }
            catch (Exception ex)
            {
                await OnError(ex.Message);
            }
        }

        private void WriteTopicsCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (PublisherVM != null) { e.CanExecute = true; }
        }
        #endregion

        #region ToggleButtons
        private void ConnSett_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeIn");
            if (Anima != null) { Anima.Begin(); }
            OnPublisherVMProgress("Connection Settings Panel Open..");
        }

        private void ConnSett_Unchecked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeOut");
            if (Anima != null) { Anima.Begin(); }
            OnPublisherVMProgress("Connection Settings Panel Close..");
        }
        #endregion
    }
}
