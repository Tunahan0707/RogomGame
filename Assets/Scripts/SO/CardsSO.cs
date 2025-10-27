
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card")]
public class CardsSO : ScriptableObject, IUnlockable
{
    public enum CardType { Attack, Skill, Power, Defence }

    [Header("Base Info")]
    public string cardName;
    [TextArea] public string description;
    public Sprite artwork;
    public CardType cardType;
    public CardRarelitys rarity;
    public int cost;
    public int unlockLevel;
    public int howManyHaveOnStart;
    public bool isLocked;
    public bool isUpgradedVersion;
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
    [HideInInspector] public string cardID;

    [Header("Effects")]
    public List<CardEffect> effects = new();
    public List<CardEffect> upgradedEffects = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(cardID))
        {
            cardID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
