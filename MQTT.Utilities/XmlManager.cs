using System.Reflection;

namespace MQTT.Utilities
{
    public static class XmlManager
    {
        public static Stream GetXmlConfigurationStream(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(fileName);
        }

    }
}
