using Nullzone.Unity.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class QuizBackButton : MonoBehaviour
{
    [SerializeField, FieldName("Send Camera To:"), Tooltip("Do not touch. The transform which the camera is sent to upon exiting a quiz.")] private Transform quizBackDestination;
    [SerializeField, Tooltip("Speeds up or slows down all LerpMotion happening upon exiting a quiz.")] private float speedMultiplier = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement mainElement = document.rootVisualElement.Q<VisualElement>(name: "root");
        Button backButton = mainElement.Q<Button>(name: "BackButton");
        if (backButton is not null)
        {
            backButton.clicked += () =>
            {
                GetComponent<QuizHandler>().IsQuizDone = true;
                LerpHandler lh = LerpHandler.Instance;
                GetComponent<AudioSource>()?.Stop();
                lh.MoveObjects(LerpState.QuizSelect, true, quizBackDestination, speedMultiplier);
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
