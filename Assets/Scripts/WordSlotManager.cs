using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WordSlotManager : MonoBehaviour
{
    public bool IsCPU = false;
    public GameObject wordPrefab;
    public Transform wordParent;
    public float baseRadius = 200f;
    public float wordSpacingDegrees = 5f;
    public WordInputManager InputManager;

    public List<CurvedWordDisplay> curvedWords = new();
    

    public Action OnLoopCompleted;

    public int wordCount => curvedWords.Count;

    public string firstWordFirstLetter = ""; //First letter

    public void AddWord(string word, bool isTriggerWord)
    {
        if (InputManager.hintText.text != "") InputManager.hintText.text = "";

        if (wordCount >= 1 && word[^1].ToString().ToLower() == firstWordFirstLetter.ToLower())
        {
            Debug.Log("Loop completed!");
            ResetChain();

            OnLoopCompleted?.Invoke();
            return;
        }

        StartUpLoop(word, isTriggerWord);
    }

    public void StartUpLoop(string word, bool isTriggerWord)
    {
        var wordObj = Instantiate(wordPrefab, wordParent);
        var curved = wordObj.GetComponent<CurvedWordDisplay>();
        curved.radius = baseRadius;
        curved.SetWord(word, 0); // Temp angle

        if (!isTriggerWord) //trigger words should be able to be used again over time, so no duplicate check
        {
            GameManager.Instance.AllUsedWords.Add(word);
        }

        curvedWords.Insert(0, curved);

        if (wordCount == 1)
            firstWordFirstLetter = word[0].ToString();

        RecalculateLayout();
    }

    public void RecalculateLayout()
    {
        float angleCursor = 130f;
        for (int i = 0; i < curvedWords.Count; i++)
        {
            var wordDisplay = curvedWords[i];
            float arc = wordDisplay.GetArcAngle();
            angleCursor += arc / 2f;
            DOTween.To(() => wordDisplay.GetAngle(), wordDisplay.SetAngle, angleCursor, 0.3f);

            wordDisplay.ToggleWordVisible(angleCursor > 390 ? 0 : 1);
            angleCursor += arc / 2f + wordSpacingDegrees;
        }
    }

    public void ResetChain()
    {
        foreach (var wordDisplay in curvedWords)
        {
            if (wordDisplay != null)
                Destroy(wordDisplay.gameObject);
        }

        curvedWords.Clear();
        firstWordFirstLetter = "";
    }

    public void RemoveLastWord()
    {
        if (curvedWords.Count == 0)
            return;

        // Remove the visual word
        var last = curvedWords[0];
        var tmp = last.textField.text;
        if (last != null)
            Destroy(last.gameObject);

        curvedWords.RemoveAt(0);

        // Also remove from the word list (if it's not a trigger word)
        if (GameManager.Instance.AllUsedWords.Count > 0)
            GameManager.Instance.AllUsedWords.RemoveAt(0);

        // Recalculate layout
        float angleCursor = 130f;
        for (int i = 0; i < curvedWords.Count; i++)
        {
            var wordDisplay = curvedWords[i];
            float arc = wordDisplay.GetArcAngle();
            angleCursor += arc / 2f;
            AnimateToAngle(wordDisplay, angleCursor);

            wordDisplay.ToggleWordVisible(angleCursor > 390 ? 0 : 1);
            angleCursor += arc / 2f + wordSpacingDegrees;
        }

        // Update first letter if needed
        if (curvedWords.Count == 0)
            firstWordFirstLetter = "";
        else
            firstWordFirstLetter = curvedWords[^1].GetComponent<TextMeshProUGUI>().text[0].ToString().ToLower();
    }


    private void AnimateToAngle(CurvedWordDisplay display, float angle)
    {
        DOTween.To(() => display.GetAngle(), display.SetAngle, angle, 0.3f);
    }

    public string GetLastLetter() //Last Letter
    {
        if (curvedWords.Count == 0) return "";
        string latestWord = curvedWords[0].GetComponent<TextMeshProUGUI>().text.Trim();
        return latestWord.Length > 0 ? latestWord[^1].ToString().ToLower() : "";
    }
}