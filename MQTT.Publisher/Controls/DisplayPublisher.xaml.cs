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
        #endregion

        #region Variables
        private Task LastErrorTask = null;
        private Task LastTextLeftUpNotificationTask = null;
        private Task LastTextLeftDownNotificationTask = null;
        private string LastErrorMessage = null;
        #endregion

        #region Progress
        private async void UpdateTextLeftUpNotification(string Message)
        {
            if (Application.Current == null) return;

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LastTextLeftUpNotification = $"{DateTime.Now.ToLongTimeString()} - {Message}";
                });

                var currentTask = Task.Delay(TimeSpan.FromSeconds(Properties.Settings.Default.NotificationTime));
                LastTextLeftUpNotificationTask = currentTask;

                await currentTask;

                if (Application.Current != null && LastTextLeftUpNotificationTask == currentTask)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LastTextLeftUpNotification = null;
                    });
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("Notifica interrotta dalla chiusura dell'applicazione.");
            }
        }
        private async void UpdateTextLeftDownNotification(string Message)
        {
            if (Application.Current == null) return;

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LastTextLeftDownNotification = $"{DateTime.Now.ToLongTimeString()} - {Message}";
                });

                var currentTask = Task.Delay(TimeSpan.FromSeconds(Properties.Settings.Default.NotificationTime));
                LastTextLeftDownNotificationTask = currentTask;

                await currentTask;

                if (Application.Current != null && LastTextLeftDownNotificationTask == currentTask)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LastTextLeftDownNotification = null;
                    });
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("Notifica interrotta dalla chiusura dell'applicazione.");
            }
        }
        private void OnTextLeftUpVMProgress(string Message)
        {
            UpdateTextLeftUpNotification(Message);
        }
        private void OnTextLeftDownVMProgress(string Message)
        {
            UpdateTextLeftDownNotification(Message);
        }
        //private void OnTextRightUpVMProgress(string Message)
        //{
        //    UpdateProgressNotification(Message);
        //}
        //private void OnTextRightDownVMProgress(string Message)
        //{
        //    UpdateProgressNotification(Message);
        //}
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
        private async void WriteTopicsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                if (PublisherVM != null)
                    PublisherVM.ToggleTimerLogic();
            }
            catch (Exception ex)
            {
                await OnError(ex.Message);
            }
        }

        private void WriteTopicsCanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (PublisherVM != null) 
            {
                if(PublisherVM.SelectedConnectionSettings != null)
                    e.CanExecute = true; 
            }
        }
        #endregion

        #region ToggleButtons
        private void ConnSett_Checked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeIn");
            if (Anima != null) { Anima.Begin(); }
            OnTextLeftUpVMProgress("Connection Settings Panel Open..");
        }

        private void ConnSett_Unchecked(object sender, RoutedEventArgs e)
        {
            Storyboard Anima = (Storyboard)TryFindResource("ConnectionSettingsFadeOut");
            if (Anima != null) { Anima.Begin(); }
            OnTextLeftDownVMProgress("Connection Settings Panel Close..");
        }
        #endregion
    }
}
