using Nullzone.Unity.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class QuizBackButton : MonoBehaviour
{
    [SerializeField, FieldName("Send Camera To:")] private Transform quizBackDestination;
    [SerializeField] private float speedMultiplier = 1;

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
                LerpHandler lh = LerpHandler.Instance;
                lh.MoveObjects(LerpState.QuizSelect, true, quizBackDestination, speedMultiplier);
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
