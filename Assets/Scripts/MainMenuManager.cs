using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using ntw.CurvedTextMeshPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _creditScreen;
    [SerializeField] private GameObject _tutorialScreen;

    public TextProOnACircle TitleCircle;
    public TextProOnACircle SoupmanCircle;
    public TextProOnACircle SharksCircle;
    public TextProOnACircle CreditsTitleCircle;
    private float angle = 10;
    public float Speed = 10;

    private void Awake()
    {
        _creditScreen.SetActive(false);
        _tutorialScreen.SetActive(false);
        _mainMenuScreen.SetActive(true);
    }

    private void Update()
    {
        angle += Time.deltaTime * Speed;
        TitleCircle.m_angularOffset = angle;
        SharksCircle.m_angularOffset = angle;
        SoupmanCircle.m_angularOffset = angle;
        CreditsTitleCircle.m_angularOffset = angle;
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
