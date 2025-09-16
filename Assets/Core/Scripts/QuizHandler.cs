using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class QuizHandler : MonoBehaviour
{
    private List<QuizQuestion> usedQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    [SerializeField]
    private AnimalData animalData;
    [Range(1, 20)]
    [SerializeField]
    private int numberOfQuestions = 10;
    private int currentQuestionIndex = 0;
    private Button[] answerButtons;
    private int maxAnswers = 2;
    private int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
        UIDocument uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Label>("Question").text = currentQuestion.Question;
        uiDocument.rootVisualElement.Query<Button>("Answer").ToList().CopyTo(answerButtons);
        
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
        UIDocument uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Label>("Question").text = currentQuestion.Question;
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
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
