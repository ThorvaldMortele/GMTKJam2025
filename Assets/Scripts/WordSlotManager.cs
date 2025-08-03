using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public CPUPlayer CPU;
    public int MaxWordCountBeforeChainEnd = 15;
    public Color TriggerWordColorInLoop; //ADCBE7
    public Color DefaultWordColorInLoop;

    public List<CurvedWordDisplay> curvedWords = new();
    
    public Action OnLoopCompleted;
    public Action OnChainCompleted;

    public int wordCount => curvedWords.Count;

    public string firstWordFirstLetter = ""; //First letter

    public void AddWord(string word, bool isTriggerWord)
    {
        if (InputManager.hintText != null)
        {
            if (InputManager.hintText.text != "") InputManager.hintText.text = "";
        } 

        if (wordCount >= 1 && word[^1].ToString().ToLower() == firstWordFirstLetter.ToLower())
        {
            Debug.Log("Loop completed!");

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

            StartCoroutine(ShiftThenReset(true));
            return;
        }

        if (wordCount >= MaxWordCountBeforeChainEnd)
        {
            Debug.Log("Chain completed!");

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

            StartCoroutine(ShiftThenReset(false));
            return;
        }

        StartUpLoop(word, isTriggerWord);

        foreach (var wordDisplay in curvedWords)
        {
            string displayText = wordDisplay.textField.text.ToLower().Trim();

            if (InputManager.Abilities.Any(tw => tw.Name.ToLower().Trim() == displayText))
            {
                wordDisplay.textField.color = TriggerWordColorInLoop;
            }
            else
            {
                wordDisplay.textField.color = DefaultWordColorInLoop; // Reset non-trigger words
            }
        }

        RecalculateLayout();
    }

    private IEnumerator ShiftThenReset(bool isloop)
    {
        yield return StartCoroutine(ShiftWordsLeftCoroutine(isloop));
    }

    public void ShiftWordsLeftUntilOffscreen(bool isloop)
    {
        StartCoroutine(ShiftWordsLeftCoroutine(isloop));
    }

    private IEnumerator ShiftWordsLeftCoroutine(bool isloop)
    {
        float shiftAmount = 20f;

        while (true)
        {
            if (!IsCPU)
                InputManager.CanInput = false;
            else
                CPU.isStunned = true;

            // Track starting angle
            float startAngle = curvedWords[0].GetAngle();

            // Apply tweens
            foreach (var word in curvedWords)
            {
                float newAngle = word.GetAngle() + shiftAmount;
                DOTween.To(() => word.GetAngle(), word.SetAngle, newAngle, 0.15f).SetEase(Ease.OutQuad);

                float current2Angle = word.GetAngle();

                if (current2Angle > 390f) word.ToggleWordVisible(0);
            }

            // Check again after tween
            float currentAngle = curvedWords[0].GetAngle();
            if (currentAngle > 390f)
            {
                ResetChain();
                if (isloop) OnLoopCompleted?.Invoke();
                else OnChainCompleted?.Invoke();

                if (!IsCPU)
                    InputManager.CanInput = true;
                else
                    CPU.isStunned = false;
                break;
            }
                
            // Wait for tween to complete
            yield return new WaitForSeconds(0.15f);
        }
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