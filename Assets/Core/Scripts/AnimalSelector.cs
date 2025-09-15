using Nullzone.Unity.UIElements;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalSelector : MonoBehaviour
{
    [Header("Aspect Ratio")]
    [SerializeField] private float aspectRatioX = 1f;
    [SerializeField] protected float aspectRatioY = 1f;
    [SerializeField, Range(0.01f,100)] private float width;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnimalData[] animals = AssetUtility.GetAllAssetsOfType<AnimalData>().ToArray();
        UIDocument ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;
        VisualElement mainElement = root.Q<VisualElement>("animal-container");
        foreach ( AnimalData animal in animals)
        {
            AspectRatioElement aspectRatioElement = CreateAspectRatioElement(animal);
            mainElement.Add(aspectRatioElement);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private AspectRatioElement CreateAspectRatioElement(AnimalData animal)
    {
        AspectRatioElement aspectRatioElement = new AspectRatioElement();
        aspectRatioElement.SizeType = DataType.Percentage;
        aspectRatioElement.Width = width;
        aspectRatioElement.AspectRatioX = aspectRatioX;
        aspectRatioElement.AspectRatioY = aspectRatioY;
        aspectRatioElement.AddToClassList("animal-option");
        aspectRatioElement.Add(CreateAnimalButton(animal));
        return aspectRatioElement;
    }

    private VisualElement CreateAnimalButton(AnimalData animal)
    {
        Button button = new Button();
        if (animal.Sprite is not null) button.style.backgroundImage = new StyleBackground(animal.Sprite);
        else button.text = animal.Name;
        button.style.width = Length.Percent(100);
        button.style.height = Length.Percent(100);
        button.clicked += GoToQuiz(animal);
        return button;
    }

    private Action GoToQuiz(AnimalData animalData)
    {
        // go to selected quiz
        return null;
    }
}
