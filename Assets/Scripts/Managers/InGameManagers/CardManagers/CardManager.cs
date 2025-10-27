using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private List<string> deck;
    [SerializeField] private CardsDataBase cardsDataBase;
    public int handSize { get; private set; } = 5; 

    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        EnemyManager.OnEnemyDied += ReturnAllToDeck;
        PlayerManager.OnPlayerDied += ReturnAllToDeck;
    }

    private void OnDisable()
    {
        EnemyManager.OnEnemyDied -= ReturnAllToDeck;
        PlayerManager.OnPlayerDied -= ReturnAllToDeck;
    }

    private void Start()
    {
        CardZoneManager.Clear();
        deck = new(loadedData.deck);
        LoadDeck();
    }

    private void LoadDeck()
    {
        CardZoneManager.Clear();


        if (!loadedData.isNewSave)
        {
            // sırasıyla zone'lara yükle
            foreach (string id in loadedData.drawDeck)
                SpawnCards(id, CardZone.Draw);
            foreach (string id in loadedData.hand)
                SpawnCards(id, CardZone.Hand);
            foreach (string id in loadedData.discardPile)
                SpawnCards(id, CardZone.Discard);
        }
        else
        {
            foreach (string id in deck)
                SpawnCards(id, CardZone.Draw);
            ShuffleDeck(CardZone.Draw);
        }
    }
    private void SpawnCards(string id, CardZone zone)
    {
        var card = cardsDataBase.GetCardByID(id);
        if (card == null) return;
        CardManagerUI.Instance.SpawnCard(card, zone);
    }

    private void ReturnAllToDeck()
    {
        foreach (var id in new List<string>(CardZoneManager.GetByZone(CardZone.Hand)))
            CardZoneManager.SetZone(id, CardZone.Draw);
        foreach (var id in new List<string>(CardZoneManager.GetByZone(CardZone.Discard)))
            CardZoneManager.SetZone(id, CardZone.Draw);
    }

    public IEnumerator DrawCards(int count)
    {

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.15f);

            var deck = new List<string>(CardZoneManager.GetByZone(CardZone.Draw));
            if (deck.Count == 0)
            {
                List<string> discard = new(CardZoneManager.GetByZone(CardZone.Discard));
                foreach (string id in discard)
                    CardZoneManager.SetZone(id, CardZone.Draw);
                ShuffleDeck(CardZone.Draw);
                deck = new List<string>(CardZoneManager.GetByZone(CardZone.Draw));
            }

            if (deck.Count == 0) break;

            string drawn = deck[0];
            CardZoneManager.SetZone(drawn, CardZone.Hand);
            CardManagerUI.Instance.MoveCard(drawn, CardZone.Hand);
        }

    }

    public IEnumerator DiscardHand()
    {
        foreach (var id in new List<string>(CardZoneManager.GetByZone(CardZone.Hand)))
        {
            CardZoneManager.SetZone(id, CardZone.Discard);
            CardManagerUI.Instance.MoveCard(id, CardZone.Discard);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public static void ShuffleDeck(CardZone zone)
    {
        var ids = new List<string>(CardZoneManager.GetByZone(zone));
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (ids[i], ids[randomIndex]) = (ids[randomIndex], ids[i]);
        }
        CardZoneManager.ClearZone(zone);
        foreach (string id in ids)
            CardZoneManager.SetZone(id, zone);
    }
    public void Save()
    {
        loadedData.deck = new(deck);

        loadedData.drawDeck.Clear();
        loadedData.hand.Clear();
        loadedData.discardPile.Clear();

        foreach (var id in CardZoneManager.GetByZone(CardZone.Draw))
            loadedData.drawDeck.Add(CardManagerUI.GetCardID(id));
        foreach (var id in CardZoneManager.GetByZone(CardZone.Hand))
            loadedData.hand.Add(CardManagerUI.GetCardID(id));
        foreach (var id in CardZoneManager.GetByZone(CardZone.Discard))
            loadedData.discardPile.Add(CardManagerUI.GetCardID(id));


        SaveManager.Save(loadedData, Consts.FileNames.FightDataFile);
        Debug.Log("💾 FightData kaydedildi.");
    }

}
