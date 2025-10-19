using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerDied;
    public static int playerHealth;
    public int shield;
    public int maxHealth;
    [SerializeField] private PlayersDataBase playersDataBase;
    [HideInInspector] public static PlayerDisplay playerDisplay;
    [SerializeField] private PlayerManagerUI playerManagerUI;
    private FightData loadedData => FightDataHolder.Instance.fightData;
    private PlayerData playerData => PlayerDataHolder.Instance.playerData;

    private void Awake()
    {
        maxHealth = Mathf.RoundToInt(playerData.maxHP);
        if (loadedData.currentHP == 0)
            playerHealth = maxHealth;
        else
            playerHealth = Mathf.RoundToInt(loadedData.currentHP);
        if (playerData.currentPlayerID != null)
            playerManagerUI.SpawnPlayer(playersDataBase.GetPlayerByID(playerData.currentPlayerID));
        else
            playerManagerUI.SpawnPlayer(playersDataBase.startingPlayer);
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

    private void Die()
    {
        OnPlayerDied?.Invoke();
    }

    private void Heal(int heal)
    {
        playerHealth += heal;
        if (playerHealth > maxHealth)
            playerHealth = maxHealth;
        playerDisplay.UpdateHealthDisplay(playerHealth, maxHealth);
    }
    private void AddShield(int addedShield)
    {
        shield += addedShield;
        playerDisplay.UpdateShieldDisplay(shield, maxHealth);
    }
}
