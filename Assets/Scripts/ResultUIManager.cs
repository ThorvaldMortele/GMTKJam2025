using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private Color _greenOverlayColor;
    [SerializeField] private Color _redOverlayColor;

    [Header("BG")]
    [SerializeField] private GameObject _resultScreen;

    [Header("Result Values")]
    [SerializeField] private TMP_Text _resultScoreCountText;
    [SerializeField] private TMP_Text _resultLoopsCountText;
    [SerializeField] private TMP_Text _resultChainsCountText;
    [SerializeField] private TMP_Text _resultAbilitiesUsedCountText;

    [SerializeField] private TMP_Text _resultText;

    [Header("Audio")]
    public EventReference gameWonSFX;
    public EventReference gameLostSFX;

    private void Awake()
    {
        _resultScreen.SetActive(false);
    }

    public void ShowResultScreen(bool playerWon)
    {
        SetResultScreen(playerWon);
        _resultScreen.SetActive(true);
    }

    private void SetResultScreen(bool playerWon)
    {
        //Determine result
        Color resultColor = Color.white;
        string resultText = string.Empty;

        if (playerWon)
        {
            resultColor = _greenOverlayColor;
            resultText = "You Won!";
        }            
        else
        {
            resultColor = _redOverlayColor;
            resultText = "You Lost!";
        }

        //Set Values

        _resultScoreCountText.text = $"{_gameManager.PlayerScore} | {_gameManager.CPUScore}";
        _resultLoopsCountText.text = $"{_gameManager.PlayerCompletedLoops} |  {_gameManager.CPUCompletedLoops}";
        _resultChainsCountText.text = $"{_gameManager.PlayerCompletedChains} | {_gameManager.CPUCompletedChains}";
        _resultAbilitiesUsedCountText.text = $"{_gameManager.PlayerAbilitiesUsed} | {_gameManager.CPUAbilitiesUsed}";

        _resultText.text = resultText;
        _resultText.color = resultColor;
    }

    public void PlayAgain()
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void GameWonSFX()
    {
        var instance = RuntimeManager.CreateInstance(gameWonSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
    public void GameLostSFX()
    {
        var instance = RuntimeManager.CreateInstance(gameLostSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
}
