using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] private Color _greenOverlayColor;
    [SerializeField] private Color _redOverlayColor;

    [Header("BG")]
    [SerializeField] private GameObject _resultScreen;

    [Header("Result Values")]
    [SerializeField] private TMP_Text _resultLoopsCountText;
    [SerializeField] private TMP_Text _resultWordsCountText;
    [SerializeField] private TMP_Text _resultAbilitiesUsedCountText;

    [SerializeField] private TMP_Text _resultText;


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

        _resultLoopsCountText.text = "Result | Result";
        _resultWordsCountText.text = "Result | Result";
        _resultAbilitiesUsedCountText.text = "Result | Result";

        _resultText.text = resultText;
        _resultText.color = resultColor;
    }

    public void PlayAgain()
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }
}
