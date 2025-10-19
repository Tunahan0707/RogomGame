using UnityEngine;

public class WinLosePopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }
    void OnEnable()
    {
        PlayerManager.OnPlayerDied += ShowLosePopup;
        EnemyManager.OnEnemyDied += ShowWinPopup;
    }

    void OnDisable()
    {
        PlayerManager.OnPlayerDied -= ShowLosePopup;
        EnemyManager.OnEnemyDied -= ShowWinPopup;
    }
    public void ShowWinPopup()
    {
        winPanel.SetActive(true);
    }

    public void ShowLosePopup()
    {
        losePanel.SetActive(true);
    }
}
