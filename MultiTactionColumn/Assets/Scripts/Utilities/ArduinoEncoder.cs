//using UnityEngine;
//using System.Collections;
//using System;
//using System.IO;
//using System.IO.Ports;
//using System.Threading;

//namespace M1.Utilities
//{
//    public class ArduinoEncoder : SingletonBehaviour<ArduinoEncoder>
//    {
//        private int baudRate = 9600;

//        string inData = "";
//        int encoderData = 0;
//        int prevEncoderData = 0;
//        SerialPort stream;
//        Thread dataStreamThread;
//        bool threadOpen = false;

//        int prevButton1State = 0;
//        int button1State = 0;

//        string prevDefaultString = "";
//        string defaultString = "";

//        // debug
//        string availablePorts = "Ports:";

//        void Awake()
//        {
//            DontDestroy();
//        }

//        IEnumerator Start()
//        {
//            string s = "";
//            s += "////////////////////////////////////////////////////////////////////////////////\n";
//            s += "// Arduino: //\n";
//            s += "////////////////////////////////////////////////////////////////////////////////\n";

//            // read in COM PORT
//            yield return null;
//            string comPort = "COM3";

//            if (Config.HasKey(CONFIG_KEYS.comport))
//                comPort = Config.Read(CONFIG_KEYS.comport);

//            s += "\nConnecting to port: " + comPort + "\n";
            
//            try
//            {
//                stream = new SerialPort(comPort, baudRate);
//                stream.Open();
//                dataStreamThread = new Thread(DataStreamThread);
//                dataStreamThread.Start();
//                threadOpen = true;
//                s += "Connected successfully\n";
//            }
//            catch
//            {
//                s += "Failed to connect to port: " + comPort + "\n";

//                string[] devices = System.IO.Ports.SerialPort.GetPortNames();

//                if (devices.Length == 0)
//                {
//                    s += "\nNo Ports Avilable";
//                }
//                else
//                {
//                    s += "\n// Available Ports: //";
//                    for (int i = 0; i < devices.Length; i++)
//                    {
//                        s += "\n" + devices[i];
//                    }
//                }

//            }
            

//            s += "\n////////////////////////////////////////////////////////////////////////////////\n";
//            Debug.Log(s += "\n");
//        }

//        void DataStreamThread()
//        {
//            while (threadOpen)
//            {
//                if (stream.IsOpen)
//                {
//                    try
//                    {
//                        inData = stream.ReadLine();

//                        string[] s = inData.Split(':');

//                        if (s.Length > 1)
//                        {
//                            switch (s[0])
//                            {
//                                case "e":
//                                    encoderData = int.Parse(s[1]);
//                                    break;
//                                case "b1":
//                                    if (int.Parse(s[1]) != button1State)
//                                        button1State = int.Parse(s[1]);
//                                    break;
//                                default:
//                                    defaultString = s[0];
//                                    break;
//                            }
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        Debug.Log("Could not read stream: " + e.Message);
//                    }
//                }
//            }
//        }

//        void Update()
//        {
//            // get input into main thred
//            if (encoderData != prevEncoderData)
//            {
//                prevEncoderData = encoderData;
//            }

//            if (button1State != prevButton1State)
//            {
//                prevButton1State = button1State;

//                if (button1State == 1)
//                {
//                    InputManager.Instance.ButtonDown(0);
//                }
//                else if (button1State == 0)
//                {
//                    InputManager.Instance.ButtonUp(0);
//                }
//            }

//            if (string.Compare(prevDefaultString, defaultString) != 0)
//            {
//                prevDefaultString = defaultString;
//                Debug.Log("Arduino : " + defaultString);
//            }
//        }

//        void OnApplicationQuit()
//        {
//            threadOpen = false;
//            if (stream != null)
//            {
//                if (stream.IsOpen)
//                {
//                    stream.Close();
//                }
//            }
//        }
//    }
//}
