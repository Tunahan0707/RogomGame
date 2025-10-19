using System;
using UnityEngine;

public class PUANManager : MonoBehaviour
{
    public static event Action<int> OnPUANChanged;

    [SerializeField] private int puan = 0;
    public int PUAN => puan;

    public void AddPUAN(int amount)
    {
        puan += amount;
        OnPUANChanged?.Invoke(puan);
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
