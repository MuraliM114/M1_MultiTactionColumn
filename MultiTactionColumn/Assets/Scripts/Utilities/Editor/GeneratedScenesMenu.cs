// This is an auto generated class.
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GeneratedSceneMenu 
{


    [MenuItem("M1/Scenes/Scenes/MainScene")]
    private static void MenuItem0() {
        Debug.Log("Selected item: MainScene");
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
     }

    [MenuItem("M1/Scenes/Scenes/StartScene")]
    private static void MenuItem1() {
        Debug.Log("Selected item: StartScene");
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/StartScene.unity");
     }

    [MenuItem("M1/Scenes/Test Scenes/test")]
    private static void MenuItem2() {
        Debug.Log("Selected item: test");
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/Test Scenes/test.unity");
     }


}
