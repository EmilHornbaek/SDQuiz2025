using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using Nullzone.Unity.Attributes;

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
    [SerializeField] private AudioClip quizMusic;

    [Header("Return Settings")]
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField, FieldName("End Screen Duration")] private float returnTimerDuration;
    [SerializeField, FieldName("Send Camera To:")] private Transform quizBackDestination;
    private float returnTimer;
    private bool countdown = false;

    private float timer;
    private static System.Random random;
    private List<QuizQuestion> usedQuestions = new List<QuizQuestion>();
    private QuizQuestion currentQuestion;
    private List<QuizAnswer> currentAnswers = new List<QuizAnswer>();
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

        if (random == null) random = new System.Random(DateTime.Now.Millisecond);

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
    }

    public void Update()
    {
        if (isQuizDone || firstStart) return;

        if (returnTimer <= 0 && countdown)
        {
            isQuizDone = true;
            countdown = false;
            if (quizMusic is not null && audioSource)
            {
                audioSource.Stop();
            }
            LerpHandler lh = LerpHandler.Instance;
            lh.MoveObjects(LerpState.QuizSelect, true, quizBackDestination, speedMultiplier);
        }
        else if (countdown)
        {
            returnTimer -= Time.deltaTime;
        }

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
        //uiDocument.enabled = true;

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

        if (quizMusic is not null && audioSource is not null)
        {
            audioSource.clip = quizMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Første spørgsmål
        SetupRandomQuestion();
    }

    private bool TryPickValidQuestion(out QuizQuestion validQuestion)
    {
        validQuestion = null;

        // Kandidater: alle ubrugt
        var pool = new List<QuizQuestion>();
        foreach (var q in animalData.QuizQuestions)
            if (!usedQuestions.Contains(q)) pool.Add(q);

        if (pool.Count == 0) return false;

        // Shuffle pool for tilfældig rækkefølge (Fisher–Yates)
        pool = Shuffle<QuizQuestion>(pool);

        // Gå igennem indtil vi finder én med nok svar
        foreach (var q in pool)
        {
            int correct = 0, wrong = 0;
            foreach (var a in q.Answers)
            {
                if (a.Allowed) correct++; else wrong++;
                if (correct >= 1 && wrong >= 3) break;
            }
            if (correct >= 1 && wrong >= 3)
            {
                validQuestion = q;
                return true;
            }
        }

        return false;
    }

    private void SetupRandomQuestion()
    {
        // 1) Grundtjek
        if (animalData == null || animalData.QuizQuestions == null || animalData.QuizQuestions.Length == 0)
        {
            Debug.LogError("No quiz questions in AnimalData.");
            EndGame();
            return;
        }
        if (currentQuestionIndex >= numberOfQuestions || usedQuestions.Count >= animalData.QuizQuestions.Length)
        {
            EndGame();
            return;
        }

        // 2) Find et gyldigt, ubrugt spørgsmål (med nok korrekte/for kerte)
        if (!TryPickValidQuestion(out var nextQuestion))
        {
            Debug.LogWarning("No remaining questions with at least 1 correct and 3 wrong answers.");
            EndGame();
            return;
        }

        currentQuestion = nextQuestion;
        usedQuestions.Add(currentQuestion);

        // 3) UI-tekster
        container.Q<Label>("Question").text = currentQuestion.Question;
        container.Q<Label>("Score").text = $"Score: {score}/{currentQuestionIndex}";

        if (answerButtons.Count > 0 && (originalButtonColor == default))
            originalButtonColor = answerButtons[0].style.backgroundColor;

        // 4) Byg præcis 4 svar (1 korrekt + 3 forkerte)
        var corrects = new List<QuizAnswer>();
        var wrongs = new List<QuizAnswer>();
        foreach (var a in currentQuestion.Answers)
            if (a.Allowed) corrects.Add(a); else wrongs.Add(a);

        // (burde være opfyldt pga. TryPickValidQuestion, men vi tjekker alligevel)
        if (corrects.Count == 0 || wrongs.Count < 3)
        {
            Debug.LogWarning($"Selected question is no longer valid. Skipping.");
            SetupRandomQuestion(); // sjælden fallback, men sikkert (meget lille dybde)
            return;
        }

        // 5) Vælg tilfældigt 1 korrekt og 3 forkerte
        List<QuizAnswer> answers = new List<QuizAnswer>();
        answers.Add(corrects[random.Next(0, corrects.Count)]);

        wrongs = Shuffle<QuizAnswer>(wrongs);
        answers.Add(wrongs[0]);
        answers.Add(wrongs[1]);
        answers.Add(wrongs[2]);

        // Shuffle svarene
        answers = Shuffle<QuizAnswer>(answers);

        currentAnswers = answers;

        ResetAnswers();

        // 6) Bind til knapper
        for (int i = 0; i < answerButtons.Count; i++)
        {
            var btn = answerButtons[i];
            if (i < answers.Count)
            {
                btn.text = answers[i].Answer;
                int idx = i;

                if (_buttonHandlers[idx] != null)
                    btn.clicked -= _buttonHandlers[idx];

                _buttonHandlers[idx] = () => OnAnswerSelected(idx);
                btn.clicked += _buttonHandlers[idx];

                btn.style.display = DisplayStyle.Flex;
                btn.EnableInClassList("wrong-answer", false);
            }
            else
            {
                btn.style.display = DisplayStyle.None;
            }
        }

        // 7) Timer UI
        timer = setTimer;
        var pb = container.Q<ProgressBar>("Timer");
        if (pb != null)
        {
            pb.highValue = setTimer;
            pb.value = timer;
            pb.title = "Tid tilbage: " + Mathf.CeilToInt(timer) + "s";
        }
    }


    // Gemmer stabile delegates til korrekt afmelding (lambda ≠ lambda)
    private readonly System.Action[] _buttonHandlers = new System.Action[8]; // antag max 8 svar-knapper; udvid hvis nødvendigt

    private void OnAnswerSelected(int index)
    {
        if (currentAnswers[index].Allowed)
        {
            score++;
            if (audioSource && correctAnswerSound) audioSource.PlayOneShot(correctAnswerSound);
            if (!animalData.QuizQuestions[currentQuestionIndex].guessedCorrectly)
            {
                animalData.QuizQuestions[currentQuestionIndex].guessedCorrectly = true;
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.Overview[animalData].AddPoint();
                }
            }
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

            btn.ToggleInClassList("wrong-answer");
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
        maxAnswers = 2;
        SetupRandomQuestion();
    }

    private void EndGame()
    {
        // TODO: Vis endeskærm

        //uiDocument.enabled = false;
        LerpHandler lh = LerpHandler.Instance;
        lh.MoveObjects(LerpState.QuizEnd, false, null, speedMultiplier);
        returnTimer = returnTimerDuration;
        countdown = true;
    }

    public void SetAnimalData(AnimalData data) => animalData = data;

    private void ResetAnswers()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (_buttonHandlers[i] != null)
                answerButtons[i].clicked -= _buttonHandlers[i];

            answerButtons[i].EnableInClassList("wrong-answer", false);
        }
    }

    private List<T> Shuffle<T>(List<T> collection)
    {
        int shuffleDepth = DateTime.Now.Millisecond - (int)(Time.deltaTime*10f);
        if (shuffleDepth < 0) shuffleDepth *= -1;
        for (int i = collection.Count - 1; i > 0 && shuffleDepth > 0; i--, shuffleDepth--)
        {
            int j = random.Next(0, i + 1);
            (collection[i], collection[j]) = (collection[j], collection[i]);
        }

        return collection;
    }
}
