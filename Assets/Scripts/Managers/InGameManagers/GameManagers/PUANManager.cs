using System;
using UnityEngine;

public class PUANManager : MonoBehaviour
{
    public static event Action<int> OnPUANChanged;
    public static PUANManager Instance;

    public static int puan { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        if (PlayerDataHolder.Instance != null && PlayerDataHolder.Instance.playerData != null)
            puan = PlayerDataHolder.Instance.playerData.PUAN;
        else
            puan = 0;

        OnPUANChanged?.Invoke(puan);
    }

    public void AddPUAN(int amount)
    {
        puan += amount;
        OnPUANChanged?.Invoke(puan);
        // Gerekirse PlayerDataHolder'a yazıp kaydet
    }

    public bool SpendPUAN(int amount)
    {
        if (puan >= amount)
        {
            puan -= amount;
            OnPUANChanged?.Invoke(puan);
            return true;
        }
        Debug.Log("Not enough PUAN!");
        return false;
    }
}
