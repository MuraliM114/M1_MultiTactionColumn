using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M1.Utilities
{
    public class UIDebug : SingletonBehaviour<UIDebug>
    {
        public static bool useUIDebug = true;
        public bool useStackTrace = false;
        public float logTime = 8;

        public Color color = Color.white;
        private string printString = "";
        private List<string> printList = new List<string>();

        void Awake()
        {
            DontDestroy();
        }

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }
        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        void Log(string _message, string stackTrace, LogType type)
        {
            string logString = _message;

            if (useStackTrace)
            {
                logString = "<b><size=14>" + _message + "</size></b>" + "\n<i><size=12>" + type + "\n" + stackTrace + "</size></i>";
            }

            StartCoroutine(Instance.AddPrint(logString));
        }

        IEnumerator AddPrint(string _message)
        {
            printList.Add(_message);
            yield return new WaitForSeconds(logTime);
            printList.Remove(_message);
        }

        void OnGUI()
        {
            if (useUIDebug)
            {
                GUI.color = color;

                // order by newest on top
                printString = "";
                for (int i = printList.Count - 1; i >= 0; i--)
                {
                    printString += printList[i] + "\n";
                }

                // trim string 
                if (printString.Length > 10000)
                {
                    printString = printString.Remove(10000, printString.Length - 10000);
                }
                
                GUI.Label(new Rect(10, 10, Screen.width - 20, Screen.height - 40), printString);
            }
        }
    }
}