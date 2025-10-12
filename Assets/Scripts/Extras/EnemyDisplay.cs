using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EnemyDisplay : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private Image enemyHealthBar;

    private EnemysSO enemyData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        enemyNameText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enemyNameText.gameObject.SetActive(false);
    }

    public void SetEnemyData(EnemysSO enemy)
    {
        enemyData = enemy;
        if (enemyData != null)
        {
            enemyNameText.text = enemyData.enemyName;
            UpdateHealthDisplay(enemyManager.currentHealth, enemyData.health);
        }
    }

    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        enemyHealthText.text = "Can: " + currentHealth.ToString();
        enemyHealthBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
