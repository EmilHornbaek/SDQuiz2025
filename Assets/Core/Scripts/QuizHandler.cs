using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using Nullzone.Unity.Attributes;
/// <summary>
/// Manages the flow and logic of a quiz game, including question selection, user interaction, scoring, and game state
/// transitions.
/// </summary>
/// <remarks>This class is responsible for handling all aspects of the quiz gameplay, such as initializing the
/// quiz, managing the timer, processing user answers, and determining when the game ends. It interacts with the Unity
/// UI system to update the user interface dynamically and plays audio clips for feedback during the game. The quiz is
/// based on a set of questions and answers provided by an <see cref="AnimalData"/> object.  The class assumes that the
/// Unity scene contains the necessary UI elements, such as buttons for answers, labels for displaying the question and
/// score, and a progress bar for the timer. It also requires audio clips for correct and incorrect answers, as well as
/// background music for the quiz.  This class is designed to be attached to a Unity GameObject and relies on Unity's
/// MonoBehaviour lifecycle methods, such as <see cref="Awake"/> and <see cref="Update"/>.</remarks>
public class QuizHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AnimalData animalData;

    [Header("Gameplay")]
    [SerializeField, Range(1, 20), Tooltip("Number of questions in the quiz")]
    private int numberOfQuestions = 10;
    [SerializeField, Range(1, 120), Tooltip("Time in seconds for each question")]
    private int setTimer = 60;
    [SerializeField, Tooltip("How many attempts the player has"), Range(1, 3)]
    private int maxAttempts = 2;

    [Header("Audio")]
    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip quizMusic;

    [Header("Return Settings")]
    [SerializeField, Tooltip("Speeds up or slows down all LerpMotion happening after the quiz ends.")] private float speedMultiplier = 1;
    [SerializeField, FieldName("End Screen Duration"), Tooltip("The amount of time the game waits before returning to the Quiz Selection after completing a quiz. Measured in seconds.")] private float returnTimerDuration;
    [SerializeField, FieldName("Send Camera To:"), Tooltip("Do not touch. The transform which the camera is sent back to after the quiz ends.")] private Transform quizBackDestination;
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
    /// <summary>
    /// Initializes components and prepares the UI and other dependencies for use.
    /// </summary>
    /// <remarks>This method is called automatically by Unity when the script instance is being loaded.  It
    /// ensures that required components, such as <see cref="AudioSource"/> and <see cref="UIDocument"/>,  are retrieved
    /// and initialized. Additionally, it sets up the random number generator and collects  references to UI elements,
    /// such as answer buttons, for later use.</remarks>
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
    /// <summary>
    /// Updates the state of the quiz, managing timers, audio playback, and transitions.
    /// </summary>
    /// <remarks>This method should be called regularly, such as in a game loop, to ensure the quiz logic
    /// progresses correctly. It handles countdown timers, updates the progress bar, plays audio cues, and transitions
    /// to the next question or state when appropriate.</remarks>
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
    /// <summary>
    /// Resets the quiz to its initial state, preparing it for a new session.
    /// </summary>
    /// <remarks>This method resets all quiz-related data, including the score, question index, and used
    /// questions.  It also reinitializes the timer, updates the progress bar, and prepares the UI for the first
    /// question.  If quiz music is configured, it will start playing in a loop. Ensure that the container and its child
    /// elements, such as buttons and the progress bar, are properly set up before calling this method.</remarks>
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
    /// <summary>
    /// Attempts to select a valid quiz question from the available pool of unused questions.
    /// </summary>
    /// <remarks>A valid question is defined as one that has at least one correct answer and at least three
    /// incorrect answers. The method ensures that the selected question is chosen randomly from the pool of unused
    /// questions. If no valid question is found, the method returns <see langword="false"/> and <paramref
    /// name="validQuestion"/>  is set to <see langword="null"/>.</remarks>
    /// <param name="validQuestion">When this method returns, contains the selected <see cref="QuizQuestion"/> if a valid question is found; 
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if a valid question is successfully selected; otherwise, <see langword="false"/>.</returns>
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
    /// <summary>
    /// Sets up a random quiz question by selecting a valid, unused question and preparing the UI for user interaction.
    /// </summary>
    /// <remarks>This method ensures that the selected question has at least one correct answer and three
    /// incorrect answers.  It updates the UI elements, including the question text, score display, answer buttons, and
    /// timer.  If no valid questions remain or the game conditions are met, the game ends.</remarks>
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
    /// <summary>
    /// Handles the selection of an answer in the quiz, updating the score and progressing the game state based on the
    /// correctness of the selected answer.
    /// </summary>
    /// <remarks>If the selected answer is correct, the score is incremented, and the game progresses to the
    /// next question.  Additionally, the player's stats are updated if applicable. If the answer is incorrect, the
    /// number of remaining  attempts is decremented, and the selected answer is visually marked as incorrect. If no
    /// attempts remain, the game  progresses to the next question.</remarks>
    /// <param name="index">The index of the selected answer in the list of available answers.</param>
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
    /// <summary>
    /// Unsubscribes all event handlers from the click events of the answer buttons.
    /// </summary>
    /// <remarks>This method iterates through the collection of answer buttons and removes the associated
    /// event handlers  from their click events. Only buttons with non-null handlers are affected.</remarks>
    private void UnsubscribeAllButtons()
    {
        for (int i = 0; i < answerButtons.Count && i < _buttonHandlers.Length; i++)
        {
            if (_buttonHandlers[i] != null)
                answerButtons[i].clicked -= _buttonHandlers[i];
        }
    }
    /// <summary>
    /// Advances to the next question in the quiz. If the quiz is complete, ends the game.
    /// </summary>
    /// <remarks>This method increments the current question index and determines whether the quiz should
    /// continue  or end based on the number of questions and the used questions. If the quiz is complete, it plays  a
    /// victory sound (if configured) and ends the game. Otherwise, it sets up the next question with  a default maximum
    /// number of answers.</remarks>
    private void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= numberOfQuestions)
        {
            EndGame();
            return;
        }

        if (usedQuestions.Count >= animalData.QuizQuestions.Length)
        {
            EndGame();
            return;
        }
        maxAnswers = maxAttempts;
        SetupRandomQuestion();
    }
    /// <summary>
    /// Ends the game and displays the final score and a random fact about the animal.
    /// </summary>
    /// <remarks>This method updates the UI to show the end screen, including the player's final score  and a
    /// randomly selected fact. It also initiates a countdown timer for returning to the main menu.</remarks>
    private void EndGame()
    {
        // TODO: Vis endeskærm

        //uiDocument.enabled = false;
        if (audioSource && victorySound) audioSource.PlayOneShot(victorySound);
        LerpHandler lh = LerpHandler.Instance;
        lh.MoveObjects(LerpState.QuizEnd, false, null, speedMultiplier);
        returnTimer = returnTimerDuration;
        if (animalData.Facts.Length != 0)
        {
            container.Q<Label>("Question").text = animalData.Facts[random.Next(0, animalData.Facts.Length)].fact;
        }
        container.Q<Label>("Score").text = $"Din endelige score er: {score} ud af {currentQuestionIndex}";
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].style.display = DisplayStyle.None;
        }
        countdown = true;
    }
    /// <summary>
    /// Sets the animal data for the current instance.
    /// </summary>
    /// <param name="data">The <see cref="AnimalData"/> object containing the animal's information. Cannot be <see langword="null"/>.</param>
    public void SetAnimalData(AnimalData data) => animalData = data;
    /// <summary>
    /// Resets the state of all answer buttons by removing event handlers and clearing visual indicators.
    /// </summary>
    /// <remarks>This method detaches any previously assigned click event handlers from the answer buttons 
    /// and removes the "wrong-answer" visual class from each button. It ensures that the buttons  are in a clean state
    /// for reuse.</remarks>
    private void ResetAnswers()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (_buttonHandlers[i] != null)
                answerButtons[i].clicked -= _buttonHandlers[i];

            answerButtons[i].EnableInClassList("wrong-answer", false);
        }
    }
    /// <summary>
    /// Randomizes the order of elements in the specified collection.
    /// </summary>
    /// <remarks>The method uses a pseudo-random number generator to shuffle the elements. The degree of
    /// shuffling  is influenced by the current system time and other factors, which may result in varying levels of
    /// randomness.</remarks>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to shuffle. The collection is modified in place.</param>
    /// <returns>The shuffled collection.</returns>
    private List<T> Shuffle<T>(List<T> collection)
    {
        int shuffleDepth = DateTime.Now.Millisecond - (int)(Time.deltaTime * 10f);
        if (shuffleDepth < 0) shuffleDepth *= -1;
        for (int i = collection.Count - 1; i > 0 && shuffleDepth > 0; i--, shuffleDepth--)
        {
            int j = random.Next(0, i + 1);
            (collection[i], collection[j]) = (collection[j], collection[i]);
        }

        return collection;
    }
}
