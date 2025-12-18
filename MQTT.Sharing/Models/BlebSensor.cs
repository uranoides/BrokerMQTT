using System.ComponentModel;

namespace MQTT.Sharing.Models
{
    public class BlebSensor : INotifyPropertyChanged
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string topic = string.Empty;
        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                OnPropertyChanged(nameof(Topic));
            }
        }

        private int placeId;
        public int PlaceId
        {
            get { return placeId; }
            set
            {
                placeId = value;
                OnPropertyChanged(nameof(PlaceId));
            }
        }

        private string gateway_ID;
        public string Gateway_ID
        {
            get { return gateway_ID; }
            set
            {
                gateway_ID = value;
                OnPropertyChanged(nameof(Gateway_ID));
            }
        }

        private string sensor_ID;
        public string Sensor_ID
        {
            get { return sensor_ID; }
            set
            {
                sensor_ID = value;
                OnPropertyChanged(nameof(Sensor_ID));
            }
        }

        private string sensor_Type;
        public string Sensor_Type
        {
            get { return sensor_Type; }
            set
            {
                sensor_Type = value;
                OnPropertyChanged(nameof(Sensor_Type));
            }
        }

        private string sensor_Communication;
        public string Sensor_Communication
        {
            get { return sensor_Communication; }
            set
            {
                sensor_Communication = value;
                OnPropertyChanged(nameof(Sensor_Communication));
            }
        }

        private string sensor_Area;
        public string Sensor_Area
        {
            get { return sensor_Area; }
            set
            {
                sensor_Area = value;
                OnPropertyChanged(nameof(Sensor_Area));
            }
        }

        private string sensor_Location;
        public string Sensor_Location
        {
            get { return sensor_Location; }
            set
            {
                sensor_Location = value;
                OnPropertyChanged(nameof(Sensor_Location));
            }
        }

        private string sensor_Status;
        public string Sensor_Status
        {
            get { return sensor_Status; }
            set
            {
                sensor_Status = value;
                OnPropertyChanged(nameof(Sensor_Status));
            }
        }

        private long sensor_Threshold;
        public long Sensor_Threshold
        {
            get { return sensor_Threshold; }
            set
            {
                sensor_Threshold = value;
                OnPropertyChanged(nameof(Sensor_Threshold));
            }
        }

        private long sensor_Value;
        public long Sensor_Value
        {
            get { return sensor_Value; }
            set
            {
                sensor_Value = value;
                OnPropertyChanged(nameof(Sensor_Value));
            }
        }

        private bool? presence;
        public bool? Presence
        {
            get { return presence; }
            set
            {
                presence = value;
                OnPropertyChanged(nameof(Presence));
            }
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }
    }
}
