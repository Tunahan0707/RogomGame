using UnityEngine;
using System.Collections.Generic;
using System;

public class CardManagerUI : MonoBehaviour
{
    public static CardManagerUI Instance;

    [Header("UI Parents")]
    [SerializeField] private Transform drawDeckParent;
    [SerializeField] private Transform handParent;
    [SerializeField] private Transform discardParent;

    [Header("Prefabs")]
    [SerializeField] private GameObject cardPrefab;

    private HandLayout handLayout => HandLayout.Instance;

    public static Dictionary<string, CardDisplay> cards = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnCard(CardsSO card, CardZone zone)
    {
        GameObject go = Instantiate(cardPrefab, GetParent(zone));
        var display = go.GetComponent<CardDisplay>();
        display.SetCard(card);
        cards[display.GetCardID()] = display;
        go.SetActive(zone == CardZone.Hand);
        CardZoneManager.SetZone(display.GetCardID(), zone);
        if (zone == CardZone.Hand)
            handLayout.AddCard(go);
    }

    public void MoveCard(string id, CardZone zone)
    {
        if (!cards.ContainsKey(id)) return;
        var card = cards[id];
        card.transform.SetParent(GetParent(zone), false);
        card.gameObject.SetActive(zone == CardZone.Hand);
        if (zone == CardZone.Hand)
            handLayout.AddCard(card.gameObject);
        else
            handLayout.RemoveCard(card.gameObject);
    }

    private Transform GetParent(CardZone zone) =>
        zone switch
        {
            CardZone.Draw => drawDeckParent,
            CardZone.Hand => handParent,
            CardZone.Discard => discardParent,
            _ => drawDeckParent
        };
    
    public static string GetCardID(string id)
    {
        CardDisplay display = cards[id];
        return display.cardData.cardID;
    }

    public bool TryGetCardDisplay(string id, out CardDisplay display)
    {
        return cards.TryGetValue(id, out display);
    }
}
