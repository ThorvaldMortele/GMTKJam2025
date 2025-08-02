using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryLoader : MonoBehaviour
{
    public HashSet<string> WordSet { get; private set; }
    public bool IsReady { get; private set; } = false;

    public IEnumerator LoadWordsCoroutine()
    {
        ResourceRequest request = Resources.LoadAsync<TextAsset>("words");
        yield return request;

        TextAsset wordFile = request.asset as TextAsset;
        string[] words = wordFile.text.Split('\n');
        WordSet = new HashSet<string>();

        foreach (var word in words)
        {
            string clean = word.Trim().ToLower();
            if (!string.IsNullOrEmpty(clean))
                WordSet.Add(clean);
        }

        IsReady = true;
    }

    public bool IsValidWord(string word)
    {
        return IsReady && WordSet.Contains(word.ToLower());
    }
}