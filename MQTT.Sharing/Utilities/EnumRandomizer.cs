using MQTT.Sharing.Enumerations;

namespace MQTT.Sharing.Utilities
{
    public static class EnumRandomizer
    {
        public static BlebSensorType GetRandomBlebSensorType()
        {
            Array values = Enum.GetValues(typeof(BlebSensorType));
            Random random = new Random();
            int randomIndex = random.Next(values.Length);
            return (BlebSensorType)values.GetValue(randomIndex);
        }
        public static BlebSensorLocation GetRandomSensorLocation()
        {
            Array values = Enum.GetValues(typeof(BlebSensorLocation));
            Random random = new Random();
            int randomIndex = random.Next(values.Length);
            return (BlebSensorLocation)values.GetValue(randomIndex);
        }
        public static string GetRandomStatusString()
        {
            BlebStatus[] statuses = (BlebStatus[])Enum.GetValues(typeof(BlebStatus));
            Random random = new Random();
            int randomIndex = random.Next(statuses.Length);
            BlebStatus randomStatus = statuses[randomIndex];

            return randomStatus.ToString();
        }
        public static int GetRandomInt(int max)
        {
            Random random = new Random();
            return random.Next(0, max);
        }
        public static string GetRandomAlphanumeric(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }
}
