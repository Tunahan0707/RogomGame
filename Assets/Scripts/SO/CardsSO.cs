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
    [Header("Card description")]
    public string cardName = "Type any name...";
    [TextArea] public string description = "Type any description...";
    public Sprite artwork;

    [Header("Card attributes")]
    public CardType cardType;
    public CardRarelitys cardRarelity;
    public int cost = 0;
    public int howManyHaveOnStart = 0;
    [HideInInspector] public string cardID;
    public int attackValue = 0;
    public int unlockLevel = 0;
    public int shieldValue = 0;
    public int healValue = 0;
    public int addingResistnace = 0;
    public int addingStrenght = 0;
    public int debuffingEnemysStrenght = 0;
    public int debuffingEnemysResistance = 0;
    public bool isLocked = false;

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
