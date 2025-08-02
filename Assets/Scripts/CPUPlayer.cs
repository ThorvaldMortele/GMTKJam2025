using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CPUPlayer : MonoBehaviour
{
    public WordInputManager inputManager;
    public float minDelay = 0.3f;
    public float maxDelay = 0.7f;

    public float InitialStartDelay = 1f;
    public float MinDelayBetweenWords = 0.5f;
    public float MaxDelayBetweenWords = 2f;
    public float DelayWhenRethinking = 1f;

    public int MaxWordCountToFormLoop = 12;
    private bool isStunned = false;

    [Range(0f, 1f)]
    public float triggerWordChance = 0.9f; // 90% chance to use a trigger word

    private Coroutine playRoutine;

    public void StartPlaying()
    {
        if (playRoutine == null)
            playRoutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        yield return new WaitForSeconds(InitialStartDelay);

        while (true)
        {
            if (isStunned)
            {
                yield return null;
                continue;
            }

            inputManager.IsCPU = true;
            inputManager.CanInput = true;

            string lastLetter = GetLastLetterSafe();
            string wordToType = FindValidWordStartingWith(lastLetter, inputManager.DictionaryLoader.CPUWordSet);

            if (wordToType == "ImpossibleReset")
            {
                var startWord = GameManager.Instance.DictionaryLoader.CPUWordSet
                .Where(w => w.Length > 1
                     && !GameManager.Instance.AllUsedWords.Contains(w))
                .ToList()
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

                inputManager.slotManager.ResetChain();
                inputManager.slotManager.StartUpLoop(startWord, false);
                yield return null;
            }

            if (wordToType == null)
            {
                inputManager.feedbackText.text = $"CPU: No word for '{lastLetter}'";
                yield break;
            }

            inputManager.inputText.text = "";
            for (int i = 0; i < wordToType.Length; i++)
            {
                if (isStunned)
                {
                    inputManager.feedbackText.text = "CPU: ...uhhh...";
                    yield return null;
                    break;
                }

                // Re-check last letter during typing
                string currentLast = GetLastLetterSafe();
                if (currentLast != lastLetter)
                {
                    inputManager.feedbackText.text = $"CPU: Restarting... letter changed to '{currentLast}'";
                    yield return new WaitForSeconds(DelayWhenRethinking);
                    break; // restart outer loop
                }

                inputManager.inputText.text += wordToType[i].ToString().ToUpper();
                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

                // If we typed the full word successfully, submit
                if (i == wordToType.Length - 1)
                {
                    inputManager.currentInput = wordToType;
                    inputManager.TrySubmitWord(inputManager.ActiveTriggerWords);

                    yield return new WaitForSeconds(Random.Range(MinDelayBetweenWords, MaxDelayBetweenWords));
                }
            }

            yield return null; // small yield before next loop
        }
    }

    public void TriggerBrainfart(float duration = 5f)
    {
        if (!isStunned)
            StartCoroutine(BrainfartRoutine(duration));
    }

    private IEnumerator BrainfartRoutine(float duration)
    {
        isStunned = true;
        inputManager.feedbackText.text = "CPU: ...uhhh...";
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private string GetLastLetterSafe()
    {
        if (inputManager.slotManager.wordCount == 0)
            return ((char)('a' + Random.Range(0, 26))).ToString();
        return inputManager.slotManager.GetLastLetter();
    }

    private string FindValidWordStartingWith(string letter, HashSet<string> wordset)
    {
        if (GameManager.Instance.ActiveTriggerWords != null && GameManager.Instance.ActiveTriggerWords.Count > 0)
        {
            if (Random.value < triggerWordChance)
            {
                var triggerWord = GameManager.Instance.ActiveTriggerWords
                    .Where(w => w.StartsWith(letter))
                    .OrderBy(_ => Random.value)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(triggerWord))
                {
                    Debug.Log($"CPU: Using trigger word '{triggerWord}'");
                    return triggerWord;
                }
            }
        }

        var allWords = wordset
            .Where(w => w.Length > 1
                     && w.StartsWith(letter)
                     && !GameManager.Instance.AllUsedWords.Contains(w))
            .ToList();

        if (allWords.Count == 0) return null;

        string firstLetterOfFirstWord = inputManager.slotManager.wordCount > 0
            ? inputManager.slotManager.curvedWords[^1].textField.text[0].ToString().ToLower()
            : "";

        int count = inputManager.slotManager.wordCount;
        float closeChance = Mathf.Clamp01(count / 10f); // up to 100% at 10+ words

        bool shouldTryToClose = Random.value < closeChance;

        if (shouldTryToClose)
        {
            string closingWord = allWords
                .FirstOrDefault(w => w[^1].ToString().ToLower() == firstLetterOfFirstWord);

            if (closingWord != null)
            {
                Debug.Log($"CPU: Closing loop with '{closingWord}' (chance {closeChance * 100:F0}%)");
                return closingWord;
            }
            else
            {
                return "ImpossibleReset";
            }
        }

        // fallback to any valid non-closing word
        return allWords
            .OrderBy(_ => Random.value)
            .FirstOrDefault();
    }
}