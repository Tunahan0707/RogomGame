using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card")]
public class CardsSO : ScriptableObject, IGameObject
{
    public enum CardType
    {
        Attack,
        Skill,
        Power,
        Defence

    }
    public string cardName;
    public string description;
    public Sprite artwork;
    public CardType cardType;
    public CardRarelitys cardRarelity;
    public int cost;
    public int howManyHaveOnStart;
    [HideInInspector] public string cardID;
    public int attackValue;
    public bool isLocked;
    public int unlockLevel;

    public string Name { get => cardName; set => cardName = value; }
    public int UnlockLevel { get => unlockLevel; set => unlockLevel = value; }
    public string ID { get => cardID; set => cardID = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(cardID))
        {
            cardID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
