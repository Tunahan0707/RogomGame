using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Öz", menuName = "ScriptableObjects/Öz")]
public class OzsSO : ScriptableObject, IUnlockable
{
    [Header("Öz Info")]
    public string ozName;
    public string description;
    public Sprite artwork;
    public OzType ozType;
    public bool isLocked;
    public int unlockLevel;
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
    [HideInInspector] public string ozsID;
    
    [Header("Öz Stats")]
    public int attackMultiplier = 1;
    public int defenseMultiplier = 1;
    public int extraHealth = 0;
    public int healing = 0;
    public int extraHealing = 0;
    public int goldMultiplier = 1;
    public int xpMultiplier = 1;
    public int extraXp = 0;
    public int PUAN_Multiplier = 1;
    public int extraGold = 0;
    public List<string> specialEffects = new();

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ozsID))
        {
            ozsID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
