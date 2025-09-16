using Nullzone.Unity.Attributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(FieldNameAttribute))]
public class FieldNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FieldNameAttribute attr = (FieldNameAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, new GUIContent(attr.Label), true);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}