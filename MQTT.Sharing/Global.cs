namespace MQTT.Sharing
{
    public static class Global
    {
        #region Topics
        public static List<string> TestTopics = new List<string>
        {
            "blebimola/test1",
        };
        public static string GetRandomTestTopic()
        {
            int randomIndex = Random.Shared.Next(TestTopics.Count);

            return TestTopics[randomIndex];
        }
        #endregion
    }
}
