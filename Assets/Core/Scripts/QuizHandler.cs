using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class QuizHandler : MonoBehaviour
{
    [SerializeField]
    [Range(1, 120)]
    [Tooltip("Time in seconds for each question")]
    private int setTimer = 60;
    private float timer;
    private List<QuizQuestion> usedQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    [SerializeField]
    private AnimalData animalData;
    [Range(1, 20)]
    [SerializeField]
    [Tooltip("Number of questions in the quiz")]
    private int numberOfQuestions = 10;
    private int currentQuestionIndex = 0;
    private Button[] answerButtons;
    private int maxAnswers = 2;
    private int score = 0;
    private StyleColor originalButtonColor;
    private UIDocument uiDocument;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
        if (uiDocument == null) { uiDocument = GetComponent<UIDocument>(); }
        uiDocument.rootVisualElement.Q<Label>("Question").text = currentQuestion.Question;
        uiDocument.rootVisualElement.Query<Button>("Answer").ToList().CopyTo(answerButtons);
        uiDocument.rootVisualElement.Q<Label>("Score").text = "Score: " + score + "/" + currentQuestionIndex;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.Answers.Length)
            {
                answerButtons[i].text = currentQuestion.Answers[i].Answer;
                int index = i; // Capture the index for the lambda
                answerButtons[i].clicked += () => OnAnswerSelected(index);
            }
            else
            {
                answerButtons[i].style.display = DisplayStyle.None;
            }
        }
        usedQuestions.Add(currentQuestion);
        originalButtonColor = answerButtons[0].style.backgroundColor;
    }

    private void OnAnswerSelected(int index)
    {
        if (currentQuestion.Answers[index].Allowed)
        {
            score++;
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int tempIndex = i; // Capture the index for the lambda
                answerButtons[i].clicked -= () => OnAnswerSelected(tempIndex);
            }
            NextQuestion();
        }
        else
        {
            maxAnswers--;
            answerButtons[index].clicked -= () => OnAnswerSelected(index); // Disable further clicks
            answerButtons[index].style.backgroundColor = Color.red; // Indicate wrong answer
            if (maxAnswers <= 0) 
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    int tempIndex = i; // Capture the index for the lambda
                    answerButtons[i].clicked -= () => OnAnswerSelected(tempIndex);
                }
                NextQuestion(); 
            }
        }
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= numberOfQuestions)
        {
            // Quiz Finished - Show Score and send back to main menu
        }
        QuizQuestion nextQuestion;
        do
        {
            nextQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
        } while (usedQuestions.Contains(nextQuestion));
        currentQuestion = nextQuestion;
        usedQuestions.Add(currentQuestion);
        uiDocument.rootVisualElement.Q<Label>("Question").text = currentQuestion.Question;
        uiDocument.rootVisualElement.Q<Label>("Score").text = "Score: " + score + "/" + currentQuestionIndex;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.Answers.Length)
            {
                answerButtons[i].text = currentQuestion.Answers[i].Answer;
                int index = i; // Capture the index for the lambda
                answerButtons[i].clicked += () => OnAnswerSelected(index);
                answerButtons[i].style.display = DisplayStyle.Flex;
                answerButtons[i].style.backgroundColor = originalButtonColor;
            }
            else
            {
                answerButtons[i].style.display = DisplayStyle.None;
            }
        }
        timer = setTimer;
    }

    private void ResetQuiz()
    {
        score = 0;
        currentQuestionIndex = 0;
        usedQuestions.Clear();
        maxAnswers = 2;
        timer = setTimer;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int tempIndex = i; // Capture the index for the lambda
            answerButtons[i].clicked -= () => OnAnswerSelected(tempIndex);
            answerButtons[i].style.backgroundColor = originalButtonColor;
        }
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (timer > 0)
            {
            timer -= 1 * Time.deltaTime;
            uiDocument.rootVisualElement.Q<ProgressBar>("Timer").value = timer;
            uiDocument.rootVisualElement.Q<Label>("Timer").text = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
        }
        else
        {
            NextQuestion();
        }
    }
}
