using UnityEngine;

public enum OzType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Öz", menuName = "ScriptableObjects/Öz")]
public class OzsSo : ScriptableObject
{
    [Header("Öz Info")]
    public string ozName;
    public string description;
    public Sprite artwork;
    [Header("Öz Stats")]
    public int attackMultiplier;
    public int defenseMultiplier;
    public int extraHealth;
    public int healing;
    public int extraHealing;
    public int goldMultiplier;
    public int xpMultiplier;
    public int extraXp;
    public int PUAN_Multiplier;
    public int extraGold;
    public string specialEffect;
}
