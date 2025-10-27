using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public static event Action OnPlayerDied;
    public static event Action OnStatsChanged;
    public int currentHealth { get; private set; }
    public int shield { get; private set; }
    public int strenght { get; private set; }
    public int resistance { get; private set; }
    public int maxHealth { get; private set; }

    [SerializeField] private PlayersDataBase playersDataBase;
    [HideInInspector] public static PlayerDisplay playerDisplay;
    [SerializeField] private PlayerManagerUI playerManagerUI;
    private FightData loadedData => FightDataHolder.Instance.fightData;
    private PlayerData playerData => PlayerDataHolder.Instance.playerData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (playerData.currentPlayerID != null)
            playerManagerUI.SpawnPlayer(playersDataBase.GetPlayerByID(playerData.currentPlayerID));
        else
            playerManagerUI.SpawnPlayer(playersDataBase.startingPlayer);
        maxHealth = Mathf.RoundToInt(playerDisplay.playerData.health + playerData.extraHP);
        if (loadedData.isNewSave)
        {
            currentHealth = maxHealth;
            strenght = playerDisplay.playerData.baseStrenght;
            shield = playerDisplay.playerData.baseShield;
            resistance = playerDisplay.playerData.baseResistance;
        }
        else
        {
            currentHealth = Mathf.RoundToInt(loadedData.currentHP);
            strenght = loadedData.playersStrenght;
            resistance = loadedData.playersResistance;
            shield = loadedData.currentPlayerShield;
        }
        playerDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
        playerDisplay.UpdateShieldDisplay(shield, maxHealth);
        SetEffects();
    }

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
        playerDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }

    private void SetEffects()
    {
        playerDisplay.SetEffects(0, resistance);
        playerDisplay.SetEffects(1, strenght);
        OnStatsChanged?.Invoke();
    }

    private void Die()
    {
        OnPlayerDied?.Invoke();
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        playerDisplay.UpdateHealthDisplay(currentHealth, maxHealth);
    }
    public void AddShield(int addedShield)
    {
        shield += addedShield + resistance;
        playerDisplay.UpdateShieldDisplay(shield, maxHealth);
    }
    public void DebuffStrenght(int debuff)
    {
        strenght -= debuff;
        SetEffects();
    }
    public void DebuffResistance(int debuff)
    {
        resistance -= debuff;
        SetEffects();
    }
    public void BuffStrenght(int buff)
    {
        strenght += buff;
        SetEffects();
    }
    public void BuffResistance(int buff)
    {
        resistance += buff;
        SetEffects();
    }
    public void NextTurn()
    {
        if (strenght > 0)
            strenght -= 1;
        if (resistance > 0)
            resistance -= 1;
        if (strenght < 0)
            strenght += 1;
        if (resistance < 0)
            resistance += 1;
        shield = 0;
        playerDisplay.UpdateShieldDisplay(shield, maxHealth);
        SetEffects();
    }
    public void Save()
    {
        loadedData.playersStrenght = strenght;
        loadedData.playersResistance = resistance;
        loadedData.currentHP = currentHealth;
        loadedData.currentPlayerShield = shield;
    }
}
