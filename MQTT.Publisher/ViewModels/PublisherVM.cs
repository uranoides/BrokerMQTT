using MQTT.Sharing.Models;
using MQTTnet;
using System.ComponentModel;

namespace MQTT.Publisher.ViewModels
{
    public class PublisherVM : INotifyPropertyChanged
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Error
        public delegate void ErrorHandler(string ErrorMessage, bool Silent = false);
        public event ErrorHandler Error;
        #endregion

        #region Progress
        public delegate void ProgressHandler(string Message);
        public event ProgressHandler Progress;
        #endregion

        #region Variables
        private IMqttClient mqttClient;
        private ConnectionSettings connectionSettings;
        #endregion

        #region Properties
        private List<BlebSensor> blebSensors;
        public List<BlebSensor> BlebSensors
        {
            get { return blebSensors; }
            set
            {
                blebSensors = value;
                OnPropertyChanged(nameof(BlebSensors));
            }
        }
        private List<BlebSensor> blebSensorsPayloads;
        public List<BlebSensor> BlebSensorsPayloads
        {
            get { return blebSensorsPayloads; }
            set
            {
                blebSensorsPayloads = value;
                OnPropertyChanged(nameof(BlebSensorsPayloads));
            }
        }
        private BlebSensor selectedBlebSensor;
        public BlebSensor SelectedBlebSensor
        {
            get { return selectedBlebSensor; }
            set
            {
                selectedBlebSensor = value;
                OnPropertyChanged(nameof(SelectedBlebSensor));
                //UpdateJsonDisplay();
            }
        }
        #endregion

        #region Builder
        public PublisherVM()
        {
            InitProcedures();
        }
        private async void InitProcedures()
        {
            await GetBlebSensorsAsync();

            BlebSensorsPayloads = new List<BlebSensor>();
            InitializeTaskTimer();
            InitializeUiTimer();
        }
        #endregion

        #region Methods
        private async Task GetBlebSensorsAsync()
        {
            JsonManager jsonManager = new JsonManager();
            connectionSettings = await jsonManager.GetConnectionByIdAsync(7);
            var reader = new ConfigurationXmlReader();
            TagVariables = reader.ReadVariables(connectionSettings.TagVariablesFileName);
            BlebSensorsAll = new List<BlebSensor>();
            foreach (var tag in TagVariables)
            {
                Global.CustomData config = CustomDataDeserializer.DeserializeFromXmlAttribute(tag.CustomData);
                BlebSensorsAll.Add(new BlebSensor()
                {
                    Topic = tag.Address,
                    PlaceId = tag.Id,
                    Sensor_Location = config.Sensor,
                    Sensor_ID = EnumRandomizer.GetRandomAlphanumeric(),
                    Sensor_Type = EnumRandomizer.GetRandomSensorType().ToString(),
                    Sensor_Area = EnumRandomizer.GetRandomSensorLocation().ToString(),
                    Sensor_Status = "Offline",
                    Sensor_Value = 0,
                    Presence = false,
                });
            }
        }
        #endregion

    }
}
