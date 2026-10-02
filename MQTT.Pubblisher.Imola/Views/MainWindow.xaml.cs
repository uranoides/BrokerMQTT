using System.Reflection;
using System.Windows;

namespace MQTT.Pubblisher.Imola
{
    public partial class MainWindow : Window
    {
        #region Builder
        public MainWindow()
        {
            InitializeComponent();

            Title = "Imola Publisher V" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        }
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await Main.Start();
        }
        #endregion
    }
}