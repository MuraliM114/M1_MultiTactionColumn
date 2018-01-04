using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace M1.Utilities
{
    [XmlRoot("Root")]
    public class Container
    {
        [XmlArray("Container"), XmlArrayItem("Entry")]
        public List<ContainerData> data = new List<ContainerData>();

        public static string ClientAssetPath(string name)
        {
#if UNITY_EDITOR
            return Application.dataPath + "/ClientAssets/" + name;
#else
        return Application.dataPath + "/../ClientAssets/" + name;
#endif
        }

        public static Container Load(string name)
        {
            var serializer = new XmlSerializer(typeof(Container));
            using (var stream = new StreamReader(ClientAssetPath(name)))
            {
                return serializer.Deserialize(stream) as Container;
            }

            //For iOS, uncomment this - string "name" given should NOT include an extension in it
            //return DeserializeTextAsset<Container>(name);
        }

        #region iOS XML Loading Functions
        //For this to work properly, create a "Resources" folder under the "Assets" folder
        //Rename the .xml file extension to a .txt

        public static T DeserializeTextAsset<T>(string filename)
        {
            TextAsset textAsset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("Could not load text asset " + filename);
            }
            return DeserializeString<T>(textAsset.ToString());
        }
        //filename an object from an XML string.
        public static T DeserializeString<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(xml);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            T obj = (T)serializer.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;
        }
        public static void SerializeObject<T>(string filename, T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, data);
            textWriter.Close();
        }

        #endregion
    }
}