using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public WordSlotManager wordSlotManager;
    public WordSlotManager wordSlotManagerCPU;
    public WordInputManager wordInputManager;
    public WordInputManager wordInputManagerCPU;
    public DictionaryLoader DictionaryLoader;
    public CPUPlayer CPU;

    public float StartDelay = 3f;

    [Header("Player Results")]
    public int PlayerCompletedLoops = 0;
    public int PlayerWordsCount = 0;
    public int PlayerAbilitiesUsed = 0;

    [Header("CPU Results")]
    public int CPUCompletedLoops = 0;
    public int CPUWordsCount = 0;
    public int CPUAbilitiesUsed = 0;

    [Header("Timer")]
    public float startTime = 300f;  // Starting time in seconds
    public float currentTime;
    public TMP_Text tmpTimerText;  // Reference to a TMP Text component (for TextMeshPro)

    [Header("ResultScreen")]
    public ResultUIManager resultManager;

    void Start()
    {
        wordSlotManager.OnLoopCompleted = OnPlayerLoopCompleted;
        wordSlotManagerCPU.OnLoopCompleted = OnCPULoopCompleted;
        StartCoroutine(LoadWords());
        SetUpTimer();
    }
    private void Update()
    {
        UpdateTimer();
    }


    private IEnumerator LoadWords()
    {
        yield return new WaitForSeconds(StartDelay);

        yield return StartCoroutine(DictionaryLoader.LoadWordsCoroutine());

        wordSlotManager.AddWord("apple");
        wordSlotManagerCPU.AddWord("apple");

        StartGame();
    }

    private void StartGame()
    {
        wordInputManager.CanInput = true;
        wordInputManagerCPU.CanInput = true;
        CPU.StartPlaying();
    }

    private void OnPlayerLoopCompleted()
    {
        PlayerCompletedLoops++;
        string next = GetNextWordForPlayer();
        wordSlotManager.AddWord(next);
    }

    private void OnCPULoopCompleted()
    {
        CPUCompletedLoops++;
        string next = GetNextWordForCPU();
        wordSlotManagerCPU.AddWord(next);
    }

    private string GetNextWordForPlayer()
    {
        return DictionaryLoader.WordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => Random.value)
            .FirstOrDefault() ?? "loop";
    }

    private string GetNextWordForCPU()
    {
        return DictionaryLoader.WordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => Random.value)
            .FirstOrDefault() ?? "loop";
    }

    #region Timer
    private void SetUpTimer()
    {
        currentTime = startTime;  // Initialize current time to start time
    }

    private void UpdateTimer()
    {
        // Countdown timer
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;  // Subtract time passed since last frame
            DisplayTime(currentTime);
        }
        else
        {
            // Timer has finished
            currentTime = 0;
            TimerFinished();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Convert the time to minutes and seconds
        int minutes = Mathf.FloorToInt(timeToDisplay / 60); // Get minutes
        int seconds = Mathf.FloorToInt(timeToDisplay % 60); // Get remaining seconds

        // Format the time as MM:SS
        string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);

        // If you're using TextMeshPro
        if (tmpTimerText != null)
        {
            tmpTimerText.text = timeFormatted; // Display formatted time
        }
    }

    void TimerFinished()
    {
        //Check who won
        bool playerWon = false;

        if (CPUCompletedLoops <= PlayerCompletedLoops)
            playerWon = true;
        else
            playerWon = false;

        resultManager.ShowResultScreen(playerWon);
    }
    #endregion
}