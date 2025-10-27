using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "ScriptableObjects/Player")]
public class PlayersSO : ScriptableObject, IUnlockable
{
    public string playersName;
    public Sprite playerSprite;
    public int health = 50;
    public int inStartCoins = 0;
    public int baseShield = 0;
    public int baseStrenght = 0;
    public int baseResistance = 0;
    public int maxMana = 3;
    public SkillMapSO skillMap;
    public bool isLocked;
    public int unlockLevel;
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
    [HideInInspector] public string playerID;
    public List<CardsSO> extraStartingCards = new();

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
