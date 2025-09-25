using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class UnityEditorAnimalsAssistTool
{
    [MenuItem("Tools/Animals/Create Variant")]
    public static void CreateVariant()
    {
        CreateVariant("Animal Variant");
    }

    [MenuItem("Tools/Animals/Create Data")]
    public static void CreateData()
    {
        CreateData("Animal_SO");
    }

    public static object CreateVariant(string variantName)
    {
        string animalBasePrefabPath = "Assets/Core/Prefabs/AnimalBase_Prefab.prefab";

        GameObject animalBasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(animalBasePrefabPath);

        if (animalBasePrefab is null) { 
            Debug.Log($"Base prefab not found at: {animalBasePrefabPath}"); 
            return null; 
        }

        string folder = "Assets/Content/Animals";

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(animalBasePrefab);
        string variantPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{variantName}_Prefab.prefab");
        GameObject variant = PrefabUtility.SaveAsPrefabAsset(instance, variantPath);

        Object.DestroyImmediate(instance);

        Selection.activeObject = variant;
        ProjectWindowUtil.ShowCreatedAsset(variant);

        Debug.Log($"Created variant prefab at: {variantPath}");

        return variant;
    }

    public static object CreateData(string dataName)
    {
        string folder = "Assets/Content/Animals/Resources/ScriptableObjects";
        string variantPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{dataName}.asset");

        AnimalData scriptableObject = ScriptableObject.CreateInstance<AnimalData>();
        scriptableObject.Name = dataName.Remove(dataName.IndexOf("_"), 3);

        AssetDatabase.CreateAsset(scriptableObject, variantPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = scriptableObject;
        ProjectWindowUtil.ShowCreatedAsset(scriptableObject);

        return scriptableObject;
    }


    private static string GetSelectedFolderPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return "Assets";
        return AssetDatabase.IsValidFolder(path) ? path : Path.GetDirectoryName(path).Replace("\\", "/");
    }
}
