using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static event Action<int> OnCoinsChanged;
    public static CoinManager Instance { get; private set; }

    [field: SerializeField]
    public static int Coins { get; private set; } = 0;
    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        OnCoinsChanged?.Invoke(Coins);
        Coins = loadedData.coin;
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
}
