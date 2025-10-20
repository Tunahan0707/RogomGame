using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    [Header("Card Lists")]
    public List<string> drawDeck;
    public List<string> hand;
    public List<string> discardPile;
    public List<string> deck;
    private List<string> singleUseDiscardPile;
    private List<string> singleUseDrawDeck;
    private List<string> singleUseHand;

    private FightData loadedData => FightDataHolder.Instance.fightData;
    public int handSize = 5;


    [SerializeField] private CardsDataBase cardsDataBase;

    private void OnEnable()
    {
        EnemyManager.OnEnemyDied += DeckAllCards;
        PlayerManager.OnPlayerDied += DeckAllCards;
        deck = new(loadedData.deck);
        drawDeck = new();
        hand = new();
        discardPile = new();
        singleUseDiscardPile = new(loadedData.discardPile);
        singleUseHand = new(loadedData.hand);
        singleUseDrawDeck = new(loadedData.drawDeck);
        LoadDeck();
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemyDied -= DeckAllCards;
        PlayerManager.OnPlayerDied -= DeckAllCards;
    }

    private void DeckAllCards()
    {
        foreach (string card in hand)
        {
            drawDeck.Add(card);
            CardManagerUI.Instance.UpdateDrawDeckUI(card);
        }
        foreach (string card in discardPile)
        {
            drawDeck.Add(card);
            CardManagerUI.Instance.UpdateDrawDeckUI(card);
        }
        hand.Clear();
        discardPile.Clear();
    }

    private void LoadDeck()
    {
        if (GameStartManager.currentGameType == GameType.ContinueGame)
        {
            CardDisplay.AllCards = new();
            foreach (string card in singleUseDrawDeck)
            {
                var cardData = cardsDataBase.GetCardByID(card);
                if (cardData == null) continue;
                CardManagerUI.Instance.StartDrawDeckUI(cardData);
            }
            foreach (string card in singleUseHand)
            {
                var cardData = cardsDataBase.GetCardByID(card);
                if (cardData == null) continue;
                CardManagerUI.Instance.StartHandUI(cardData);
            }
            foreach (string card in singleUseDiscardPile)
            {
                var cardData = cardsDataBase.GetCardByID(card);
                if (cardData == null) continue;
                CardManagerUI.Instance.StartDiscardPileUI(cardData);
            }
            discardPile.AddRange(CardManagerUI.discardCardIDs);
            hand.AddRange(CardManagerUI.handCardIDs);
            drawDeck.AddRange(CardManagerUI.deckCardIDs);
        }
        else if (GameStartManager.currentGameType == GameType.NewGame)
            InitializeDeck();
    }

    private void InitializeDeck()
    {
            foreach (var card in deck)
            {
                var cardData = cardsDataBase.GetCardByID(card);
                if (cardData == null) continue;
                CardManagerUI.Instance.StartDrawDeckUI(cardData);
            }
        drawDeck.AddRange(CardManagerUI.deckCardIDs);
        ShuffleDeck(drawDeck);
    }

    public IEnumerator DrawCards(int count)
    {
        TurnManager.endTurnButton1.gameObject.SetActive(false);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if (drawDeck.Count == 0)
            {
                yield return StartCoroutine(ShuffleDiscardIntoDeck());
            }

            if (drawDeck.Count == 0) break; // Yoksa çık
            string drawn = drawDeck[0];
            drawDeck.RemoveAt(0);
            hand.Add(drawn);
            CardManagerUI.Instance.UpdateHandUI(drawn);
        }
        TurnManager.endTurnButton1.gameObject.SetActive(true);
    }

    public void DiscardHand()
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
        }
        hand.Clear();
        StartCoroutine(CardManagerUI.Instance.ClearHandUI());
    }

    private IEnumerator ShuffleDiscardIntoDeck()
    {
        
        List<string> tempList = new List<string>(discardPile);

        foreach (var card in tempList)
        {
            TurnManager.endTurnButton1.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            drawDeck.Add(card);
            CardManagerUI.Instance.UpdateDrawDeckUI(card);
            discardPile.Remove(card);
        }
        ShuffleDeck(drawDeck);
    }

    public static void ShuffleDeck(List<string> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }
    }
    public List<string> GetCurrent(string listName)
    {
        List<string> currentList = new();
        List<string> x = new();
        if (listName == Consts.ListNames.DECK)
            currentList = deck;
        else if (listName == Consts.ListNames.DRAW_DECK)
            x = drawDeck;
        else if (listName == Consts.ListNames.HAND)
            x = hand;
        else if (listName == Consts.ListNames.DISCARD_PILE)
            x = discardPile;
        foreach (string card in x)
        {
            if (card == null)
            {
                Debug.LogWarning(card + " Bulunamadı!!");
                continue;
            }
            string cardID = CardDisplay.GetCardID(card);
            if (cardID == null)
            {
                Debug.LogWarning(cardID + " Bulunamadı!!");
                continue;
            }
            currentList.Add(cardID);
        }
        return currentList;
    }
}
