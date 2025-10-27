using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public static event Action OnEnemyDied;
    public static event Action<EnemyAlgoritmController> OnEnemySelected;
    [Header("Enemy Settings")]
    public EnemysSO currentEnemy;

    [SerializeField] private EnemiesDataBase enemiesDataBase;
    [SerializeField] private EnemyManagerUI enemyManagerUI;

    public EnemyDisplay enemyDisplay { get; private set; }

    [HideInInspector] 
    public int currentHealth { get; private set; }
    public int shield { get; private set; }

    public List<string> currentEnemys;
    private int maxHealth;

    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (enemyManagerUI == null)
            enemyManagerUI = FindAnyObjectByType<EnemyManagerUI>();
        if (currentEnemy != null)
        {
            currentHealth = loadedData.currentEnemyHP;
            shield = loadedData.currentEnemyShield;
        }

    }


    public void SelectEnemy(EnemyType type)
    {
        currentEnemy = enemiesDataBase.GetEnemyByType(type);
        currentHealth = currentEnemy.health;
        maxHealth = currentEnemy.health;
        shield = currentEnemy.baseShield;
        if (currentEnemy == null) return;
        enemyManagerUI.SpawnEnemy(currentEnemy);
        currentEnemys.AddRange(EnemyManagerUI.currentEnemys);
        enemyDisplay = EnemyDisplay.GetEnemyDisplay(currentEnemys[0]);
        var ai = enemyDisplay.GetComponent<EnemyAlgoritmController>();
        OnEnemySelected?.Invoke(ai);
        ai.DecideNextPlan();
    }
    public void SpawnEnemyByID(string id)
    {
        enemyDisplay = null;
        currentEnemy = enemiesDataBase.GetEnemyByID(id);
        currentHealth = loadedData.currentEnemyHP;
        maxHealth = currentEnemy.health;
        shield = loadedData.currentEnemyShield;
        enemyManagerUI.SpawnEnemy(currentEnemy);
        currentEnemys.AddRange(EnemyManagerUI.currentEnemys);
        enemyDisplay = EnemyDisplay.GetEnemyDisplay(currentEnemys[0]);
        var ai = enemyDisplay.GetComponent<EnemyAlgoritmController>();
        OnEnemySelected?.Invoke(ai);
        ai.UpdateCurrentPlan();
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
        enemyDisplay.UpdateShieldDisplay(shield, maxHealth);
        enemyDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }
    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        enemyDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }
    public void AddShield(int addedShield)
    {
        if (addedShield < 0) return;
        shield += addedShield;
        enemyDisplay.UpdateShieldDisplay(shield, maxHealth);
    }
    private void Die()
    {
        OnEnemyDied?.Invoke();
        currentEnemy = null;
        enemyDisplay.DestroyEnemy();
        currentEnemys = new();
        EnemyManagerUI.currentEnemys = new();
        TurnManager.currentTurn = Turn.Off;
    }

    public void EndTurn()
    {
        shield = 0;
        enemyDisplay.UpdateShieldDisplay(shield, maxHealth);
    }

    public void Save()
    {
        loadedData.currentEnemyHP = currentHealth;
        loadedData.enemyID = currentEnemy.enemyID;
        loadedData.currentEnemyShield = shield;
    }
}
