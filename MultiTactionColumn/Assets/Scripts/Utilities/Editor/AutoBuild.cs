using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using System.Text;
using UniRx;

public class AutoBuild
{
    const string configAssetPath = "/Config";
    const string configBuildPath = "/../Config/";
    const string clientAssetsAssetPath = "/ClientAssets";
    const string clientAssetsBuildPath = "/../ClientAssets/";
    const string documentationAssetPath = "/Documentation";
    const string documentationBuildPath = "/../Documentation/";
    const string batchAssetPath = "/Batch/StartUp";
    const string batchBuildPath = "/../";
    const string changeLogBuildPath = "";

    const bool copyConfig = true;
    const bool copyClientAssets = true;
    const bool copyDocumentation = true;
    const bool copyBatch = true;

    const string batchKeyString = "Build.exe";

    private static string path;

    private static string companyName;
    public static string CompanyName
    {
        set { companyName = value; PlayerSettings.companyName = value; }
        get { return companyName; }
    }

    [MenuItem("M1/Build")]
    public static void BuildWithCustomSettings()
    {
        BuildPropertyWindow.ShowWindow();
    }

    public static void BuildProjectCustomSettings(BuildOptions _buildOption, string _buildName,
        bool _copyAssets, string _changeLog = null, BuildTargetGroup _buildTargetGroup = BuildTargetGroup.Unknown, BuildTarget _buildTarget = BuildTarget.NoTarget)
    {
        string[] activeScenes = GetActiveScenes();

        // save location
        CreatFolder(Application.dataPath + "/../Builds/" + _buildName);

        // get path from save panel
        path = EditorUtility.SaveFilePanel("Build", "Builds/" + _buildName, _buildName, "exe");

        if (string.Compare(path, "") != 0)
        {
            // hold file extensions
            string tail = "";
            // add .exe if needed
            tail = path.Substring(Math.Max(0, path.Length - 4));
            if (string.Compare(tail, ".exe") != 0)
            {
                path += ".exe";
            }

            UpdateVersionNumber();

            if (_copyAssets)
            {
                if (copyConfig)
                {
                    CopyAssets(configAssetPath, configBuildPath);
                }

                if (copyClientAssets)
                {
                    CopyAssets(clientAssetsAssetPath, clientAssetsBuildPath);
                }

                if (copyDocumentation)
                {
                    CopyAssets(documentationAssetPath, documentationBuildPath);
                }

                if (copyBatch)
                {
                    CopyBatch(batchAssetPath, batchBuildPath + _buildName, _buildName);
                }
            }

            if (_changeLog != null)
            {
                CreateChangeLogFile(_changeLog, changeLogBuildPath + _buildName);
            }

            if (_buildTargetGroup == BuildTargetGroup.Unknown && _buildTarget == BuildTarget.NoTarget)
            {
                // build
                BuildPipeline.BuildPlayer(activeScenes, path, EditorUserBuildSettings.activeBuildTarget,
                    BuildOptions.ShowBuiltPlayer | _buildOption);
            }
            else
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTargetGroup, _buildTarget);
                BuildPipeline.BuildPlayer(activeScenes, path, EditorUserBuildSettings.activeBuildTarget,
                    (BuildOptions.ShowBuiltPlayer | _buildOption));
            }
        }
        else
        {
            Debug.Log("Failed to Build: Missing build name or file path");
        }
    }

    private static void UpdateVersionNumber()
    {
        if (!File.Exists(Application.dataPath + "/Config/VersionNumber.txt"))
            File.Create(Application.dataPath + "/Config/VersionNumber.txt");

        StreamReader inStream = new StreamReader(Application.dataPath + "/Config/VersionNumber.txt");

        int majorBuildNumber = 0;
        int minorBuildNumber = 1;

        string line = inStream.ReadLine();
        Debug.Log(line);
        if (line != null)
        {
            string[] words = line.Split('.', ' ');

            majorBuildNumber = int.Parse(words[0]);
            minorBuildNumber = int.Parse(words[1]) + 1;
        }
        inStream.Close();

        line = majorBuildNumber + "." + minorBuildNumber.ToString("000") + " - " + DateTime.Now.ToString();
        Debug.Log(line);
        StreamWriter outStream = new StreamWriter(Application.dataPath + "/Config/VersionNumber.txt");

        outStream.WriteLine(line);

        outStream.Close();
    }

    private static void CreatFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        else
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }

    private static string[] GetActiveScenes()
    {
        List<string> sceneList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneList.Add(scene.path);
            }
        }
        return sceneList.ToArray();
    }

    private static void CopyAssets(string _from, string _to)
    {
        // create ClientAssets folder
        string assetsFolder = path + _to;
        CreatFolder(assetsFolder);

        // move files to client assets from project if we need to
        string[] files = System.IO.Directory.GetFiles(Application.dataPath + _from);
        string tail = "";
        foreach (string s in files)
        {
            tail = s.Substring(Math.Max(0, s.Length - 5));
            if (string.Compare(tail, ".meta") != 0)
                File.Copy(s, assetsFolder + Path.GetFileName(s), true);
        }
    }

    private static void CopyBatch(string _from, string _to, string _buildName)
    {
        string[] batchLines = System.IO.File.ReadAllLines(Application.dataPath + _from + ".bat");

        for (int i = 0; i < batchLines.Length; i++)
        {
            batchLines[i] = batchLines[i].Replace(batchKeyString, _buildName + ".exe");
        }

        File.WriteAllLines(path + _to + ".bat", batchLines);
    }

    private static void CreateChangeLogFile(string _changeLog, string savePath)
    {
        string file =  path + "/../changelog.txt";
        File.WriteAllLines(file, _changeLog.Split('\n'), Encoding.UTF8);
    }
}
