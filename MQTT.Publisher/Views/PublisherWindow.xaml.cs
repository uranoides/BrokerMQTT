using System.Windows;

namespace MQTT.Publisher.Views
{
    public partial class PublisherWindow : Window
    {
        #region Builder
        public PublisherWindow()
        {
            InitializeComponent();
        }
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await Publisher.Start();
        }
        #endregion
    }
}
