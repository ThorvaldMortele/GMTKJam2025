using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WordSlotManager wordSlotManager;
    public WordSlotManager wordSlotManagerCPU;
    public WordInputManager wordInputManager;
    public WordInputManager wordInputManagerCPU;
    public DictionaryLoader DictionaryLoader;
    public CPUPlayer CPU;

    public int PlayerCompletedLoops = 0;
    public int CPUCompletedLoops = 0;

    public float StartDelay = 3f;

    void Start()
    {
        wordSlotManager.OnLoopCompleted = OnPlayerLoopCompleted;
        wordSlotManagerCPU.OnLoopCompleted = OnCPULoopCompleted;
        StartCoroutine(LoadWords());
    }

    private IEnumerator LoadWords()
    {
        yield return new WaitForSeconds(StartDelay);

        yield return StartCoroutine(DictionaryLoader.LoadWordsCoroutine());

        wordSlotManager.AddWord("apple");
        wordSlotManagerCPU.AddWord("apple");

        StartGame();
    }

    private void StartGame()
    {
        wordInputManager.CanInput = true;
        wordInputManagerCPU.CanInput = true;
        CPU.StartPlaying();
    }

    private void OnPlayerLoopCompleted()
    {
        PlayerCompletedLoops++;
        string next = GetNextWordForPlayer();
        wordSlotManager.AddWord(next);
    }

    private void OnCPULoopCompleted()
    {
        CPUCompletedLoops++;
        string next = GetNextWordForCPU();
        wordSlotManagerCPU.AddWord(next);
    }

    private string GetNextWordForPlayer()
    {
        return DictionaryLoader.WordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => Random.value)
            .FirstOrDefault() ?? "loop";
    }

    private string GetNextWordForCPU()
    {
        return DictionaryLoader.WordSet
            .Where(w => w.Length > 1)
            .OrderBy(_ => Random.value)
            .FirstOrDefault() ?? "loop";
    }
}
