using UnityEngine;
using System.Collections;
using System.IO;

namespace M1.Utilities
{
    public class VersionNumber : SingletonBehaviour<VersionNumber>
    {
        [SerializeField]
        private bool showVersionNumber = true;
        private string version;
        private Rect position = new Rect(0, 0, 300, 20);

        [SerializeField]
        private float displayTime = 15.0f;
        [SerializeField]
        private Color displayColor = Color.white;

        void Start()
        {
            DontDestroy();
            version = GetVersion();

            // Log current version in log file
            string s = "";
            s += "////////////////////////////////////////////////////////////////////////////////\n";
            s += "// version " + version + " //\n";
            s += "////////////////////////////////////////////////////////////////////////////////\n";
            Debug.Log(s);

            StartCoroutine(GUITimer());

            position.x = Screen.width - 350f;
            position.y = Screen.height - position.height - 10f;
        }

        private string GetVersion()
        {
            StreamReader inStream;
#if UNITY_EDITOR
            inStream = new StreamReader(Application.dataPath + "/Config/VersionNumber.txt");
#else
        inStream = new StreamReader(Application.dataPath + "/../Config/" + "VersionNumber.txt");
#endif
            return inStream.ReadLine();
        }

        IEnumerator GUITimer()
        {
            yield return new WaitForSeconds(displayTime);
            showVersionNumber = false;
        }

        void OnGUI()
        {
            if (showVersionNumber)
            {
                GUI.contentColor = displayColor;
                GUI.Label(position, "v" + version);
            }
        }
    }
}