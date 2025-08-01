using UnityEngine;
using TMPro;

public class WordInputManager : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public WordSlotManager slotManager;
    public TMP_Text feedbackText;

    private string currentInput = "";

    void Update()
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

    void TrySubmitWord()
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

        // Replace this with a real dictionary check if needed
        if (word.Length <= 1)
        {
            ShowFeedback("Invalid word.");
            return;
        }

        slotManager.AddWord(word);
        currentInput = "";
        ShowFeedback("");
    }

    void ShowFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
    }
}
