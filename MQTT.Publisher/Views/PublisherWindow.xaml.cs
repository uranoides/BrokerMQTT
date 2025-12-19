using System.Reflection;
using System.Windows;

namespace MQTT.Publisher.Views
{
    public partial class PublisherWindow : Window
    {
        #region Builder
        public PublisherWindow()
        {
            InitializeComponent();

            Title = "Broker Publisher V" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        }
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await Publisher.Start();
        }
        #endregion
    }
}
