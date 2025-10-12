using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CardManagerUI : MonoBehaviour
{
    public static CardManagerUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform handParent;
    [SerializeField] private Transform drawDeckParent;
    [SerializeField] private Transform discardPileParent;

    [SerializeField] private CardManager cardManager;
    [SerializeField] private List<GameObject> currentHandUIs = new();
    public static HashSet<string> deckCardIDs = new();
    [SerializeField] private List<GameObject> currentDiscardUIs = new();
    [SerializeField] private HandLayout handLayout;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Sahnedeki CardManager referansını al
        cardManager = FindObjectsByType<CardManager>(FindObjectsSortMode.None)[0];
    }

    public void StartDrawDeckUI(CardsSO card)
    {
        GameObject cardGO = Instantiate(cardPrefab);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        cardDisplay.SetCard(card);
        cardGO.transform.SetParent(drawDeckParent, false);
        cardGO.SetActive(false);
        deckCardIDs.Add(cardDisplay.GetCardID());
    }
    
    public void UpdateDrawDeckUI(string cardID)
    {
        if (!deckCardIDs.Contains(cardID))
        {
            Debug.LogWarning($"Card ID {cardID} not found in deckCardIDs.");
            return;
        }

        var cardDisplay = CardDisplay.GetCardDisplay(cardID);
        if (cardDisplay == null)
        {
            Debug.LogWarning($"Card with ID {cardID} not found in AllCards.");
            return;
        }

        var cardGO = cardDisplay.gameObject;
        cardGO.transform.SetParent(drawDeckParent, false);
        cardGO.SetActive(false);
    }

    public void UpdateHandUI(string card)
    {
        if (deckCardIDs.Count == 0)
        {
            Debug.LogWarning("No cards in draw deck UI to draw from.");
            return;
        }
        CardDisplay cardDisplay = CardDisplay.GetCardDisplay(card);
        if (cardDisplay == null)
        {
            Debug.LogWarning($"Card with ID {card} not found.");
            return;
        }
        GameObject cardGO = cardDisplay.GetGameObject(card);
        cardGO.transform.SetParent(handParent, false);
        cardGO.SetActive(true);
        currentHandUIs.Add(cardGO);
        handLayout.AddCard(cardGO);
    }
    public void ClearHandUI()
    {
        foreach (var go in currentHandUIs)
        {
            go.SetActive(false);
            go.transform.SetParent(discardPileParent);
            currentDiscardUIs.Add(go);
        }
        currentHandUIs.Clear();
        handLayout.ClearHand();
    }
    public void CardPlayed(GameObject card)
    {
        card.transform.SetParent(discardPileParent);
        currentDiscardUIs.Add(card);
        currentHandUIs.Remove(card);
        card.SetActive(false);
        handLayout.RemoveCard(card);
    }
}
