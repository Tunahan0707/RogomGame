using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField] private Transform effectList;
    [SerializeField] private TextMeshProUGUI enemyIntentText;
    private string enemyID;
    private HashSet<EffectsDisplay> effectDisplays;

    public static Dictionary<string, EnemyDisplay> AllEnemys = new();

    public EnemysSO enemyData;
    [SerializeField] private EffectsDisplayUI effectsUI;

    private void Awake()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>();
        if (AllEnemys == null || AllEnemys.Count == 0)
            AllEnemys = new();
        if (effectsUI == null)
            effectsUI = FindFirstObjectByType<EffectsDisplayUI>();
        effectDisplays = new();
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
            enemyShieldBar2.fillAmount = 0f;
            enemyShieldBar.fillAmount = 0f;
            UpdateHealthDisplay(enemyManager.currentHealth, enemyData.health);
            UpdateShieldDisplay(enemyManager.shield, enemyData.health);
            if (string.IsNullOrEmpty(enemyID))
                enemyID = System.Guid.NewGuid().ToString();
            if (!AllEnemys.ContainsKey(enemyID))
                AllEnemys.Add(enemyID, this);
        }
    }
    public void ShowIntent(string action, string icon, int value)
    {
        if (value > 0)
            enemyIntentText.text = $"{icon} {action} {value}";
        else
            enemyIntentText.text = $"{icon} {action}";
    }

    public string GetEnemyID()
    {
        return enemyID;
    }

    public static EnemyDisplay GetEnemyDisplay(string id)
    {
        if (AllEnemys.TryGetValue(id, out var enemy))
            return enemy;
        return null;
    }

    public void DestroyEnemy()
    {
        // Önce sözlükten kaldır
        if (AllEnemys.ContainsValue(this))
        {
            string keyToRemove = null;
            foreach (var kvp in AllEnemys)
            {
                if (kvp.Value == this)
                {
                    keyToRemove = kvp.Key;
                    break;
                }
            }
            if (keyToRemove != null)
                AllEnemys.Remove(keyToRemove);
        }

        // Sonra objeyi sahneden sil
        Destroy(gameObject);
    }

    public void UpdateShieldDisplay(int currentShield, int maxHealth)
    {
        float x = (float)currentShield / maxHealth;
        float y;
        if (currentShield == 0)
            enemyShieldText.gameObject.SetActive(false);
        else
        {
            if (!enemyShieldText.gameObject.activeSelf)
                enemyShieldText.gameObject.SetActive(true);
            enemyShieldText.text = currentShield.ToString();
        }
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
        enemyHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        enemyHealthBar.fillAmount = (float)currentHealth / maxHealth;
    }
    public void SetEffects(int iconIndex, int value)
    {
        if (effectsUI == null)
            effectsUI = FindAnyObjectByType<EffectsDisplayUI>();

        if (effectsUI == null)
        {
            Debug.LogError("EffectsDisplayUI sahnede yok!");
            return;
        }

        // Önce effectDisplays'te var mı diye bak
        var existingEffect = effectDisplays.FirstOrDefault(e => e.iconIndex == iconIndex);

        if (existingEffect != null)
        {
            if (value == 0)
            {
                existingEffect.Destroy();
                effectDisplays.Remove(existingEffect);
            }
            else
            {
                existingEffect.SetDatas(effectsUI.Icons[iconIndex], value, iconIndex);
            }
        }
        else
        {
            // Effect yok ve value 0 değilse yeni spawn et
            if (value != 0)
            {
                var newEffect = effectsUI.SpawnEffect(iconIndex, value, effectList);
                if (newEffect != null)
                    effectDisplays.Add(newEffect);
            }
        }
    }
}
