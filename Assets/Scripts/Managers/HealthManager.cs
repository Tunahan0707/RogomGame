using UnityEngine;
using System;

public class HealthManager : MonoBehaviour
{
    public static event Action<int, int> OnHealthChanged;
    public static event Action OnPlayerDied;

    [field: SerializeField]
    public static int CurrentHealth { get; private set; } = 100;
    [field: SerializeField]
    public static int MaxHealth { get; private set; }

    private void Awake()
    {
        MaxHealth = (int)PlayerDataHolder.Instance.playerData.maxHP;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
    private void OnEnable()
    {
        CurrentHealth = (int)FightDataHolder.Instance.fightData.currentHP;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void Die()
    {
        OnPlayerDied?.Invoke();
    }
    public static float GetHealth()
    {
        return (float)CurrentHealth;
    }
}
