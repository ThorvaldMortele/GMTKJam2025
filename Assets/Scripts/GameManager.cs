using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WordSlotManager wordSlotManager;

    void Start()
    {
        wordSlotManager.AddWord("apple");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
