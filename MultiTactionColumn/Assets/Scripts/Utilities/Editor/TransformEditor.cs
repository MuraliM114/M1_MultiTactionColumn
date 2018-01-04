using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    private const float FIELD_WIDTH = 240.0f;
    private const bool WIDE_MODE = true;

    private const float MAX_POSITION = 1000000.0f;

    private GUIContent positionGUIContent = new GUIContent(LocalString("Position"), LocalString("The local position of this Game Object relative to the parent."));
    private GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation"), LocalString("The local rotation of this Game Object relative to the parent."));
    private GUIContent scaleGUIContent = new GUIContent(LocalString("Scale"), LocalString("The scale of this Game Object relative to the parent."));

    private static string positionWarningText =
            LocalString(
                "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

    private SerializedProperty positionProperty;
    private SerializedProperty rotationProperty;
    private SerializedProperty scaleProperty;

    //private Vector3 savedPositionValue;
    //private Quaternion savedRotationValue;
    //private Vector3 savedScaleValue;

    private static string LocalString(string text)
    {
        return text; //LocalizationDatabase.GetLocalizedString(text);
    }

    public void OnEnable()
    {
        this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
        this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
        this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
        //savedPositionValue = positionProperty.vector3Value;
        //savedRotationValue = rotationProperty.quaternionValue;
        //savedScaleValue = scaleProperty.vector3Value;
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.wideMode = WIDE_MODE;
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - FIELD_WIDTH;

        this.serializedObject.Update();
        Color background = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Position"))
        {
            serializedObject.FindProperty("m_LocalPosition").vector3Value = Vector3.zero;//savedPositionValue;
        }
        if (GUILayout.Button("Reset Rotation"))
        {
            serializedObject.FindProperty("m_LocalRotation").quaternionValue = Quaternion.identity;//savedRotationValue;
        }
        if (GUILayout.Button("Reset Scale"))
        {
            serializedObject.FindProperty("m_LocalScale").vector3Value = Vector3.one;//savedScaleValue;
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = background;

        EditorGUILayout.PropertyField(positionProperty, positionGUIContent);
        RotationPropertyField(rotationProperty, rotationGUIContent);
        EditorGUILayout.PropertyField(scaleProperty, scaleGUIContent);

        if (!ValidatePosition(((Transform) this.target).position))
        {
            EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
        }

        //GUILayout.BeginHorizontal();
        //GUI.backgroundColor = Color.green;
        //if (GUILayout.Button("Save Initial Values"))
        //{
        //    savedPositionValue = ((Transform) target).position;
        //    savedRotationValue = ((Transform) target).localRotation;
        //    savedScaleValue = ((Transform) target).localScale;
        //}

        //GUILayout.EndHorizontal();
        //GUI.backgroundColor = background;

        this.serializedObject.ApplyModifiedProperties();
  }

    private bool ValidatePosition(Vector3 position)
    {
        if (Mathf.Abs(position.x) > MAX_POSITION) return false;
        if (Mathf.Abs(position.y) > MAX_POSITION) return false;
        if (Mathf.Abs(position.z) > MAX_POSITION) return false;
        return true;
    }

    private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
    {
        Transform transform = (Transform) this.targets[0];
        Quaternion localRotation = transform.localRotation;

        foreach (UnityEngine.Object t in (UnityEngine.Object[])targets)
        {
            if(!SameRotation(localRotation, ((Transform)t).localRotation))
            {
                EditorGUI.showMixedValue = true;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();

        Vector3 eulersAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Rotation changed");
            foreach (UnityEngine.Object obj in this.targets)
            {
                Transform t = (Transform) obj;
                t.eulerAngles = eulersAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }

        EditorGUI.showMixedValue = false;
    }

    private bool SameRotation(Quaternion rotation1, Quaternion rotation2)
    {
        if (rotation1.x != rotation2.x) return false;
        if (rotation1.y != rotation2.y) return false;
        if (rotation1.z != rotation2.z) return false;
        if (rotation1.w != rotation2.w) return false;
        return true;
    }
}
