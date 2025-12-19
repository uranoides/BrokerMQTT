using System.Reflection;
using System.Security.Policy;
using System.Windows;

namespace MQTT.Subscriber
{
    public partial class SubscriberWindow : Window
    {
        #region Builder
        public SubscriberWindow()
        {
            InitializeComponent();

            Title = "Broker Subscriber V" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        }
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await Subscriber.Start();
        }
        #endregion
    }
}
