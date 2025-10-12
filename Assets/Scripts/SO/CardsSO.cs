using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card")]
public class CardsSO : ScriptableObject
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
    public string cardID;
    public int attackValue;

    public void DoCardTypeAction()
    {
        if (cardType == CardType.Attack)
        {
            // Do something specific for Attack cards
        }
        else if (cardType == CardType.Skill)
        {
            // Do something specific for Skill cards
        }
        else if (cardType == CardType.Power)
        {
            // Do something specific for Power cards
        }
        else if (cardType == CardType.Defence)
        {
            // Do something specific for Defence cards
        }
    }
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
