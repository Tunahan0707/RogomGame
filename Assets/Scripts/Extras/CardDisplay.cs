using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<CardDisplay> OnCardClicked;
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
    private Quaternion originalRotation;
    private Vector2 originalPosition;
    private RectTransform rect;

    public CardsSO cardData { get; private set; }
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
        Color typeColor = Color.white;
        string text = "Saldırı";
        switch (card.cardType)
{
    case CardsSO.CardType.Attack:
        typeColor = new Color(0.55f, 0.1f, 0.1f); // koyu kırmızı (bordo ton)
        text = "Saldırı";
        break;

    case CardsSO.CardType.Skill:
        typeColor = new Color(0.15f, 0.35f, 0.6f); // koyu mavi-lacivert ton
        text = "Beceri";
        break;

    case CardsSO.CardType.Power:
        typeColor = new Color(0.35f, 0.25f, 0.5f); // koyu mor ton
        text = "Güç";
        break;

    case CardsSO.CardType.Defence:
        typeColor = new Color(0.15f, 0.35f, 0.6f); // koyu mavi-lacivert ton
        text = "Beceri";
        break;
}

        artwork.transform.parent.GetComponent<Image>().color = typeColor;
        costText.text = card.cost.ToString();
        cardTypeText.text = text;
        attackValueText.text = card.attackValue.ToString();
        attackValueText.gameObject.SetActive(card.attackValue != 0);
        if (string.IsNullOrEmpty(inDeckCardIDText))
            inDeckCardIDText = System.Guid.NewGuid().ToString();
        if (!AllCards.ContainsKey(inDeckCardIDText))
            AllCards.Add(inDeckCardIDText, this);
    }

    private void OnCardPlayed()
    {
        if (TurnManager.currentTurn != TurnManager.Turn.Player) return;
        if (cardManager == null)
        {
            OnCardClicked?.Invoke(this);
            return; 
        }
        else if (cardManager.hand.Contains(inDeckCardIDText))
        {
            OnCardClicked?.Invoke(this);
        }
        else
        {
            Debug.Log("Kart oynanamadı");
        }
    }

    public void PlayCard()
    {
        cardManager.hand.Remove(inDeckCardIDText);
        cardManager.discardPile.Add(inDeckCardIDText);
        CardManagerUI.Instance.CardPlayed(gameObject);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.SetAsLastSibling();
        rect.localScale = originalScale * 1.2f;
        rect.localRotation = Quaternion.identity;

        HandLayout layout = transform.parent.GetComponent<HandLayout>();
        if (layout != null)
            layout.UpdateHandLayout(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localScale = originalScale;
        rect.localRotation = originalRotation;

        HandLayout layout = transform.parent.GetComponent<HandLayout>();
        if (layout != null)
            layout.UpdateHandLayout();
    }

    public static string GetCardID(string inDeckCardIDText)
    {
        if (AllCards.TryGetValue(inDeckCardIDText, out var card))
            return card.cardData.cardID;

        Debug.LogWarning($"Card with ID {inDeckCardIDText} not found.");
        return null;
    }
}
