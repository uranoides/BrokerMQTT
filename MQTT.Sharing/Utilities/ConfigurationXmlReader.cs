using MQTT.Sharing.Models;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace MQTT.Sharing.Utilities
{
    public static class XmlManager
    {
        public static Stream GetXmlConfigurationStream(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(fileName);
        }
    }
    public class ConfigurationXmlReader
    {
        public List<VariableData> ReadVariables(string fileName)
        {
            var variablesList = new List<VariableData>();

            using (Stream xmlStream = XmlManager.GetXmlConfigurationStream(fileName))
            {
                if (xmlStream == null)
                {
                    Console.WriteLine("Errore: Impossibile trovare la risorsa XML incorporata.");
                    return variablesList;
                }

                try
                {
                    XDocument doc = XDocument.Load(xmlStream);

                    var groups = doc.Descendants("Group");

                    foreach (var group in groups)
                    {
                        string groupName = group.Attribute("Name")?.Value ?? "Sconosciuto";
                        int id = Convert.ToInt32(group.Attribute("Index")?.Value);

                        var variables = group.Elements("Variable");

                        foreach (var variable in variables)
                        {
                            variablesList.Add(new VariableData
                            {
                                Id = id,
                                GroupName = groupName,
                                VariableName = variable.Attribute("Name").Value,
                                Address = variable.Attribute("Address").Value,
                                Description = variable.Attribute("Description").Value,
                                CustomData = variable.Attribute("CustomData").Value
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante il parsing del file XML: {ex.Message}");
                }
            }

            return variablesList;
        }
    }
}
