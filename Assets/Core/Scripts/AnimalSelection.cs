using Nullzone.Unity.Attributes;
using Nullzone.Unity.UIElements;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimalSelection : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField, Tooltip("Speeds up or slows down all LerpMotion happening upon selecting a quiz.")] private float speedMultiplier = 1;
    [SerializeField, FieldName("Send Camera To:"), Tooltip("Do not touch. The transform which the camera is sent to upon selecting a quiz.")] private Transform animalButtonDestination;
    private LerpState lerpSwitch = LerpState.QuizSelect;
    private Dictionary<Label, AnimalData> pointLabelLink = new Dictionary<Label, AnimalData>();
    private AudioSource audioSource;

    private VisualElement mainElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        AnimalData[] animals = AssetUtility.GetAllAssetsOfType<AnimalData>().ToArray();
        UIDocument ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;
        mainElement = root.Q<VisualElement>("animal-container");
        VisualElement template = mainElement.Q<TemplateContainer>(name: "Template");
        template?.RemoveFromHierarchy();

        mainElement?.AddToClassList("disabled");

        foreach (AnimalData animal in animals)
        {
            PointData point = new PointData();
            point.SetMaxPoints(animal.QuizQuestions.Length);
            PlayerStats.Instance.Overview.Add(animal, point);
            VisualElement instance = template?.visualTreeAssetSource.Instantiate();
            AspectRatioElement aspectRatioElement = instance?.Q<AspectRatioElement>(name: "AspectRatioElement");
            Button button = aspectRatioElement?.Q<Button>(name: "AnimalButton");
            Label label = aspectRatioElement?.Q<Label>(name: "PointLabel");

            if (button is null || aspectRatioElement is null || mainElement is null || label is null) return;

            if (animal.Sprite == null)
            {
                button.text = animal.Name;
            }
            else
            {
                button.style.backgroundImage = new StyleBackground(animal.Sprite);
                button.text = "";
            }

            button.clicked += () => GoToQuiz(animal);
            label.text = $"{PlayerStats.Instance.Overview[animal].Points} / {PlayerStats.Instance.Overview[animal].MaxPoints}";

            pointLabelLink.Add(label, animal);

            mainElement.Add(aspectRatioElement);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (animalButtonDestination.gameObject.GetComponent<QuizHandler>().IsQuizDone && mainMenuMusic != null && !audioSource.isPlaying)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true;
            audioSource.Play();
        } 
        QuizHandler quizHandler = animalButtonDestination.gameObject.GetComponent<QuizHandler>();
        foreach (KeyValuePair<Label, AnimalData> item in pointLabelLink)
        {
            item.Key.text = $"{PlayerStats.Instance.Overview[item.Value].Points} / {PlayerStats.Instance.Overview[item.Value].MaxPoints}";
        }
    }

    private void GoToQuiz(AnimalData animalData)
    {
        if (mainMenuMusic != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (audioSource != null && animalData.Sound != null)
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
