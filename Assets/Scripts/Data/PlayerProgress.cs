using UnityEngine;
using System;

[System.Serializable]
public class PlayerProgress
{
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 200;

    // 🎯 EVENT: Level up gerçekleştiğinde tetiklenecek
    public static event Action<int> OnPlayerLevelUp;

    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);

            Debug.Log($"🎖 Oyuncu seviye atladı! Yeni seviye: {level}");
            OnPlayerLevelUp?.Invoke(level); // 🔥 event gönder
        }
    }
}
