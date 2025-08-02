using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public int PlayerCompletedLoops = 0;
    public int CPUCompletedLoops = 0;

    public float StartDelay = 3f;

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
        StartCoroutine(LoadWords());
    }

    private IEnumerator LoadWords()
    {
        yield return new WaitForSeconds(StartDelay);

        yield return StartCoroutine(DictionaryLoader.LoadWordsCoroutine());

        wordSlotManager.AddWord("apple", false);
        wordSlotManagerCPU.AddWord("apple", false);

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
        PlayerCompletedLoops++;
        string next = GetNextWordForPlayer();

        bool istriggerword;
        if (ActiveTriggerWords.Contains(next)) istriggerword = true;
        else istriggerword = false;

        wordSlotManager.AddWord(next, istriggerword);
    }

    private void OnCPULoopCompleted()
    {
        CPUCompletedLoops++;
        string next = GetNextWordForCPU();


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
}
