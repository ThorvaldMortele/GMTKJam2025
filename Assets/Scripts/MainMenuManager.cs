using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _creditScreen;
    [SerializeField] private GameObject _tutorialScreen;



    private void Awake()
    {
        _creditScreen.SetActive(false);
        _tutorialScreen.SetActive(false);
        _mainMenuScreen.SetActive(true);
    }

    public void ShowTutorial()
    {
        _mainMenuScreen.SetActive(false);
        _creditScreen.SetActive(false);
        _tutorialScreen.SetActive(true);
    }
    public void BackToMainMenu()
    {
        _creditScreen.SetActive(false);
        _tutorialScreen.SetActive(false);
        _mainMenuScreen.SetActive(true);
    }
    public void ShowCredits()
    {
        _mainMenuScreen.SetActive(false);
        _tutorialScreen.SetActive(false);
        _creditScreen.SetActive(true);
    }
    public void StartGameScene()
    {
        _mainMenuScreen.SetActive(false);
        _creditScreen.SetActive(false);
        _tutorialScreen.SetActive(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
