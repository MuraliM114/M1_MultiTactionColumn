using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    /// <summary>
    /// For counting the number of runs completed by an automated test build.
    /// </summary>
    public static int automatedTestRunCount;

    /// <summary>
    /// File name to save data to.
    /// Default: persistentData.txt
    /// </summary>
    public static string fileName = "";

    /// <summary>
    /// File path to save data to.
    /// </summary>
    public static string filePath = "";

    /// <summary>
    /// URL to the google form to update and fill.
    /// URL is from 
    /// </summary>
    public static string googleFormURL = "";

    /// <summary>
    /// Reset any persistent data here.
    /// </summary>
    public static void ResetPersistentData()
    {
        automatedTestRunCount = 0;
    }

    /// <summary>
    /// Saves persistent data as a text file to desired path. 
    /// Default Path: Application.dataPath + fileName
    /// Default File: persistentData.txt
    /// </summary>
    /// <param name="_filePath"></param>
    public static void SaveTextFile(string _filePath = null)
    {
        if (string.IsNullOrEmpty(_filePath))
        {
            string defaultPath = Application.dataPath;

            if (string.IsNullOrEmpty(fileName))
                fileName = "persistentData.txt";

            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(filePath);

            string fullFilePath = defaultPath + "/" + fileName;

            string[] infoToSave = new string[]
            {
                "AutomatedTestRunCount: " + automatedTestRunCount,
            };

            File.WriteAllLines(fullFilePath, infoToSave, Encoding.UTF8);
        }
        else
        {
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);

            if (string.IsNullOrEmpty(fileName))
                fileName = "persistentData.txt";

            string fullFilePAth = _filePath + "/" + fileName;

            string[] infoToSave = new string[]
            {

            };

        }
    }

    /// <summary>
    /// Save form data to a google spreadsheet given the googleFormURL.
    /// </summary>
    public static void SaveInformationToGoogleSheet()
    {
        var form = new WWWForm();
        //form.AddField("", );

        WWW www = new WWW(googleFormURL, form);

        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("New entry create on Google Spreadsheet.");
        }
    }

    //public static void SaveBinaryFile(string _filePath = null)
    //{
        //BinaryWriter
    //}

    //public static void LoadBinaryFile(string _filePath)
    //{
        //BinaryReader
    //}
}
