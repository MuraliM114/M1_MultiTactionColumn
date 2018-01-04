using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildPropertyWindow : EditorWindow
{
    private EditorWindow _instance;

    private string buildName;
    private bool copyFiles;
    private string changeLogText;
    private Vector2 scrollPosition = Vector2.zero;

    private List<BuildTargetGroup> buildTargetGroups = new List<BuildTargetGroup>()
    {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android,
        BuildTargetGroup.iOS
    };

    private int selectedBuildTarget = 0;
    private int previousBuildTarget = 0;
    private BuildTargetGroup _buildTargetGroup;
    private BuildTarget _buildTarget;

    public static void ShowWindow()
    {
        EditorWindow.GetWindow<BuildPropertyWindow>("Builder").Show();
    }

    void OnEnable()
    {
        buildName = PlayerSettings.productName;
        copyFiles = true;
        buildName += "_" + DateTime.Now.ToString("MM-dd-yy_HHmm");
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        buildName = EditorGUILayout.TextField("Build Name:", buildName);
        copyFiles = EditorGUILayout.Toggle("Include Config Files: ", copyFiles);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Change Log:");
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(250.0f), GUILayout.Height(250.0f));
        changeLogText = EditorGUILayout.TextArea(changeLogText, GUILayout.Width(250.0f), GUILayout.Height(250.0f));
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUI.LabelField(new Rect(275.0f, 40.0f, 100.0f, 25.0f), "Build Platform:");

        GUILayout.BeginArea(new Rect(275.0f, 65.0f, 250.0f, 250.0f));
        //Windows x64
        if(GUILayout.Button(buildTargetGroups[0].ToString() + " (x64)"))
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroups[0], BuildTarget.StandaloneWindows64);
        }
        //Windows x32
        if (GUILayout.Button(buildTargetGroups[0].ToString() + " (x32)"))
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroups[0], BuildTarget.StandaloneWindows);
        }
        //Android
        if (GUILayout.Button(buildTargetGroups[1].ToString()))
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroups[1], BuildTarget.Android);
        }
        //iOS
        if (GUILayout.Button(buildTargetGroups[2].ToString()))
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroups[2], BuildTarget.iOS);
        }
        GUILayout.EndArea();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorUtility.SetDirty(this);

        Repaint();

        if (GUILayout.Button("Build", GUILayout.Width(100.0f), GUILayout.Height(25.0f)))
        {
            AutoBuild.BuildProjectCustomSettings(BuildOptions.ShowBuiltPlayer, buildName, copyFiles, changeLogText);
        }

        if(GUILayout.Button("Build and Run", GUILayout.Width(100.0f), GUILayout.Height(25.0f)))
        {
            AutoBuild.BuildProjectCustomSettings(BuildOptions.AutoRunPlayer, buildName, copyFiles, changeLogText);
        }
    }
}
