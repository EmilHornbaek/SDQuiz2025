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
    private List<Button> answerButtons = new List<Button>();
    private int maxAnswers = 2;
    private int score = 0;
    private StyleColor originalButtonColor;
    private UIDocument uiDocument;
    [SerializeField]
    private AudioClip correctAnswerSound;
    [SerializeField]
    private AudioClip wrongAnswerSound;
    [SerializeField]
    private AudioClip victorySound;
    private AudioSource audioSource;
    private bool isQuizDone = true;
    private bool firstStart = true;
    VisualElement container;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
        container.Q<Label>("Question").text = currentQuestion.Question;
        container.Q<Label>("Score").text = "Score: " + score + "/" + currentQuestionIndex;
        for (int i = 0; i < answerButtons.Count; i++)
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
        if (originalButtonColor == null || originalButtonColor == default) originalButtonColor = answerButtons[0].style.backgroundColor;
        if (container.Q<ProgressBar>("Timer").highValue != setTimer)
        {
            container.Q<ProgressBar>("Timer").highValue = setTimer;
        }
        container.Q<ProgressBar>("Timer").value = timer;
        container.Q<ProgressBar>("Timer").title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
    }

    private void OnAnswerSelected(int index)
    {
        if (currentQuestion.Answers[index].Allowed)
        {
            score++;
            audioSource.PlayOneShot(correctAnswerSound);
            for (int i = 0; i < answerButtons.Count; i++)
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
            audioSource.PlayOneShot(wrongAnswerSound);
            if (maxAnswers <= 0)
            {
                for (int i = 0; i < answerButtons.Count; i++)
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
            audioSource.PlayOneShot(victorySound);
            EndGame();
        }
        if (animalData.QuizQuestions.Length >= usedQuestions.Count)
        {
            EndGame();
        }
        else
        {
            QuizQuestion nextQuestion;
            do
            {
                nextQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
            } while (usedQuestions.Contains(nextQuestion));
            currentQuestion = nextQuestion;
            usedQuestions.Add(currentQuestion);
            container.Q<Label>("Question").text = currentQuestion.Question;
            container.Q<Label>("Score").text = "Score: " + score + "/" + currentQuestionIndex;
            for (int i = 0; i < answerButtons.Count; i++)
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
            container.Q<ProgressBar>("Timer").value = timer;
            container.Q<Label>("Timer").text = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
        }

    }
    /// <summary>
    /// Resets the quiz to its initial state, clearing progress and preparing for a new session.
    /// </summary>
    /// <remarks>This method resets the score, question index, and any used questions. It also restores the 
    /// default state of answer buttons, including their event handlers and styles, and restarts the quiz timer. Call
    /// this method to restart the quiz from the beginning.</remarks>
    public void ResetQuiz()
    {
        if (audioSource == null) { audioSource = gameObject.GetComponent<AudioSource>(); }
        if (uiDocument == null) uiDocument = gameObject.GetComponent<UIDocument>();
        var uiRoot = uiDocument.rootVisualElement;
        container = uiRoot.Q<VisualElement>("root");
        if (answerButtons == null || answerButtons.Count == 0) {answerButtons = container.Query<Button>("Answer").ToList(); }
        isQuizDone = false;
        score = 0;
        currentQuestionIndex = 0;
        usedQuestions.Clear();
        maxAnswers = 2;
        timer = setTimer;
        container.Q<ProgressBar>("Timer").value = timer;
        container.Q<ProgressBar>("Timer").title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
        if (!firstStart) { ResetAnswers(); }
        else { firstStart = false; }
        Start();

    }

    // Update is called once per frame
    void Update()
    {
        if (isQuizDone) return;
        else
        {
            if (timer > 0)
            {
                timer -= 0.5f;
                container.Q<ProgressBar>("Timer").value = timer;
                container.Q<ProgressBar>("Timer").title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
            }
            else
            {
                audioSource.PlayOneShot(wrongAnswerSound);
                NextQuestion();
            }
        }
    }
    private void EndGame()
    {
        isQuizDone = true;
        // Show final score and pick between return to menu or restart
    }
    public void SetAnimalData(AnimalData data)
    {
        animalData = data;
    }

    private void ResetAnswers()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int tempIndex = i; // Capture the index for the lambda
            answerButtons[i].clicked -= () => OnAnswerSelected(tempIndex);
            answerButtons[i].style.backgroundColor = originalButtonColor;
        }
    }
}
