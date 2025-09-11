using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalTool : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tools/Animals/Open Animal Tool _%&A")]
    public static void ShowExample()
    {
        AnimalTool wnd = GetWindow<AnimalTool>();
        wnd.titleContent = new GUIContent("AnimalTool");
        wnd.minSize = new Vector2(100,50);
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        List<Button> buttons = root.Query<Button>().ToList();

        buttons.Find((button => button.name == "CreateAnimalFiles")).clicked -= CreateAnimalFiles;
        buttons.Find((button => button.name == "CreateAnimalFiles")).clicked += CreateAnimalFiles;
    }

    public void CreateAnimalFiles()
    {
        VisualElement root = rootVisualElement;
        TextField animalName = root.Query<TextField>().ToList().Find(field=>field.label == "Animal Name");

        AnimalData data = UnityEditorAnimalsAssistTool.CreateData($"{animalName.value}_SO") as AnimalData;
        GameObject variant = UnityEditorAnimalsAssistTool.CreateVariant($"{animalName.value}") as GameObject;
        variant.GetComponent<Animal>().AnimalData = data;
        PrefabUtility.SavePrefabAsset(variant);
    }
}
