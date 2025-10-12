using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static Action OnDeckCompleted;

    [Header("Card Lists")]
    public List<string> drawDeck;
    public List<string> hand;
    public List<string> discardPile;
    public List<string> deck;
    private List<string> singleUseDrawDeck;
    private List<string> singleUseDiscardPile;
    private List<string> singleUseHand;

    [SerializeField] private CardDisplay cardDisplay;
    FightData loadedData => FightDataHolder.Instance.fightData;
    public int handSize = 5;


    [SerializeField] private CardsDataBase cardsDataBase;


    private void OnEnable()
    {
        deck = new(loadedData.deck);
        drawDeck = new();
        hand = new();
        discardPile = new();
        
        LoadPlayerDeck();
    }

    private void LoadPlayerDeck()
    {
        
    }

    private void InitializeDeck()
    {

        foreach (var id in deck)
        {
            var card = cardsDataBase.GetCardByID(id);
            if (card == null) continue;
            CardManagerUI.Instance.StartDrawDeckUI(card);
        }
        drawDeck.AddRange(CardManagerUI.deckCardIDs);
        ShuffleDeck(drawDeck);
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawDeck.Count == 0)
            {
                ShuffleDiscardIntoDeck();
            }

            if (drawDeck.Count == 0) break; // Yoksa çık
            string drawn = drawDeck[0];
            drawDeck.RemoveAt(0);
            hand.Add(drawn);
            CardManagerUI.Instance.UpdateHandUI(drawn);
        }
    }

    public void DiscardHand()
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
        }
        hand.Clear();
        CardManagerUI.Instance.ClearHandUI();
    }

    private void ShuffleDiscardIntoDeck()
    {
        foreach (var card in discardPile)
        {
            drawDeck.Add(card);
            CardManagerUI.Instance.UpdateDrawDeckUI(card);
        }
        discardPile.Clear();
        ShuffleDeck(drawDeck);
    }

    private void ShuffleDeck(List<string> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, deck.Count);
            var temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }
    public List<string> GetCurrent(string listName)
    {
        List<string> x = new();
        List<string> currentList = null;
        if (listName == Consts.ListNames.DRAW_DECK)
        {
            x = drawDeck;
        }
        else if (listName == Consts.ListNames.DISCARD_PILE)
        {
            x = discardPile;
        }
        else if (listName == Consts.ListNames.DECK)
        {
            return deck;
        }
        else if (listName == Consts.ListNames.HAND)
        {
            x = hand;
        }
        else
        {
            Debug.LogWarning($"List name {listName} is not recognized.");
        }
        foreach (var cardDeckID in x)
        {
            if (cardsDataBase.GetCardByID(cardDeckID) == null)
            {
                Debug.LogWarning($"Card with ID {cardDeckID} not found in CardsDataBase.");
                continue;
            }
            string cardID = cardDisplay.GetCardID(cardDeckID);
            if (cardID == null)
            {
                Debug.LogWarning($"CardDisplay for ID {cardDeckID} not found.");
                continue;
            }
            currentList.Add(cardID);
        }
        return currentList;
    }
}
