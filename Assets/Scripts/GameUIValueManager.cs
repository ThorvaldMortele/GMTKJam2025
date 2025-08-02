using TMPro;
using UnityEngine;

//Get the different UI asppects that need values
//Get a value parameter 
//Other scripts should fill in that value 

public class GameUIValueManager : MonoBehaviour
{
    [Header("Player Values")]
    public int playerLoopCount = 0;
    public int playerWordCount = 0;
    public string playerLastUsedLetter = ""; //The last letter of the previous word, will be the first letter of their next word (Can change with abilities)
    public string playerLoopLetter = ""; //The letter they should finish their word with to form a loop

    [Header("Player UI")]
    [SerializeField] private TMP_Text _uiPlayerLoopCountText;
    [SerializeField] private TMP_Text _uiPlayerWordCountText;
    [SerializeField] private TMP_Text _uiPlayerStartLetterReminderText;
    [SerializeField] private TMP_Text _uiPlayerEndLoopLetterReminderText;

    [Header("Opponent Values")]
    public int opponentLoopCount = 0;
    public int opponentWordCount = 0;
    public string opponentLastUsedLetter = ""; //The last letter of the previous word, will be the first letter of their next word (Can change with abilities)
    public string opponentLoopLetter = ""; //The letter they should finish their word with to form a loop

    [Header("Opponent UI")]
    [SerializeField] private TMP_Text _uiOpponentLoopCountText;
    [SerializeField] private TMP_Text _uiOpponentWordCountText;
    [SerializeField] private TMP_Text _uiOpponentStartLetterReminderText;
    [SerializeField] private TMP_Text _uiOpponentEndLoopLetterReminderText;


    public void UpdateGameUIValues()
    {
        UpdatePlayerUI();
        UpdateOpponentUI();
    }


    private void UpdatePlayerUI()
    {
        _uiPlayerLoopCountText.text = $"Score: {playerLoopCount}";
        _uiPlayerWordCountText.text = $"Words: {playerWordCount}";
        _uiPlayerStartLetterReminderText.text = $"Input a word starting with [{playerLastUsedLetter}].";
        _uiPlayerEndLoopLetterReminderText.text = $"End with [{playerLoopLetter}] to form a loop.";
    }

    private void UpdateOpponentUI()
    {
        _uiOpponentLoopCountText.text = $"Score: {opponentLoopCount}";
        _uiOpponentWordCountText.text = $"Words: {opponentWordCount}";
        _uiOpponentStartLetterReminderText.text = $"Input a word starting with [{opponentLastUsedLetter}].";
        _uiOpponentEndLoopLetterReminderText.text = $"End with [{opponentLoopLetter}] to form a loop.";
    }
}