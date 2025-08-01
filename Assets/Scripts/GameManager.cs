using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WordSlotManager wordSlotManager;
    public DictionaryLoader DictionaryLoader;

    void Start()
    {
        StartCoroutine(LoadWords());
    }

    private IEnumerator LoadWords()
    {
        yield return StartCoroutine(DictionaryLoader.LoadWordsCoroutine());

        wordSlotManager.AddWord("apple");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
