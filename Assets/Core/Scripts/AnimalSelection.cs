using Nullzone.Unity.Attributes;
using Nullzone.Unity.UIElements;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalSelection : MonoBehaviour
{
    [SerializeField, FieldName("Send Camera To:")] private Transform animalButtonDestination;
    [SerializeField] private LerpState lerpSwitch = LerpState.QuizSelect;

    private VisualElement mainElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnimalData[] animals = AssetUtility.GetAllAssetsOfType<AnimalData>().ToArray();
        UIDocument ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;
        mainElement = root.Q<VisualElement>("animal-container");
        VisualElement template = mainElement.Q<TemplateContainer>(name:"Template");
        template?.RemoveFromHierarchy();

        mainElement?.AddToClassList("disabled");
        
        foreach ( AnimalData animal in animals)
        {
            PointData point = new PointData();
            point.SetMaxPoints(animal.QuizQuestions.Length);
            PlayerStats.Instance.Overview.Add(animal, point);
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

    private void GoToQuiz(AnimalData animalData)
    {
        LerpHandler lh = LerpHandler.Instance;
        lh.MoveObjects(lerpSwitch, false, animalButtonDestination);
        QuizHandler quizHandler = animalButtonDestination.gameObject.GetComponent<QuizHandler>();
        quizHandler.SetAnimalData(animalData);
        quizHandler.ResetQuiz();
    }

}
