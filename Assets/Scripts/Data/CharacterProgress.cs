using System;
using UnityEngine;

[System.Serializable]
public class CharacterProgress
{
    public static event Action<string, int> OnCharacterLevelUp;
    public string characterID;
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    public CharacterProgress(string id)
    {
        characterID = id;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
            Debug.Log($"🧙 {characterID} seviye atladı! Yeni seviye: {level}");
            OnCharacterLevelUp?.Invoke(characterID, level); // 🔥 event gönder
        }
    }
}
