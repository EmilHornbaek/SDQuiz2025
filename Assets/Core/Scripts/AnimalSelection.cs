using Nullzone.Unity.UIElements;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalSelection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnimalData[] animals = AssetUtility.GetAllAssetsOfType<AnimalData>().ToArray();
        UIDocument ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;
        VisualElement mainElement = root.Q<VisualElement>("animal-container");
        VisualElement template = mainElement.Q<TemplateContainer>(name:"Template");
        template?.RemoveFromHierarchy();
        
        foreach ( AnimalData animal in animals)
        {
            VisualElement instance = template?.visualTreeAssetSource.Instantiate();
            AspectRatioElement aspectRatioElement = instance?.Q<AspectRatioElement>(name:"AspectRatioElement");
            Button button = aspectRatioElement?.Q<Button>(name: "AnimalButton");

            if (button is null || aspectRatioElement is null || mainElement is null) return;

            if (animal.Sprite is not null) button.style.backgroundImage = new StyleBackground(animal.Sprite);
            button.text = animal.Name;
            button.clicked += () => GoToQuiz(animal);

            mainElement.Add(aspectRatioElement);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Action GoToQuiz(AnimalData animalData)
    {
        // go to selected quiz
        return null;
    }
}
