//using System.Collections.Generic;
//using DG.Tweening;
//using TMPro;
//using UnityEngine;

//public class WordSlotManager : MonoBehaviour
//{
//    public GameObject wordPrefab;
//    public Transform wordParent;

//    public float baseRadius = 200f;
//    public float degreesPerLetter = 10f;
//    public float wordSpacingDegrees = 5f;
//    public float inputAngle = 0f; // Input is at 0°, so first word goes at 180°

//    private List<CurvedWordDisplay> curvedWords = new();

//    public int wordCount => curvedWords.Count;

//    public void AddWord(string word)
//    {
//        var wordObj = Instantiate(wordPrefab, wordParent);
//        var curved = wordObj.GetComponent<CurvedWordDisplay>();
//        curved.radius = baseRadius;
//        curved.degreesPerLetter = degreesPerLetter;
//        curved.SetWord(word, 0); // Temp angle

//        curvedWords.Insert(0, curved);

//        float angleCursor = 180f;
//        for (int i = 0; i < curvedWords.Count; i++)
//        {
//            var wordDisplay = curvedWords[i];
//            float span = wordDisplay.GetLetterCount() * degreesPerLetter;

//            if (i == 0)
//            {
//                AnimateToAngle(wordDisplay, angleCursor);
//            }
//            else
//            {
//                float prevSpan = curvedWords[i - 1].GetLetterCount() * degreesPerLetter;
//                float shift = (prevSpan + span) / 2f + wordSpacingDegrees;
//                angleCursor += shift;

//                AnimateToAngle(wordDisplay, angleCursor);
//            }
//        }
//    }

//    private void AnimateToAngle(CurvedWordDisplay display, float targetAngle)
//    {
//        DOTween.To(
//            () => display.centerAngle,
//            a =>
//            {
//                display.centerAngle = a;
//                display.ApplyCurve();
//            },
//            targetAngle,
//            0.3f
//        ).SetEase(Ease.OutQuad);
//    }
//    public string GetLastLetter()
//    {
//        if (curvedWords.Count == 0) return "";
//        string latestWord = curvedWords[0].GetComponent<TextMeshProUGUI>().text.Trim();
//        return latestWord.Length > 0 ? latestWord[^1].ToString().ToLower() : "";
//    }
//    public string GetLastWord()
//    {
//        return curvedWords.Count == 0 ? "" : curvedWords[0].GetComponent<TextMeshProUGUI>().text;
//    }
//}

//using System.Collections.Generic;
//using DG.Tweening;
//using TMPro;
//using UnityEngine;

//public class WordSlotManager : MonoBehaviour
//{
//    public GameObject wordPrefab;
//    public Transform wordParent;
//    public float baseRadius = 200f;
//    public float wordSpacingDegrees = 5f;

//    private List<CurvedWordDisplay> curvedWords = new();
//    public int wordCount => curvedWords.Count;

//    public void AddWord(string word)
//    {
//        var wordObj = Instantiate(wordPrefab, wordParent);
//        var curved = wordObj.GetComponent<CurvedWordDisplay>();
//        curved.radius = baseRadius;
//        curved.SetWord(word, 0); // Temp angle

//        curvedWords.Insert(0, curved);

//        // Start placing from input angle (0°), go counterclockwise
//        float angleCursor = 0f;
//        for (int i = 0; i < curvedWords.Count; i++)
//        {
//            var wordDisplay = curvedWords[i];
//            float arc = wordDisplay.GetArcAngle();
//            angleCursor -= arc / 2f;
//            AnimateToAngle(wordDisplay, angleCursor);
//            angleCursor -= arc / 2f + wordSpacingDegrees;
//        }
//    }

//    private void AnimateToAngle(CurvedWordDisplay display, float angle)
//    {
//        DOTween.To(() => display.GetAngle(), display.SetAngle, angle, 0.3f);
//    }

//    public string GetLastLetter()
//    {
//        if (curvedWords.Count == 0) return "";
//        string latestWord = curvedWords[0].GetComponent<TextMeshProUGUI>().text.Trim();
//        return latestWord.Length > 0 ? latestWord[^1].ToString().ToLower() : "";
//    }
//}

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
    public int wordCount => curvedWords.Count;

    public void AddWord(string word)
    {
        var wordObj = Instantiate(wordPrefab, wordParent);
        var curved = wordObj.GetComponent<CurvedWordDisplay>();
        curved.radius = baseRadius;
        curved.SetWord(word, 0); // Temp angle

        curvedWords.Insert(0, curved);

        // Start placing from 180° (left of input field), go counterclockwise
        float angleCursor = 130f;
        for (int i = 0; i < curvedWords.Count; i++)
        {
            var wordDisplay = curvedWords[i];
            float arc = wordDisplay.GetArcAngle();
            angleCursor += arc / 2f;
            AnimateToAngle(wordDisplay, angleCursor);
            angleCursor += arc / 2f + wordSpacingDegrees;
        }
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
