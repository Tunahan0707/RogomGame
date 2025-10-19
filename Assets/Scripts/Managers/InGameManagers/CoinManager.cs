using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static event Action<int> OnCoinsChanged;

    [field: SerializeField]
    public static int Coins { get; private set; } = 0;

    private void Awake()
    {
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }
        Debug.Log("Not enough coins!");
        return false;
    }
    public static int GetCurrentCoins()
    {
        return Coins;
    }
}
