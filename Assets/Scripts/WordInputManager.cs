using UnityEngine;
using TMPro;
using System.Collections;

public class WordInputManager : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public WordSlotManager slotManager;
    public TMP_Text feedbackText;
    public DictionaryLoader DictionaryLoader;

    public string currentInput = "";
    public bool CanInput = false;
    public bool IsCPU = false;

    void Update()
    {
        if (CanInput && !IsCPU)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput[..^1];
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    TrySubmitWord();
                }
                else if (char.IsLetter(c))
                {
                    currentInput += c;
                }
            }

            inputText.text = currentInput.ToUpper();
        }
    }

    public void TrySubmitWord()
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

        if (word.Length <= 1 || !DictionaryLoader.IsValidWord(word))
        {
            ShowFeedback("Invalid word.");
            return;
        }

        if (slotManager.AllUsedWords.Contains(word))
        {
            ShowFeedback("Already used this word.");
            return;
        }

        slotManager.AddWord(word);
        currentInput = "";
        ShowFeedback(word + " added");
    }

    private void ShowFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
    }
}
