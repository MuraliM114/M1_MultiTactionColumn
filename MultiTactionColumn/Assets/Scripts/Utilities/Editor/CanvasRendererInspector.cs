using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CanvasRenderer))]
public class CanvasRendererInspector : Editor
{
    public static List<GameObject> enabledList = new List<GameObject>(); //A list of objects that become enabled
    public static List<CanvasGroup> opacityList = new List<CanvasGroup>(); //A list of canvasgroups that have their opacity changed from 0

    static CanvasRendererInspector()
    {
        EditorApplication.playModeStateChanged += PlayModeHandler; //When the editor changes play states
    }

    public override void OnInspectorGUI()
    {
        CanvasRenderer tar = (CanvasRenderer)target; //get reference to this component
        Transform t = tar.transform;

        if (GUILayout.Button("Reveal")) //The reveal button (shows up when you click on canvasrenderer component)
        {
            Reveal(t);
        }

        if (GUILayout.Button("Reset")) //Re-Hides anything that was revealed using the reveal button
        {
            RevertReveal();
        }
    }

    private static void Reveal(Transform originalTransform)
    {
        Transform trans = originalTransform;

        while (trans != null) //becomes null when it has no parent
        {
            if (!trans.gameObject.active)
            {
                enabledList.Add(trans.gameObject);
                trans.gameObject.SetActive(true);
            }
            if (trans.GetComponent<CanvasGroup>() != null)
            {
                if (trans.GetComponent<CanvasGroup>().alpha == 0)
                {
                    opacityList.Add(trans.GetComponent<CanvasGroup>());
                    trans.GetComponent<CanvasGroup>().alpha = 1;
                }
            }
            trans = trans.parent;
        }
    }

    public static void RevertReveal()
    {
        foreach (GameObject go in enabledList)
        {
            go.SetActive(false);
        }
        foreach (CanvasGroup cg in opacityList)
        {
            cg.alpha = 0;
        }
        enabledList.Clear();
        opacityList.Clear();
    }

    private static void PlayModeHandler(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            RevertReveal(); //Reverts the revealed objects before entering play mode
    }

    public class FileModificationReset : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths) //When saving a scene
        {
            CanvasRendererInspector.RevertReveal(); //revert the reveal first
            return paths;
        }
    }
}
