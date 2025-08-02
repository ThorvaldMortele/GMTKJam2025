using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WordSlotManager : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform wordParent;
    public float baseRadius = 200f;
    public float wordSpacingDegrees = 5f;

    private List<CurvedWordDisplay> curvedWords = new();
    public List<string> AllUsedWords = new List<string>();

    public int wordCount => curvedWords.Count;

    public void AddWord(string word)
    {
        var wordObj = Instantiate(wordPrefab, wordParent);
        var curved = wordObj.GetComponent<CurvedWordDisplay>();
        curved.radius = baseRadius;
        curved.SetWord(word, 0); // Temp angle

        AllUsedWords.Add(word);
        curvedWords.Insert(0, curved);

        // Start placing from 130° (left of input field), go counterclockwise
        float angleCursor = 130f;
        for (int i = 0; i < curvedWords.Count; i++)
        {
            var wordDisplay = curvedWords[i];
            float arc = wordDisplay.GetArcAngle();
            angleCursor += arc / 2f;
            AnimateToAngle(wordDisplay, angleCursor);

            if (angleCursor > 390) wordDisplay.ToggleWordVisible(0);
            else wordDisplay.ToggleWordVisible(1);

            angleCursor += arc / 2f + wordSpacingDegrees;
        }
    }

    public void RemoveWord()
    {

    }

    private void AnimateToAngle(CurvedWordDisplay display, float angle)
    {
        DOTween.To(() => display.GetAngle(), display.SetAngle, angle, 0.3f);
    }

    public string GetLastLetter()
    {
        if (curvedWords.Count == 0) return "";
        string latestWord = curvedWords[0].GetComponent<TextMeshProUGUI>().text.Trim();
        return latestWord.Length > 0 ? latestWord[^1].ToString().ToLower() : "";
    }
}
