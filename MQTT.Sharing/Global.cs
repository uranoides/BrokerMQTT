namespace MQTT.Sharing
{
    public static class Global
    {
        #region Topics
        public static List<string> TestTopics = new List<string>
        {
            "blebimola/palletizer1",
            "blebimola/palletizer2",
            "blebimola/unload_udc_01",
            "blebimola/unload_udc_02",
            "blebimola/unload_udc_03",
        };
        public static string GetRandomTestTopic()
        {
            int randomIndex = Random.Shared.Next(TestTopics.Count);

            return TestTopics[randomIndex];
        }
        #endregion
    }
}
