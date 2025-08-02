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
            inputManager.IsCPU = true;
            inputManager.CanInput = true;

            string lastLetter = GetLastLetterSafe();
            string wordToType = FindValidWordStartingWith(lastLetter, inputManager.DictionaryLoader.CPUWordSet);

            if (wordToType == null)
            {
                inputManager.feedbackText.text = $"CPU: No word for '{lastLetter}'";
                yield break;
            }

            inputManager.inputText.text = "";
            for (int i = 0; i < wordToType.Length; i++)
            {
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
                    inputManager.TrySubmitWord();

                    yield return new WaitForSeconds(Random.Range(MinDelayBetweenWords, MaxDelayBetweenWords));
                }
            }

            yield return null; // small yield before next loop
        }
    }

    private string GetLastLetterSafe()
    {
        if (inputManager.slotManager.wordCount == 0)
            return ((char)('a' + Random.Range(0, 26))).ToString();
        return inputManager.slotManager.GetLastLetter();
    }

    private string FindValidWordStartingWith(string letter, HashSet<string> wordset)
    {
        var allWords = wordset
            .Where(w => w.Length > 1
                     && w.StartsWith(letter)
                     && !inputManager.slotManager.AllUsedWords.Contains(w))
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
        }

        // fallback to any valid non-closing word
        return allWords
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        //return inputManager.DictionaryLoader.WordSet
        //    .Where(w => w.Length > 1
        //             && w.StartsWith(letter)
        //             && !inputManager.slotManager.AllUsedWords.Contains(w))
        //    .OrderBy(w => Random.value)
        //    .FirstOrDefault();
    }
}