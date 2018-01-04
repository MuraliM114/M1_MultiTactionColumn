using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace M1.Utilities
{
    public enum TEST_ENUM
    {
        one = 1,
        two = 2,
        three = 3,
    };

    [System.Serializable]
    public class ContainerData
    {
        public TEST_ENUM testEnum;

        public int intValue;
        public float floatValue;
        public string stringValue;

        [XmlArray("Text"), XmlArrayItem("T")]
        public List<string> Text = new List<string>();
    }
}