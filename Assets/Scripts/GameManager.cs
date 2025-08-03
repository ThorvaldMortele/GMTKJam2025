using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public WordSlotManager wordSlotManager;
    public WordSlotManager wordSlotManagerCPU;
    public WordInputManager wordInputManager;
    public WordInputManager wordInputManagerCPU;
    public DictionaryLoader DictionaryLoader;
    public CPUPlayer CPU;
    public AbilityCardUIValueManager AbilityPrefab;
    public List<AbilityCardUIValueManager> CurrentActiveAbilityCards = new List<AbilityCardUIValueManager>();
    public List<string> ActiveTriggerWords = new List<string>();
    public List<string> AllUsedWords = new List<string>();
    public bool GameStarted = false;

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

    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    public float punchScale = 1.5f;
    public float punchDuration = 0.3f;

    [Header("Audio")]
    public EventReference loopScoredSFX;
    public EventReference chainScoredSFX;
    public EventReference countdownTickSFX;
    public EventReference countdownStartSFX;

    [Header("Ability Card Slots")]
    public Transform AbilitySlotA;
    public Transform AbilitySlotB;

    private AbilityCardUIValueManager cardInSlotA;
    private AbilityCardUIValueManager cardInSlotB;

    [SerializeField] private AbilitySticker slapStickerPrefab;
    [SerializeField] private Transform playerStickerArea;
    [SerializeField] private Transform cpuStickerArea;

    public GameObject DifficultySelectScreen;
    public List<GameObject> GameplayObjects;

    private bool GameEnded = false;

    public CPUVoiceLineManager voicelinemanager;

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
        DifficultySelectScreen.SetActive(true);
        GameplayObjects.ForEach(x => x.SetActive(false));

        wordSlotManager.OnLoopCompleted = OnPlayerLoopCompleted;
        wordSlotManagerCPU.OnLoopCompleted = OnCPULoopCompleted;
        wordSlotManager.OnChainCompleted = OnPlayerChainCompleted;
        wordSlotManagerCPU.OnChainCompleted = OnCPUChainCompleted;

        SetUpTimer();
    }

    private void Update()
    {
        if (GameStarted)
            UpdateTimer();
    }

    private IEnumerator RandomCallRoutine(float totalDuration, int numberOfCalls)
    {
        var callTimes = GenerateCallTimes(totalDuration, numberOfCalls, 15f);
        float elapsed = 0f;
        int index = 0;

        while (elapsed < totalDuration && index < callTimes.Count)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= callTimes[index])
            {
                StartCoroutine(voicelinemanager.PlayCPUCommentVoiceLine());
                index++;
            }

            yield return null;
        }
    }

    private List<float> GenerateCallTimes(float totalDuration, int numberOfCalls, float minSpacing)
    {
        List<float> validTimes = new();
        int safety = 0;

        while (validTimes.Count < numberOfCalls && safety < 10000)
        {
            float candidate = UnityEngine.Random.Range(0f, totalDuration);
            bool tooClose = validTimes.Any(t => Mathf.Abs(t - candidate) < minSpacing);

            if (!tooClose)
                validTimes.Add(candidate);

            safety++;
        }

        validTimes.Sort();
        return validTimes;
    }


    public void SelectDifficulty(string chosenDifficulty)
    {
        DifficultySelectScreen.SetActive(false);
        GameplayObjects.ForEach(x => x.SetActive(true));

        switch(chosenDifficulty) 
        {
            case "easy":
                {
                    CPU.DelayWhenRethinking = 5f;
                    CPU.MinDelayBetweenWords = 6f;
                    CPU.MaxDelayBetweenWords = 9f;
                    CPU.InitialStartDelay = 3f;
                    break;
                }
            case "medium":
                {
                    CPU.DelayWhenRethinking = 4f;
                    CPU.MinDelayBetweenWords = 4f;
                    CPU.MaxDelayBetweenWords = 8f;
                    CPU.InitialStartDelay = 1f;
                    break;
                }
            case "hard":
                {
                    CPU.DelayWhenRethinking = 2f;
                    CPU.MinDelayBetweenWords = 2f;
                    CPU.MaxDelayBetweenWords = 4f;
                    CPU.InitialStartDelay = 0f;
                    CPU.MaxWordCountToFormLoop = 12;
                    break;
                }
        }

        StartCoroutine(LoadWords());
    }

    private IEnumerator LoadWords()
    {
        yield return StartCoroutine(DictionaryLoader.LoadWordsCoroutine());

        yield return StartCoroutine(CountdownRoutine());

        wordSlotManager.AddWord(GetNextWordForPlayer(), false);
        wordSlotManagerCPU.AddWord(GetNextWordForCPU(), false);

        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(RandomCallRoutine(startTime, 8));
        GameStarted = true;

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

        for (int i = 0; i < triggerWords.Count; i++)
        {
            var abilityName = triggerWords[i];
            var ability = wordInputManager.Abilities.FirstOrDefault(x => x.Name.ToLower() == abilityName);

            Transform targetSlot = i == 0 ? AbilitySlotA : AbilitySlotB;
            var cardGO = Instantiate(AbilityPrefab, targetSlot);
            cardGO.transform.localPosition = Vector3.zero;
            cardGO.SetCardValue(ability.Name, ability.Description);

            if (i == 0) cardInSlotA = cardGO;
            else cardInSlotB = cardGO;

            CurrentActiveAbilityCards.Add(cardGO);
        }

        return triggerWords;
    }

    public void GenerateNewTriggerWord(string usedTriggerWord)
    {
        if (wordInputManager.Abilities == null || wordInputManager.Abilities.Count < 2)
            throw new ArgumentException("List must contain at least 2 items.");

        var usedAbilityCard = CurrentActiveAbilityCards.FirstOrDefault(x => x._abilityNameText.text.ToLower() == usedTriggerWord.ToLower());
        CurrentActiveAbilityCards.Remove(usedAbilityCard);
        ActiveTriggerWords.Remove(usedTriggerWord.ToLower());

        // Determine which slot to refill
        Transform targetSlot = null;
        if (usedAbilityCard == cardInSlotA)
        {
            cardInSlotA = null;
            targetSlot = AbilitySlotA;
        }
        else if (usedAbilityCard == cardInSlotB)
        {
            cardInSlotB = null;
            targetSlot = AbilitySlotB;
        }

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

        usedAbilityCard?.PlayDisappearAnimation(() =>
        {
            Destroy(usedAbilityCard.gameObject);

            var ability = wordInputManager.Abilities.FirstOrDefault(x => x.Name.ToLower() == newTrigger);
            var newCard = Instantiate(AbilityPrefab, targetSlot);
            newCard.transform.localPosition = Vector3.zero;
            newCard.SetCardValue(ability.Name, ability.Description);

            if (targetSlot == AbilitySlotA) cardInSlotA = newCard;
            else if (targetSlot == AbilitySlotB) cardInSlotB = newCard;

            CurrentActiveAbilityCards.Add(newCard);
        });
    }

    private void OnPlayerLoopCompleted()
    {
        PlayerScore++;
        PlayerCompletedLoops++;
        PlayLoopCompletedSFX();
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
        PlayChainCompletedSFX();
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

    public void ShowAbilitySticker(string abilityName, bool onCPU, AbilityCardUIValueManager originCard)
    {
        Transform targetArea = onCPU ? cpuStickerArea : playerStickerArea;
        Transform canvasRoot = targetArea.root; // Root canvas

        // Spawn sticker under canvas root
        var sticker = Instantiate(slapStickerPrefab, canvasRoot);
        sticker.text.text = abilityName.ToUpper();
        sticker.rectTransform.localScale = Vector3.one * 0.7f;
        sticker.rectTransform.localRotation = Quaternion.identity;

        // Get screen position of origin card
        Vector2 screenStartPos = RectTransformUtility.WorldToScreenPoint(null, originCard._shakeTarget.position);

        // Convert screen position to local position in canvas
        Vector2 localStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot as RectTransform,
            screenStartPos,
            null, // null because Screen Space - Overlay
            out localStartPos
        );
        sticker.rectTransform.localPosition = localStartPos;

        // Get screen position of the target area center
        Vector2 screenTargetPos = RectTransformUtility.WorldToScreenPoint(null, targetArea.position);
        Vector2 localTargetPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot as RectTransform,
            screenTargetPos,
            null,
            out localTargetPos
        );

        // Random slap rotation angle (e.g. -15° to 15°)
        float slapAngle = UnityEngine.Random.Range(-15f, 15f);

        // Animate slap
        Sequence seq = DOTween.Sequence();

        // Fly to target
        seq.Append(sticker.rectTransform.DOLocalMove(localTargetPos, 0.25f).SetEase(Ease.OutQuad));
        seq.Join(sticker.rectTransform.DOScale(Vector3.one * 1.2f, 0.25f));

        // Slap impact: rotate + squash
        seq.AppendCallback(() =>
        {
            sticker.rectTransform.localRotation = Quaternion.Euler(0f, 0f, slapAngle);
        });
        seq.Append(sticker.rectTransform.DOScale(new Vector3(1.2f, 0.6f, 1f), 0.07f).SetEase(Ease.OutCubic)); // squish
        seq.Append(sticker.rectTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack)); // settle

        // Hold, then fade
        seq.AppendInterval(1f);
        seq.OnComplete(() => Destroy(sticker.gameObject));
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
            if (!GameEnded)
            {
                currentTime = 0;
                TimerFinished();
            }    
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
        GameEnded = true;

        if (CPUScore <= PlayerScore)
        {
            playerWon = true;
            StartCoroutine(voicelinemanager.PlayCPULostVoiceLine());
        }
        else
        {
            playerWon = false;
            StartCoroutine(voicelinemanager.PlayCPUWonVoiceLine());
        }
            
        resultManager.ShowResultScreen(playerWon);
    }
    #endregion

    #region countdown
    private IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = (int)GameManager.Instance.StartDelay; i > 0; i--)
        {
            countdownText.text = i.ToString();
            PlayCountdownTickSFX();
            AnimatePunch();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "START!";
        PlayCountdownStartSFX();
        AnimatePunch();
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    private void AnimatePunch()
    {
        countdownText.transform.DOKill(); // Stop any previous tweens

        countdownText.transform.localScale = new Vector3(0.6f, 1.4f, 1f); // Stretch vertically
        countdownText.transform.localRotation = Quaternion.Euler(0, 0, -25f); // Pre-rotate for whip effect

        Sequence seq = DOTween.Sequence();

        // Scale + rotate to default
        seq.Append(countdownText.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
        seq.Join(countdownText.transform.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutExpo));

        // Optional secondary "wiggle"
        seq.Append(countdownText.transform.DORotate(new Vector3(0, 0, 10f), 0.1f).SetEase(Ease.InOutSine));
        seq.Append(countdownText.transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.InOutSine));
    }
    #endregion

    #region Audio

    private void PlayLoopCompletedSFX()
    {
        var instance = RuntimeManager.CreateInstance(loopScoredSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
    private void PlayChainCompletedSFX()
    {
        var instance = RuntimeManager.CreateInstance(chainScoredSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
    private void PlayCountdownTickSFX()
    {
        var instance = RuntimeManager.CreateInstance(countdownTickSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
    private void PlayCountdownStartSFX()
    {
        var instance = RuntimeManager.CreateInstance(countdownStartSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }

    #endregion
}


