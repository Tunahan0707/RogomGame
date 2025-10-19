using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static event Action OnEnemyDied;
    [Header("Enemy Settings")]
    public EnemysSO currentEnemy;

    [SerializeField] private EnemiesDataBase enemiesDataBase;
    [SerializeField] private EnemyManagerUI enemyManagerUI;

    private EnemyDisplay enemyDisplay;

    [HideInInspector] 
    public int currentHealth;
    public int shield;

    private int maxHealth;

    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void Awake()
    {
        if (enemyManagerUI == null)
            enemyManagerUI = FindAnyObjectByType<EnemyManagerUI>();
        if (currentEnemy != null)
            currentHealth = loadedData.currentEnemyHP;
    }

    public void SelectEnemy(EnemyType type)
    {
        currentEnemy = enemiesDataBase.GetEnemyByType(type);
        currentHealth = currentEnemy.health;
        shield = currentEnemy.baseShield;
        if (currentEnemy == null) return;
        enemyManagerUI.SpawnEnemy(currentEnemy);
        enemyDisplay = EnemyDisplay.GetEnemyDisplay(currentEnemy.enemyID);
    }
    public void SpawnEnemyByID(string id)
    {
        currentEnemy = enemiesDataBase.GetEnemyByID(id);
        currentHealth = loadedData.currentEnemyHP;
        shield = loadedData.currentEnemyShield;
        enemyManagerUI.SpawnEnemy(currentEnemy);
        enemyDisplay = EnemyDisplay.GetEnemyDisplay(id);
    }


    // Hasar alma fonksiyonu
    public void TakeDamage(int amount)
    {
        if (shield > 0)
        {
            shield -= amount;
            if (shield <= 0)
            {
                currentHealth += shield;
                shield = 0;
            }
        }
        else
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        enemyDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }
    private void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        enemyDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }
    private void AddShield(int addedShield)
    {
        shield += addedShield;
        enemyDisplay.UpdateShieldDisplay(shield, maxHealth);
    }
    public string GetEnemyID()
    {
        return currentEnemy != null ? currentEnemy.enemyID : "0";
    }
    public int GetEnemyHealth()
    {
        return currentHealth;
    }
    public int GetEnemyShield()
    {
        return shield;
    }
    private void Die()
    {
        OnEnemyDied?.Invoke();
        currentEnemy = null;
        enemyDisplay.DestroyEnemy();
    }
    public void EnemyAlgoritm(int x)
    {
        
    }
}
