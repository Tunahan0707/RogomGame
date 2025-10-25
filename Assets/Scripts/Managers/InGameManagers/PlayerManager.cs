using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public static event Action OnPlayerDied;
    public int playerHealth { get; private set; }
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
        maxHealth = Mathf.RoundToInt(playerData.maxHP + playerData.extraHP);
        if (loadedData.isNewSave)
        {
            playerHealth = maxHealth;
            strenght = playerDisplay.playerData.baseStrenght;
            shield = playerDisplay.playerData.baseShield;
            resistance = playerDisplay.playerData.baseResistance;
            
        }
        else
        {
            playerHealth = Mathf.RoundToInt(loadedData.currentHP);
            strenght = loadedData.playersStrenght;
            resistance = loadedData.playersResistance;
            shield = loadedData.currentPlayerShield;
        }
        if (playerData.currentPlayerID != null)
            playerManagerUI.SpawnPlayer(playersDataBase.GetPlayerByID(playerData.currentPlayerID));
        else
            playerManagerUI.SpawnPlayer(playersDataBase.startingPlayer);
        SetEffects();
    }

    public void TakeDamage(int amount)
    {
        if (shield > 0)
        {
            shield -= amount;
            if (shield <= 0)
            {
                playerHealth += shield;
                shield = 0;
            }
        }
        else
        {
            playerHealth -= amount;
            playerHealth = Mathf.Max(playerHealth, 0);
        }
        if (playerHealth <= 0)
        {
            Die();
        }
        playerDisplay.UpdateHealthDisplay(playerHealth, maxHealth);
    }

    private void SetEffects()
    {
        playerDisplay.SetEffects(0, resistance);
        playerDisplay.SetEffects(1, strenght);
    }

    private void Die()
    {
        OnPlayerDied?.Invoke();
    }

    public void Heal(int heal)
    {
        playerHealth += heal;
        if (playerHealth > maxHealth)
            playerHealth = maxHealth;
        playerDisplay.UpdateHealthDisplay(playerHealth, maxHealth);
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
        loadedData.currentHP = playerHealth;
        loadedData.currentPlayerShield = shield;
    }
}
