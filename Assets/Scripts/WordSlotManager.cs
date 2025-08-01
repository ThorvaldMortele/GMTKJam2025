using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordSlotManager : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform wordParent;

    public float baseRadius = 200f;
    public float spacingDegrees = 30f;
    public int maxWords = 10;

    public int wordCount => wordObjects.Count;

    private List<GameObject> wordObjects = new();

    public void AddWord(string word)
    {
        if (wordObjects.Count >= maxWords)
        {
            Destroy(wordObjects[0]);
            wordObjects.RemoveAt(0);
        }

        GameObject obj = Instantiate(wordPrefab, wordParent);
        CurvedWordDisplay display = obj.GetComponent<CurvedWordDisplay>();

        float angle = wordObjects.Count * -spacingDegrees;
        display.radius = baseRadius;
        display.degreesPerLetter = 10f;
        display.SetWord(word, angle);

        wordObjects.Add(obj);
        UpdateWordPositions();
    }

    private void UpdateWordPositions()
    {
        for (int i = 0; i < wordObjects.Count; i++)
        {
            var display = wordObjects[i].GetComponent<CurvedWordDisplay>();
            if (display == null) continue;

            float angle = i * -spacingDegrees;
            string word = display.GetComponent<TextMeshProUGUI>().text;
            display.SetWord(word, angle);
        }
    }

    public string GetLastLetter()
    {
        if (wordObjects.Count == 0) return "";

        var display = wordObjects[^1].GetComponent<TextMeshProUGUI>();
        if (display == null) return "";

        string lastWord = display.text.Trim();
        if (string.IsNullOrEmpty(lastWord)) return "";

        return lastWord[^1].ToString().ToLower();
    }

    public string GetLastWord()
    {
        if (wordObjects.Count == 0) return "";
        return wordObjects[^1].GetComponent<TextMeshProUGUI>().text;
    }
}
