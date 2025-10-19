using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class CardDisplay : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public event Action<CardsSO> OnCardPlayedEvent;
    [SerializeField] private Image artwork;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text attackValueText;
    [SerializeField] private TMP_Text costText;

    [HideInInspector]
    public static Dictionary<string, CardDisplay> AllCards = new();
    public string inDeckCardIDText;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Quaternion originalRotation;
    private RectTransform rect;

    private CardsSO cardData;
    private CardManager cardManager;
    private TurnManager turnManager;

    private void Awake()
    {
        cardManager = FindObjectsByType<CardManager>(FindObjectsSortMode.None)[0];
        turnManager = FindObjectsByType<TurnManager>(FindObjectsSortMode.None)[0];

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnCardPlayed);
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
        originalRotation = rect.localRotation;
        originalPosition = rect.anchoredPosition;
    }

    private void OnDestroy()
    {
        if (AllCards.ContainsKey(inDeckCardIDText))
            AllCards.Remove(inDeckCardIDText);
    }

    public void SetCard(CardsSO card)
    {
        cardData = card;
        artwork.sprite = card.artwork;
        cardNameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = card.cost.ToString();
        cardTypeText.text = card.cardType.ToString();
        attackValueText.text = card.attackValue.ToString();
        if (string.IsNullOrEmpty(inDeckCardIDText))
            inDeckCardIDText = System.Guid.NewGuid().ToString();
        if (!AllCards.ContainsKey(inDeckCardIDText))
            AllCards.Add(inDeckCardIDText, this);
    }

    private void OnCardPlayed()
    {
        if (TurnManager.currentTurn != TurnManager.Turn.Player) return;

        if (cardManager.hand.Contains(inDeckCardIDText))
        {
            cardManager.hand.Remove(inDeckCardIDText);
            cardManager.discardPile.Add(inDeckCardIDText);
            CardManagerUI.Instance.CardPlayed(gameObject);
            OnCardPlayedEvent?.Invoke(cardData);
        }
    }

    public string GetCardID()
    {
        return inDeckCardIDText;
    }
    public GameObject GetGameObject(string inDeckCardIDText)
    {
        if (AllCards.TryGetValue(inDeckCardIDText, out var card))
            return card.gameObject;

        Debug.LogWarning($"Card with ID {inDeckCardIDText} not found.");
        return null;
    }

    public static CardDisplay GetCardDisplay(string id)
    {
        if (AllCards.TryGetValue(id, out var card))
            return card;
        return null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localScale = originalScale;
        rect.anchoredPosition = originalPosition;
        HandLayout handLayout = transform.parent.GetComponent<HandLayout>();
        if (handLayout != null)
            handLayout.UpdateHandLayout();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.SetAsLastSibling();
        rect.localScale = originalScale * 1.2f;
        rect.localRotation = Quaternion.Euler(0, 0, 0);
        rect.anchoredPosition += new Vector2(0, 30f);
        HandLayout handLayout = transform.parent.GetComponent<HandLayout>();
        if (handLayout != null)
            handLayout.UpdateHandLayout();
    }
    public static string GetCardID(string inDeckCardIDText)
    {
        if (AllCards.TryGetValue(inDeckCardIDText, out var card))
            return card.cardData.cardID;

        Debug.LogWarning($"Card with ID {inDeckCardIDText} not found.");
        return null;
    }
}
