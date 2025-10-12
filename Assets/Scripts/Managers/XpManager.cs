using UnityEngine;
using System;

public class XPManager : MonoBehaviour
{
    public static event Action<int> OnXPChanged;
    public static event Action OnPlayerLevelUp;

    [SerializeField] private int currentXP = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private int xpToNextLevel = 100;

    public int CurrentXP => currentXP;
    public int Level => level;

    public void AddXP(int amount)
    {
        currentXP += amount;
        OnXPChanged?.Invoke(currentXP);

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f); // Seviye başına XP artışı
        Debug.Log($"Level Up! New Level: {level}");
        OnPlayerLevelUp?.Invoke();
        OnXPChanged?.Invoke(currentXP);
    }
}
