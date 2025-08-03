using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine.EventSystems;

public class WordInputManager : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public WordSlotManager slotManager;
    public TMP_Text feedbackText;
    public DictionaryLoader DictionaryLoader;
    public List<Ability> Abilities;
    public List<string> ActiveTriggerWords = new List<string>();
    public List<WordSlotManager> SlotManagers;
    public TextMeshProUGUI hintText;

    public string currentInput = "";
    public bool CanInput = false;
    public bool IsCPU = false;

    [Header("Audio")]
    private int wordCombo = 0;
    public EventReference typingSFX;
    public EventReference wordInputSFX;
    public EventReference wordInputRemovedSFX;
    public EventReference abilityUsedImpactSFX;

    void Update()
    {
        if (CanInput && !IsCPU)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (currentInput.Length > 0)
                    {
                        currentInput = currentInput[..^1];
                        wordCombo--;
                        PlayLetterRemovedSFX();
                    }
                        
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    TrySubmitWord(ActiveTriggerWords);
                }
                else if (char.IsLetter(c))
                {
                    wordCombo++;
                    PlayTypingSFX();
                    currentInput += c;
                }
            }

            inputText.text = currentInput.ToUpper();
        }

        if (wordCombo <= 0)
            wordCombo = 0;
    }

    public void TriggerBrainfart(float duration = 5f)
    {
        StartCoroutine(BrainfartRoutine(duration));
    }

    private IEnumerator BrainfartRoutine(float duration)
    {
        CanInput = false;
        feedbackText.text = "...uhhh...";
        yield return new WaitForSeconds(duration);
        CanInput = true;
    }

    public void SetHintText(string value)
    {
        if (hintText != null)
            hintText.text = value;
    }

    public void TrySubmitWord(List<string> activeTriggerWords)
    {
        string word = currentInput.Trim().ToLower();
        if (word.Length == 0) return;

        if (slotManager.wordCount > 0)
        {
            string lastLetter = slotManager.GetLastLetter();
            if (word[0].ToString() != lastLetter)
            {
                ShowFeedback($"Word must start with '{lastLetter}'");
                return;
            }
        }

        if (word.Length <= 1 || !DictionaryLoader.IsValidWord(word, IsCPU))
        {
            ShowFeedback("Invalid word.");
            return;
        }

        if (activeTriggerWords.Contains(word))
        {
            slotManager.AddWord(word, true);
            currentInput = "";
            ShowFeedback(word + " added");

            var target = SlotManagers.Where(x => !x.IsCPU == IsCPU).FirstOrDefault();
            var ability = Abilities.Where(x => x.Name.ToLower() == word).FirstOrDefault();

            UseAbility(ability, target);
            PlayWordEnteredSFX();
            wordCombo = 0;
            return;
        }

        if (GameManager.Instance.AllUsedWords.Contains(word))
        {
            ShowFeedback("Already used this word.");
            return;
        }

        slotManager.AddWord(word, false);
        PlayWordEnteredSFX();

        currentInput = "";
        wordCombo = 0;
        ShowFeedback(word + " added");
    }

    public void ResetMouseClickFocus()
    {
        EventSystem.current.SetSelectedGameObject(inputText.gameObject);
    }

    public void ResetLoop()
    {
        var startWord = GameManager.Instance.DictionaryLoader.CPUWordSet
        .Where(w => w.Length > 1
         && !GameManager.Instance.AllUsedWords.Contains(w))
        .ToList()
        .OrderBy(_ => Random.value)
        .FirstOrDefault();

        slotManager.ResetChain();
        slotManager.StartUpLoop(startWord, false);
    }

    public void UseAbility(Ability ability, WordSlotManager target) //Audio
    {
        if (ability?.Effect != null)
        {
            bool slapCPU = target.IsCPU;

            if (slapCPU) GameManager.Instance.PlayerAbilitiesUsed++;
            else GameManager.Instance.CPUAbilitiesUsed++;

            var originCard = GameManager.Instance.CurrentActiveAbilityCards
            .FirstOrDefault(c => c._abilityNameText.text.ToLower() == ability.Name.ToLower());

            GameManager.Instance.ShowAbilitySticker(ability.Name, slapCPU, originCard);
            PlayAbilityImpactSFX();

            ability.Effect.Apply(target, slotManager);
            GameManager.Instance.GenerateNewTriggerWord(ability.Name);
        }
    }

    private void ShowFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
    }

    private void PlayTypingSFX()
    {
        if (wordCombo >= 10) //10 is maximum
            wordCombo = 10;

        var instance = RuntimeManager.CreateInstance(typingSFX.Guid);
        instance.setParameterByName("parameter:/TypingCombo", wordCombo);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
    private void PlayLetterRemovedSFX()
    {
        if (!IsCPU)
        {
            var instance = RuntimeManager.CreateInstance(wordInputRemovedSFX.Guid);

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
            instance.start();
            instance.release();
        }
    }

    private void PlayWordEnteredSFX()
    {
        if(!IsCPU)
        {
            var instance = RuntimeManager.CreateInstance(wordInputSFX.Guid);

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
            instance.start();
            instance.release();
        }
    }

    private void PlayAbilityImpactSFX()
    {
        //Need to play only if it is not the CPU?

        var instance = RuntimeManager.CreateInstance(abilityUsedImpactSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
}
