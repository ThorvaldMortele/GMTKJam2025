using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public WordSlotManager wordSlotManager;
    public WordSlotManager wordSlotManagerCPU;
    public WordInputManager wordInputManager;
    public WordInputManager wordInputManagerCPU;
    public DictionaryLoader DictionaryLoader;
    public CPUPlayer CPU;
    public Transform AbilitiesContainer;
    public AbilityCardUIValueManager AbilityPrefab;
    public List<AbilityCardUIValueManager> CurrentActiveAbilityCards = new List<AbilityCardUIValueManager>();
    public List<string> ActiveTriggerWords = new List<string>();
    public List<string> AllUsedWords = new List<string>();

    public float StartDelay = 3f;

    [Header("Player Results")]
    public float PlayerScore = 0;
    public int PlayerCompletedLoops = 0;
    public int PlayerCompletedChains = 0;
    public int PlayerWordsCount = 0;
    public int PlayerAbilitiesUsed = 0;

    [Header("CPU Results")]
    public float CPUScore = 0;
    public int CPUCompletedLoops = 0;
    public int CPUCompletedChains = 0;
    public int CPUWordsCount = 0;
    public int CPUAbilitiesUsed = 0;

    [Header("Timer")]
    public float startTime = 300f;  // Starting time in seconds
    public float currentTime;
    public TMP_Text tmpTimerText;  // Reference to a TMP Text component (for TextMeshPro)

    [Header("ResultScreen")]
    public ResultUIManager resultManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
    }

    void Start()
    {
        wordSlotManager.OnLoopCompleted = OnPlayerLoopCompleted;
        wordSlotManagerCPU.OnLoopCompleted = OnCPULoopCompleted;
        wordSlotManager.OnChainCompleted = OnPlayerChainCompleted;
        wordSlotManagerCPU.OnChainCompleted = OnCPUChainCompleted;

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

        wordSlotManager.AddWord(GetNextWordForPlayer(), false);
        wordSlotManagerCPU.AddWord(GetNextWordForCPU(), false);

        StartGame();
    }

    private void StartGame()
    {
        ActiveTriggerWords = GenerateNewTriggerWords();
        wordInputManager.ActiveTriggerWords = ActiveTriggerWords;
        wordInputManagerCPU.ActiveTriggerWords = ActiveTriggerWords;

        wordInputManager.CanInput = true;
        wordInputManagerCPU.CanInput = true;
        CPU.StartPlaying();
    }

    private List<string> GenerateNewTriggerWords()
    {
        if (wordInputManager.Abilities == null || wordInputManager.Abilities.Count < 2)
            throw new ArgumentException("List must contain at least 2 items.");

        System.Random rng = new();
        int firstIndex = rng.Next(wordInputManager.Abilities.Count);

        int secondIndex;
        do
        {
            secondIndex = rng.Next(wordInputManager.Abilities.Count);
        } while (secondIndex == firstIndex);

        var triggerWords = new List<string> { wordInputManager.Abilities[firstIndex].Name.ToLower(), wordInputManager.Abilities[secondIndex].Name.ToLower() };

        foreach(string trigger in triggerWords)
        {
            var ability = wordInputManager.Abilities.Where(x => x.Name.ToLower() == trigger.ToLower()).FirstOrDefault();
            var abilityCard = Instantiate(AbilityPrefab, AbilitiesContainer);
            abilityCard.SetCardValue(ability.Name, ability.Description);

            CurrentActiveAbilityCards.Add(abilityCard);
        }

        return triggerWords;
    }

    public void GenerateNewTriggerWord(string usedTriggerWord)
    {
        if (wordInputManager.Abilities == null || wordInputManager.Abilities.Count < 2)
            throw new ArgumentException("List must contain at least 2 items.");

        var usedAbilityCard = CurrentActiveAbilityCards.Where(x => x._abilityNameText.text.ToLower() == usedTriggerWord.ToLower()).FirstOrDefault();
        CurrentActiveAbilityCards.Remove(usedAbilityCard);
        Destroy(usedAbilityCard.gameObject);
        ActiveTriggerWords.Remove(usedTriggerWord.ToLower());

        var unusedAbilities = wordInputManager.Abilities
        .Select(a => a.Name.ToLower())
        .Where(name => !ActiveTriggerWords.Contains(name))
        .ToList();

        if (unusedAbilities.Count == 0)
        {
            Debug.LogWarning("No new abilities available to assign as trigger words.");
            return;
        }

        System.Random rng = new();
        string newTrigger = unusedAbilities[rng.Next(unusedAbilities.Count)];
        ActiveTriggerWords.Add(newTrigger);

        var ability = wordInputManager.Abilities.Where(x => x.Name.ToLower() == newTrigger.ToLower()).FirstOrDefault();
        var abilityCard = Instantiate(AbilityPrefab, AbilitiesContainer);
        abilityCard.SetCardValue(ability.Name, ability.Description);
        CurrentActiveAbilityCards.Add(abilityCard);
    }

    private void OnPlayerLoopCompleted()
    {
        PlayerScore++;
        PlayerCompletedLoops++;
        string next = GetNextWordForPlayer();

        bool istriggerword;
        if (ActiveTriggerWords.Contains(next)) istriggerword = true;
        else istriggerword = false;

        wordSlotManager.AddWord(next, istriggerword);
    }

    private void OnPlayerChainCompleted()
    {
        PlayerScore += 0.5f;
        PlayerCompletedChains++;
        string next = GetNextWordForPlayer();

        bool istriggerword;
        if (ActiveTriggerWords.Contains(next)) istriggerword = true;
        else istriggerword = false;

        wordSlotManager.AddWord(next, istriggerword);
    }

    private void OnCPULoopCompleted()
    {
        CPUScore++;
        CPUCompletedLoops++;
        string next = GetNextWordForCPU();

        bool istriggerword;
        if (ActiveTriggerWords.Contains(next)) istriggerword = true;
        else istriggerword = false;

        wordSlotManagerCPU.AddWord(next, istriggerword);
    }

    private void OnCPUChainCompleted()
    {
        CPUScore += 0.5f;
        CPUCompletedChains++;
        string next = GetNextWordForPlayer();

        bool istriggerword;
        if (ActiveTriggerWords.Contains(next)) istriggerword = true;
        else istriggerword = false;

        wordSlotManagerCPU.AddWord(next, istriggerword);
    }

    private string GetNextWordForPlayer()
    {
        return DictionaryLoader.WordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => UnityEngine.Random.value)
            .FirstOrDefault() ?? "loop";
    }

    private string GetNextWordForCPU()
    {
        return DictionaryLoader.CPUWordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => UnityEngine.Random.value)
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

        if (CPUScore <= PlayerScore)
            playerWon = true;
        else
            playerWon = false;

        resultManager.ShowResultScreen(playerWon);
    }
    #endregion
}