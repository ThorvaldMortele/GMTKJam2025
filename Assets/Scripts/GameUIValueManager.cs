using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

//Get the different UI asppects that need values
//Get a value parameter 
//Other scripts should fill in that value 

public class GameUIValueManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    [Header("Player Values")]
    public string playerLastUsedLetter = ""; //The last letter of the previous word, will be the first letter of their next word (Can change with abilities)
    public string playerLoopLetter = ""; //The letter they should finish their word with to form a loop
    [SerializeField] private WordSlotManager _playerSlotManager;

    [Header("Player UI")]
    [SerializeField] private TMP_Text _uiPlayerScoreCountText;
    [SerializeField] private TMP_Text _uiPlayerLoopCountText;
    [SerializeField] private TMP_Text _uiPlayerChainCountText;
    [SerializeField] private TMP_Text _uiPlayerCurrentWordCountText;

    [SerializeField] private TMP_Text _uiPlayerStartLetterReminderText;
    [SerializeField] private TMP_Text _uiPlayerEndLoopLetterReminderText;

    [Header("Opponent Values")]
    public string opponentLastUsedLetter = ""; //The last letter of the previous word, will be the first letter of their next word (Can change with abilities)
    public string opponentLoopLetter = ""; //The letter they should finish their word with to form a loop
    [SerializeField] private WordSlotManager _opponentSlotManager;

    [Header("Opponent UI")]
    [SerializeField] private TMP_Text _uiOpponentScoreCountText;
    [SerializeField] private TMP_Text _uiOpponentLoopCountText;
    [SerializeField] private TMP_Text _uiOpponentChainCountText;
    [SerializeField] private TMP_Text _uiOpponentStartLetterReminderText;
    [SerializeField] private TMP_Text _uiOpponentEndLoopLetterReminderText;

    private void FixedUpdate()
    {
        UpdateGameUIValues();
    }


    public void UpdateGameUIValues()
    {
        UpdatePlayerUI();
        UpdateOpponentUI();
    }

    private void UpdatePlayerUI()
    {
        _uiPlayerScoreCountText.text = $"Score: ";
        _uiPlayerLoopCountText.text = $"Loops: {_gameManager.PlayerCompletedLoops}";
        _uiPlayerChainCountText.text = $"Chains: ";

        _uiPlayerCurrentWordCountText.text = $"Words: ";

        _uiPlayerStartLetterReminderText.text = $"Input a word starting with '{_playerSlotManager.GetLastLetter().ToUpper()}'.";
        _uiPlayerEndLoopLetterReminderText.text = $"End with '{_playerSlotManager.firstWordFirstLetter.ToUpper()}' to form a loop.";
    }
    private void UpdateOpponentUI()
    {
        _uiOpponentScoreCountText.text = $"Score: ";
        _uiOpponentLoopCountText.text = $"Loops: {_gameManager.CPUCompletedLoops}";
        _uiOpponentChainCountText.text = $"Chain: ";
        //_uiOpponentWordCountText.text = $"Words: {_gameManager.CPUWordsCount}";
        _uiOpponentStartLetterReminderText.text = $"Input a word starting with '{_opponentSlotManager.GetLastLetter().ToUpper()}'.";
        _uiOpponentEndLoopLetterReminderText.text = $"End with '{_opponentSlotManager.firstWordFirstLetter.ToUpper()}' to form a loop.";
    }
}