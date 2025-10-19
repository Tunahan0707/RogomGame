using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class EnemyDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private TextMeshProUGUI enemyShieldText;
    [SerializeField] private Image enemySprite;
    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private Image enemyShieldBar;
    [SerializeField] private Image enemyShieldBar2;

    public static Dictionary<string, EnemyDisplay> AllEnemys = new();

    private EnemysSO enemyData;

    private void Awake()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }

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
            enemySprite.sprite = enemyData.artwork;
            if (!AllEnemys.ContainsKey(enemyData.enemyID))
                AllEnemys.Add(enemyData.enemyID, this);
            enemyShieldBar2.fillAmount = 0f;
            enemyShieldBar.fillAmount = 0f;
            UpdateHealthDisplay(enemyManager.currentHealth, enemyData.health);
            UpdateShieldDisplay(enemyManager.shield, enemyData.health);
        }
    }

    public static EnemyDisplay GetEnemyDisplay(string id)
    {
        if (AllEnemys.TryGetValue(id, out var enemy))
            return enemy;
        return null;
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void UpdateShieldDisplay(int currentShield, int maxHealth)
    {
        float x = (float)currentShield / maxHealth;
        float y;
        enemyShieldText.text = currentShield.ToString();
        if (x >= 1)
        {
            y = x - 1;
            enemyShieldBar2.fillAmount = y;
            enemyShieldBar.fillAmount = 1;
        }
        else
            enemyShieldBar.fillAmount = x;

    }

    public void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        enemyHealthText.text = currentHealth.ToString();
        enemyHealthBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
