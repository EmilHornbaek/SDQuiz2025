using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class QuizHandler : MonoBehaviour
{
    private QuizQuestion[] usedQuestions;
    private QuizQuestion currentQuestion;
    [SerializeField]
    private AnimalData animalData;
    [SerializeField]
    private int numberOfQuestions = 10;
    private int currentQuestionIndex = 0;
    private Button[] answerButtons;
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
    }

    private void OnAnswerSelected(int index)
    {
        if (currentQuestion.Answers[index].)
        {
            score++;
        }
        else
        {
            Debug.Log("Wrong!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
