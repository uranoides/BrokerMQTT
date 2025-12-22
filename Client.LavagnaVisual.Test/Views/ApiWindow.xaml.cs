using Bridge.Models;
using Client.LavagnaVisual.Test.Services;
using System.Windows;

namespace Client.LavagnaVisual.Test.Views
{
    public partial class ApiWindow : Window
    {
        #region DependencyProperty
        public string LastError
        {
            get { return (string)GetValue(LastErrorProperty); }
            set { SetValue(LastErrorProperty, value); }
        }
        public static readonly DependencyProperty LastErrorProperty =
            DependencyProperty.Register("LastError", typeof(string), typeof(ApiWindow), new PropertyMetadata(null));

        public string LastStatusNotification
        {
            get { return (string)GetValue(LastStatusNotificationProperty); }
            set { SetValue(LastStatusNotificationProperty, value); }
        }
        public static readonly DependencyProperty LastStatusNotificationProperty =
            DependencyProperty.Register("LastStatusNotification", typeof(string), typeof(ApiWindow), new PropertyMetadata(null));
        #endregion

        #region Variables
        private Task LastErrorTask = null;
        private Task LastStatusNotificationTask = null;
        private string LastErrorMessage = null;
        private readonly ApiService _apiService = new ApiService();
        #endregion

        #region Error
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
                    LastError = DateTime.Now.ToLongTimeString() + " - " + errorMessage;

                    LastErrorTask = Task.Delay(new TimeSpan(0, 0, Properties.Settings.Default.LastNotificationTime));
                    LastErrorTask.ContinueWith((t) =>
                    {
                        if (LastErrorTask == t) LastError = null;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                });
            }
            if (errorMessage != LastErrorMessage) { LastErrorMessage = errorMessage; }
        }
        #endregion

        #region Progress
        private void OnNotification(string Message)
        {
            Dispatcher.Invoke(() =>
            {
                LastStatusNotification = DateTime.Now.ToLongTimeString() + " - " + Message;

                LastStatusNotificationTask = Task.Delay(new TimeSpan(0, 0, Properties.Settings.Default.LastNotificationTime));
                LastStatusNotificationTask.ContinueWith((t) =>
                {
                    if (LastStatusNotificationTask == t) LastStatusNotification = null;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }
        #endregion

        #region Builder
        public ApiWindow()
        {
            InitializeComponent();

            _apiService.OnErrorOccurred += async (s, msg) => await OnError(msg);
            _apiService.OnMessageOccurred += (s, msg) => OnNotification(msg);
        }
        #endregion  

        #region Commands
        private async void GetRecipesExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                dgRecipes.ItemsSource = await _apiService.GetRecipesAsync();
            }
            catch (Exception ex)
            {
                await OnError(ex.Message);
            }
        }
        #endregion
    }
}
