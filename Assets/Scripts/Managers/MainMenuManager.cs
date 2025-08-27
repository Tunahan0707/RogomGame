using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button playButton;
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
    private bool isSettingsPanelVisible = false;

    void Awake()
    {
        playButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.GAME));
        libraryButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.LIBRARY));
        sacrificeButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.SACRIFICE));
        quitButton.onClick.AddListener(Application.Quit);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        characterButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.CHARACTER));
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
