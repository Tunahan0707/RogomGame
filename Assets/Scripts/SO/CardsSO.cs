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

#if UNITY_EDITOR
    [CustomEditor(typeof(CardsSO))]
    public class CardsSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var card = (CardsSO)target;

            card.cardName = EditorGUILayout.TextField("Card Name", card.cardName);
            card.description = EditorGUILayout.TextField("Description", card.description);
            card.artwork = (Sprite)EditorGUILayout.ObjectField("Artwork", card.artwork, typeof(Sprite), false);
            card.cardType = (CardType)EditorGUILayout.EnumPopup("Card Type", card.cardType);
            card.cost = EditorGUILayout.IntField("Cost", card.cost);

            if (card.cardType == CardType.Attack)
            {
                card.attackValue = EditorGUILayout.IntField("Attack Value", card.attackValue);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(card);
            }
        }
    }
#endif

    [HideInInspector]
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
}
