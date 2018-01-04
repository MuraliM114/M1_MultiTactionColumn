using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItemGenerator
{
    [MenuItem("M1/Generate Scenes Menu")]
    static void GenerateScenesMenu()
    {
        //File path to store generated script
        string scriptFile = Application.dataPath + "/Scripts/Utilities/Editor/GeneratedScenesMenu.cs";

        //List of objects (in this case, scenes) to be found
        List<string> scenes = new List<string>();
        scenes = AssetDatabase.FindAssets("t:" + typeof(Scene).Name).ToList();

        //Find the asset path for any found scenes (the scenes are returned as GUID numbers convert)
        List<string> assetPath = new List<string>();
        for (int i = 0; i < scenes.Count; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(scenes[i]);
            assetPath.Add(path);
        }

        //Start creating the generated script
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// This is an auto generated class.");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("using UnityEditor.SceneManagement;");
        sb.AppendLine("");
        sb.AppendLine("public class GeneratedSceneMenu \n{");
        sb.AppendLine("\n");

        //Start filling in the generated script with code.
        for (int i = 0; i < scenes.Count; i++)
        {
            string folderName = Path.GetDirectoryName(assetPath[i]).Split(Path.DirectorySeparatorChar).LastOrDefault();
            string[] folderNames = folderName.Split('/');
            string sceneName = Path.GetFileNameWithoutExtension(assetPath[i]);
            sb.AppendLine("    [MenuItem(\"M1/Scenes/" + folderNames[folderNames.Length - 1] + "/" + sceneName + "\")]");
            sb.AppendLine("    private static void MenuItem" + i.ToString() + "() {");
            sb.AppendLine("        Debug.Log(\"Selected item: " + sceneName + "\");");
            sb.AppendLine("        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();");
            sb.AppendLine("        EditorSceneManager.OpenScene("+ '"' + assetPath[i] + '"' +");");
            sb.AppendLine("     }");
            sb.AppendLine("");
        }

        sb.AppendLine("");
        sb.AppendLine("}");

        //Delete the old script file, write a new script, and import it into the desired location
        System.IO.File.Delete(scriptFile);
        System.IO.File.WriteAllText(scriptFile, sb.ToString(), System.Text.Encoding.UTF8);
        AssetDatabase.ImportAsset("Assets/Scripts/Generated/GeneratedSceneMenu.cs");
    }
}
