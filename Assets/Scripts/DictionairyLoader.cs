using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryLoader : MonoBehaviour
{
    public HashSet<string> WordSet { get; private set; }
    public HashSet<string> CPUWordSet { get; private set; }
    public bool IsReady { get; private set; } = false;

    public IEnumerator LoadWordsCoroutine()
    {
        ResourceRequest request0 = Resources.LoadAsync<TextAsset>("cpuwords");
        yield return request0;

        ResourceRequest request = Resources.LoadAsync<TextAsset>("words");
        yield return request;

        TextAsset wordFile = request.asset as TextAsset;
        string[] words = wordFile.text.Split('\n');
        WordSet = new HashSet<string>();

        TextAsset cpuwordFile = request0.asset as TextAsset;
        string[] cpuwords = cpuwordFile.text.Split('\n');
        CPUWordSet = new HashSet<string>();

        foreach (var word in words)
        {
            string clean = word.Trim().ToLower();
            if (!string.IsNullOrEmpty(clean))
                WordSet.Add(clean);
        }

        foreach (var word in cpuwords)
        {
            string clean = word.Trim().ToLower();
            if (!string.IsNullOrEmpty(clean))
                CPUWordSet.Add(clean);
        }

        IsReady = true;
    }

    public bool IsValidWord(string word, bool isCPU)
    {
        if (isCPU) return IsReady && CPUWordSet.Contains(word.ToLower());
        else return IsReady && WordSet.Contains(word.ToLower());
    }
}