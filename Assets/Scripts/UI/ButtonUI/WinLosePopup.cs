using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLosePopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button returnToMainMenuButton;

    private void Awake()
    {
        returnToMainMenuButton.onClick.AddListener(ReturnMainMenu);
    }

    private void ReturnMainMenu()
    {
        SceneManager.LoadScene(Consts.Scenes.MAIN_MENU);
        SaveManager.DeleteSave(Consts.FileNames.FightDataFile);
    }

    private void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        GameSceneManager.OnContinueButtonClicked += FalseAll;
        PlayerManager.OnPlayerDied += ShowLosePopup;
        EnemyManager.OnEnemyDied += ShowWinPopup;
    }
    void OnDisable()
    {
        GameSceneManager.OnContinueButtonClicked -= FalseAll;
        PlayerManager.OnPlayerDied -= ShowLosePopup;
        EnemyManager.OnEnemyDied -= ShowWinPopup;
    }

    private void FalseAll()
    {
        winPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    public void ShowWinPopup()
    {
        winPanel.SetActive(true);
        continueButton.gameObject.SetActive(true);
    }

    public void ShowLosePopup()
    {
        losePanel.SetActive(true);
    }
}
