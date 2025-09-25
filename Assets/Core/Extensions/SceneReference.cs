using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class SceneReference
{
    [SerializeField] private string scenePath;
    public string SceneName => System.IO.Path.GetFileNameWithoutExtension(scenePath);
}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(SceneReference))]
public class Drawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty pathProp = property.FindPropertyRelative("scenePath");
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathProp.stringValue);

        EditorGUI.BeginChangeCheck();
        var newScene = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false) as SceneAsset;
        if (EditorGUI.EndChangeCheck() && newScene != null)
        {
            pathProp.stringValue = AssetDatabase.GetAssetPath(newScene);
        }
    }
}
#endif
