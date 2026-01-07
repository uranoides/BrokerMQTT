namespace MQTT.Sharing
{
    public static class Global
    {
        #region Topics
        public static List<string> TestTopics = new List<string>
        {
            "blebimola/unload_udc_01",
            "blebimola/unload_udc_02",
        };
        public static string GetRandomTestTopic()
        {
            int randomIndex = Random.Shared.Next(TestTopics.Count);

            return TestTopics[randomIndex];
        }
        #endregion
    }
}
