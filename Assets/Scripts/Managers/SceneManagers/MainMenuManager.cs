using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button libraryButton;
    [SerializeField] private Button sacrificeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private GameObject settingsPanel;
    [Header("Settings")]
    [SerializeField] private float settingsPanelVisibleX = 0;
    [SerializeField] private float settingsPanelHiddenX = -500;
    [SerializeField] private float settingsPanelAnimationDuration = 0.5f;

    [Header("Managers")]
    [SerializeField] private GameStartManager gameStartManager;

    [Header("Databases")]
    [SerializeField] private PlayersDataBase pDB;
    private bool isSettingsPanelVisible = false;

    private void Awake()
    {

        newGameButton.onClick.AddListener(() => gameStartManager.NewGame());
        continueButton.onClick.AddListener(() => gameStartManager.ContinueGame());
        libraryButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.LIBRARY));
        sacrificeButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.SACRIFICE));
        quitButton.onClick.AddListener(Application.Quit);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        characterButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.CHARACTER));
    }
    private void Start()
    {
        bool saveExists = SaveManager.SaveExists(Consts.FileNames.FightDataFile);
        continueButton.interactable = saveExists;
        SkillMapManager.Instance.SetCurrentPlayer(pDB.GetPlayerByID(PlayerDataHolder.Instance.playerData.currentPlayerID));
    }

    private void OnSettingsButtonClick()
    {
        if (isSettingsPanelVisible)
        {
            HideSettingsPanel();
        }
        else
        {
            ShowSettingsPanel();
        }
        AudioManager.Instance.PlaySound(AudioManager.Instance.pageSound);
    }

    private void ShowSettingsPanel()
    {
        isSettingsPanelVisible = true;
        settingsPanel.GetComponent<RectTransform>().DOAnchorPosX(settingsPanelVisibleX, settingsPanelAnimationDuration);
    }

    private void HideSettingsPanel()
    {
        isSettingsPanelVisible = false;
        settingsPanel.GetComponent<RectTransform>().DOAnchorPosX(settingsPanelHiddenX, settingsPanelAnimationDuration);
    }
}
