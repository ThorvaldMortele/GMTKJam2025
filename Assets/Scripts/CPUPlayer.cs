using System.Collections;
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
            string wordToType = FindValidWordStartingWith(lastLetter);

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

    private string FindValidWordStartingWith(string letter)
    {
        return inputManager.DictionaryLoader.WordSet
            .Where(w => w.Length > 1
                     && w.StartsWith(letter)
                     && !inputManager.slotManager.AllUsedWords.Contains(w))
            .OrderBy(w => Random.value)
            .FirstOrDefault();
    }
}