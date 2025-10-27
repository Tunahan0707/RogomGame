using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsDataBase", menuName = "ScriptableObjects/Databases/CardsDataBase", order = 0)]
public class CardsDataBase : ScriptableObject
{
    [Header("Card Data")]
    public List<CardsSO> cards;
    public List<CardsSO> startingCards;
    public List<CardsSO> normalCards;
    public List<CardsSO> rareCards;
    public List<CardsSO> epicCards;
    public List<CardsSO> legendaryCards;
    public List<CardsSO> bossCards;

    private void OnEnable()
    {
        startingCards = new();
        normalCards = new();
        rareCards = new();
        epicCards = new();
        legendaryCards = new();
        bossCards = new();
        cards = new List<CardsSO>(Resources.LoadAll<CardsSO>(Consts.FileWays.CardsSO));
        foreach (var card in cards)
        {
            switch (card.rarity)
            {
                case CardRarelitys.Start: startingCards.Add(card); break;
                case CardRarelitys.Normal: normalCards.Add(card); break;
                case CardRarelitys.Rare: rareCards.Add(card); break;
                case CardRarelitys.Epic: epicCards.Add(card); break;
                case CardRarelitys.Legendary: legendaryCards.Add(card); break;
                case CardRarelitys.Boss: bossCards.Add(card); break;
            }
        }

    }

    public List<CardsSO> GetCardsByRarelity(CardRarelitys rarelity)
    {
        return rarelity switch
        {
            CardRarelitys.Start => startingCards,
            CardRarelitys.Normal => normalCards,
            CardRarelitys.Rare => rareCards,
            CardRarelitys.Epic => epicCards,
            CardRarelitys.Legendary => legendaryCards,
            CardRarelitys.Boss => bossCards,
            _ => new List<CardsSO>(),
        };
    }
    public CardsSO GetCardByID(string cardID)
    {
        return cards.Find(card => card.cardID == cardID);
    }

    public List<CardsSO> UnlockLevel(int lvl)
    {
        return cards.FindAll(card => card.unlockLevel == lvl);
    }
}