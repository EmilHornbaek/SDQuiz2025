using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class QuizHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AnimalData animalData;

    [Header("Gameplay")]
    [SerializeField, Range(1, 20), Tooltip("Number of questions in the quiz")]
    private int numberOfQuestions = 10;
    [SerializeField, Range(1, 120), Tooltip("Time in seconds for each question")]
    private int setTimer = 60;

    [Header("Audio")]
    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    [SerializeField] private AudioClip victorySound;

    private float timer;
    private List<QuizQuestion> usedQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    private int currentQuestionIndex = 0;
    private List<Button> answerButtons = new List<Button>();
    private int maxAnswers = 2;
    private int score = 0;
    private StyleColor originalButtonColor;
    private UIDocument uiDocument;
    private AudioSource audioSource;
    private bool isQuizDone = false;   // starter som DONE → Update return’er
    private bool firstStart = true;

    private VisualElement container;

    void Awake()
    {
        // Hent komponenter TIDLIGT
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();

        if (uiDocument != null)
        {
            var uiRoot = uiDocument.rootVisualElement;
            container = uiRoot.Q<VisualElement>("root");
            if (container != null)
            {
                // Saml answer knapper en gang
                if (answerButtons == null || answerButtons.Count == 0)
                    answerButtons = container.Query<Button>("Answer").ToList();
            }
        }
    }

    void Start()
    {

        // Start rigtigt i stedet for at antage at felter er sat

    }

    public void Update()
    {
        if (isQuizDone || firstStart) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            var pb = container.Q<ProgressBar>("Timer");
            if (pb != null)
            {
                pb.value = timer;
                pb.title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
            }
        }
        else
        {
            if (audioSource && wrongAnswerSound) audioSource.PlayOneShot(wrongAnswerSound);
            NextQuestion();
        }
    }

    public void ResetQuiz()
    {
        if (container == null)
        {
            Debug.LogError("Container (root VisualElement) not found. Ensure UIDocument has a 'root' element.");
            return;
        }

        if (answerButtons == null || answerButtons.Count == 0)
            answerButtons = container.Query<Button>("Answer").ToList();

        isQuizDone = false;
        score = 0;
        currentQuestionIndex = 0;
        usedQuestions.Clear();
        maxAnswers = 2;

        var pb = container.Q<ProgressBar>("Timer");
        if (pb != null)
        {
            if (Mathf.Approximately(pb.highValue, 0) || pb.highValue != setTimer)
                pb.highValue = setTimer;
        }

        timer = setTimer;
        if (pb != null)
        {
            pb.value = timer;
            pb.title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
        }

        if (!firstStart) ResetAnswers(); else firstStart = false;

        // Første spørgsmål
        SetupRandomQuestion();
    }

    private void SetupRandomQuestion()
    {
        // Tjek data
        if (animalData == null || animalData.QuizQuestions == null || animalData.QuizQuestions.Length == 0)
        {
            Debug.LogError("No quiz questions in AnimalData.");
            EndGame();
            return;
        }

        // Slut hvis vi har brugt alle eller nået antal
        if (currentQuestionIndex >= numberOfQuestions || usedQuestions.Count >= animalData.QuizQuestions.Length)
        {
            EndGame();
            return;
        }
        
        QuizQuestion nextQuestion;
        int safety = 200;
        do
        {
            nextQuestion = animalData.QuizQuestions[Random.Range(0, animalData.QuizQuestions.Length)];
        } while (usedQuestions.Contains(nextQuestion) && --safety > 0);

        currentQuestion = nextQuestion;
        usedQuestions.Add(currentQuestion);

        // UI
        container.Q<Label>("Question").text = currentQuestion.Question;
        container.Q<Label>("Score").text = $"Score: {score}/{currentQuestionIndex}";

        // Husk original farve én gang
        if (answerButtons.Count > 0 && (originalButtonColor == default))
            originalButtonColor = answerButtons[0].style.backgroundColor;

        // Sets the answers for this question with 1 correct and 3 wrong
        List<QuizAnswer> answers = new List<QuizAnswer>();
        while (answers.Count <= 4 || answers.Count != 0)
        {
            List<QuizAnswer> temps = new List<QuizAnswer>(answers);
            while (true)
            {
                var ans = currentQuestion.Answers[Random.Range(0, currentQuestion.Answers.Length)];
                if (ans.Allowed)
                {
                    answers.Add(ans);
                    break;
                }
                else if (!temps.Contains(ans))
                {
                    temps.Add(ans);
                }
                if (temps.Count >= currentQuestion.Answers.Length) break;
            }
            if (answers.Count == 1)
            {
                while (true)
                {
                    var ans = currentQuestion.Answers[Random.Range(0, currentQuestion.Answers.Length)];
                    if (!ans.Allowed && !answers.Contains(ans))
                    {
                        answers.Add(ans);
                    }
                    if (answers.Count >= 4) break;
                }
            }
        }

        // Bind answers if we have exactly 4
        if (answers.Count == 4)
        {
            // Shuffle answers
            for (int i = 0; i < answers.Count * 2; i++)
            {
                var temp = answers[i];
                int randomIndex = 99;
                while (randomIndex == i)
                {
                    randomIndex = Random.Range(0, answers.Count);
                }
                answers[i] = answers[randomIndex];
                answers[randomIndex] = temp;
            }
            // Binds answers to buttons
            for (int i = 0; i < answerButtons.Count; i++)
            {
                var btn = answerButtons[i];
                if (i < answers.Count)
                {
                    btn.text = answers[i].Answer;
                    int idx = i;
                    // Først fjern alle tidligere handlers (se note nedenfor)
                    btn.clicked -= _buttonHandlers[idx];
                    _buttonHandlers[idx] = () => OnAnswerSelected(idx);
                    btn.clicked += _buttonHandlers[idx];

                    btn.style.display = DisplayStyle.Flex;
                    btn.style.backgroundColor = originalButtonColor;
                }
                else
                {
                    btn.style.display = DisplayStyle.None;
                }
            }

            timer = setTimer;
            var pb = container.Q<ProgressBar>("Timer");
            if (pb != null)
            {
                pb.value = timer;
                pb.title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
            }
        }
        // Hvis ikke, prøv igen
        else
        {
            SetupRandomQuestion(); // Prøv igen
        }
        
    }

    // Gemmer stabile delegates til korrekt afmelding (lambda ≠ lambda)
    private readonly System.Action[] _buttonHandlers = new System.Action[8]; // antag max 8 svar-knapper; udvid hvis nødvendigt

    private void OnAnswerSelected(int index)
    {
        if (currentQuestion.Answers[index].Allowed)
        {
            score++;
            if (audioSource && correctAnswerSound) audioSource.PlayOneShot(correctAnswerSound);
            UnsubscribeAllButtons();
            NextQuestion();
        }
        else
        {
            maxAnswers--;
            // Fjern kun denne knap midlertidigt
            var btn = answerButtons[index];
            if (_buttonHandlers[index] != null)
                btn.clicked -= _buttonHandlers[index];

            btn.style.backgroundColor = Color.red;
            if (audioSource && wrongAnswerSound) audioSource.PlayOneShot(wrongAnswerSound);

            if (maxAnswers <= 0)
            {
                UnsubscribeAllButtons();
                NextQuestion();
            }
        }
    }

    private void UnsubscribeAllButtons()
    {
        for (int i = 0; i < answerButtons.Count && i < _buttonHandlers.Length; i++)
        {
            if (_buttonHandlers[i] != null)
                answerButtons[i].clicked -= _buttonHandlers[i];
        }
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= numberOfQuestions)
        {
            if (audioSource && victorySound) audioSource.PlayOneShot(victorySound);
            EndGame();
            return;
        }

        if (usedQuestions.Count >= animalData.QuizQuestions.Length)
        {
            EndGame();
            return;
        }

        SetupRandomQuestion();
    }

    private void EndGame()
    {
        isQuizDone = true;
        // TODO: Vis endeskærm
    }

    public void SetAnimalData(AnimalData data) => animalData = data;

    private void ResetAnswers()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (_buttonHandlers[i] != null)
                answerButtons[i].clicked -= _buttonHandlers[i];

            answerButtons[i].style.backgroundColor = originalButtonColor;
        }
    }
}
