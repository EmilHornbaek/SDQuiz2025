using Nullzone.Unity.Attributes;
using Nullzone.Unity.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalSelection : MonoBehaviour
{
    [SerializeField, FieldName("Send Camera To:")] private Transform animalButtonDestination;
    [SerializeField] private LerpState lerpSwitch = LerpState.QuizSelect;
    private Dictionary<Label, AnimalData> pointLabelLink = new Dictionary<Label, AnimalData>();
    private AudioSource audioSource;
    [SerializeField] private float speedMultiplier = 1;

    private VisualElement mainElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
            Label label = aspectRatioElement?.Q<Label>(name: "PointLabel");

            if (button is null || aspectRatioElement is null || mainElement is null || label is null) return;

            if (animal.Sprite is not null) button.style.backgroundImage = new StyleBackground(animal.Sprite);
            button.text = animal.Name;
            button.clicked += () => GoToQuiz(animal);
            label.text = $"{PlayerStats.Instance.Overview[animal].Points} / {PlayerStats.Instance.Overview[animal].MaxPoints}";

            pointLabelLink.Add(label, animal);

            mainElement.Add(aspectRatioElement);
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<Label, AnimalData> item in pointLabelLink)
        {
            item.Key.text = $"{PlayerStats.Instance.Overview[item.Value].Points} / {PlayerStats.Instance.Overview[item.Value].MaxPoints}";
        }
    }

    private void GoToQuiz(AnimalData animalData)
    {
        if (audioSource is not null && animalData.Sound is not null)
        {
            audioSource.PlayOneShot(animalData.Sound);
        }

        LerpHandler lh = LerpHandler.Instance;
        lh.MoveObjects(lerpSwitch, false, animalButtonDestination, speedMultiplier);
        QuizHandler quizHandler = animalButtonDestination.gameObject.GetComponent<QuizHandler>();
        quizHandler.SetAnimalData(animalData);
        quizHandler.ResetQuiz();
    }

}
