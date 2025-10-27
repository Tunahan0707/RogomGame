using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<CardDisplay> OnCardClicked;

    [Header("UI References")]
    [SerializeField] private Image artwork;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button playButton;

    public string cardIDInScene { get; private set; }

    private RectTransform rect;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector2 originalPosition;
    private int originalCost;
    public CardsSO cardData { get; private set; }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnCardPlayed);

        originalScale = rect.localScale;
        originalRotation = rect.localRotation;
        originalPosition = rect.anchoredPosition;
    }

    public void SetCard(CardsSO card)
    {
        string cardDescription = "";

        if (card.effects != null && card.effects.Count > 0)
        {
            foreach (var effect in card.effects)
            {
                string desc = effect.GetDescription();
                if (!string.IsNullOrEmpty(desc))
                    cardDescription += desc + "\n";
            }
        }

        cardData = card;
        artwork.sprite = card.artwork;
        cardNameText.text = card.cardName + " +";
        cardNameText.text = card.cardName;

        // Ana açıklama varsa onu da en üstte göster
        if (!string.IsNullOrEmpty(card.description))
            descriptionText.text = card.description + "\n" + cardDescription;
        else
            descriptionText.text = cardDescription;
        costText.text = card.cost.ToString();

        Color typeColor;
        string typeText;

        switch (card.cardType)
        {
            case CardsSO.CardType.Attack:
                typeColor = new Color(0.55f, 0.1f, 0.1f); // koyu kırmızı
                typeText = "Saldırı";
                break;
            case CardsSO.CardType.Skill:
                typeColor = new Color(0.15f, 0.35f, 0.6f); // lacivert
                typeText = "Beceri";
                break;
            case CardsSO.CardType.Power:
                typeColor = new Color(0.35f, 0.25f, 0.5f); // mor
                typeText = "Güç";
                break;
            case CardsSO.CardType.Defence:
                typeColor = new Color(0.15f, 0.35f, 0.6f);
                typeText = "Savunma";
                break;
            default:
                typeColor = Color.white;
                typeText = "Bilinmiyor";
                break;
        }

        cardTypeText.text = typeText;

        // Kartın ana görsel çerçevesini renklendir
        artwork.transform.parent.parent.GetComponent<Image>().color = typeColor;
        cardNameText.transform.parent.GetComponent<Image>().color = typeColor;

        if (FightDataHolder.Instance.fightData.isNewSave)
            originalCost =  cardData.cost;

        if (string.IsNullOrEmpty(cardIDInScene))
            cardIDInScene = System.Guid.NewGuid().ToString();
        if (cardData.isUpgradedVersion)
            UpgradeCard();
        PlayerManager.OnStatsChanged -= UpdateCardDescription;
        PlayerManager.OnStatsChanged += UpdateCardDescription;
        ManaManager.OnManaSpent -= UpdateCostText;
        ManaManager.OnManaSpent += UpdateCostText;
    }
    private void OnDestroy()
    {
        ManaManager.OnManaSpent -= UpdateCostText;
        PlayerManager.OnStatsChanged -= UpdateCardDescription;
    }

    private void OnCardPlayed()
    {
        if (TurnManager.currentTurn != Turn.Player) return; 

        var currentZone = CardZoneManager.GetZone(cardIDInScene);
        if (currentZone != CardZone.Hand) return; // sadece elden oynanabilir

        OnCardClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.SetAsLastSibling();
        rect.localScale = originalScale * 1.2f;
        rect.localRotation = Quaternion.identity;

        // eğer eldeyse, hover animasyonu el düzenini güncelleyebilir
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

    public string GetCardID()
    {
        return cardIDInScene;
    }

    public void UpdateCardDescription()
    {
        string cardDescription = "";

        foreach (var effect in cardData.effects)
        {
            string desc = effect.GetDescription();
            if (!string.IsNullOrEmpty(desc))
                cardDescription += desc + "\n";
        }

        descriptionText.text = cardData.description + "\n" + cardDescription;
    }
    public void UpdateCostText()
    {
        if (ManaManager.currentMana < cardData.cost)
            costText.color = Color.red;
    }
    public void ChangeCost(bool isNew, int cost)
    {
        if (isNew)
            cardData.cost = cost;
        else
            cardData.cost += cost;
        costText.text = cardData.cost.ToString();

        Color newColor;
        if (ColorUtility.TryParseHtmlString("#00CC66", out newColor))
        {
            costText.color = newColor;
        }
    }
    public void ResetCost()
    {
        cardData.cost = originalCost;
        costText.text = cardData.cost.ToString();
        costText.color = Color.white;
    }

    public void UpgradeCard()
    {
        cardData.isUpgradedVersion = true;
        cardNameText.text = cardData.cardName + " +";
        cardNameText.color = Color.greenYellow;
        string cardDescription = "";
        foreach (var effect in cardData.upgradedEffects)
        {
            string desc = effect.GetDescription();
            if (!string.IsNullOrEmpty(desc))
                cardDescription += desc + "\n";
        }
        descriptionText.text = cardData.description + "\n" + cardDescription;
    }
    public void DeUpgradeCard()
    {
        cardData.isUpgradedVersion = false;
        cardNameText.text = cardData.cardName;
        cardNameText.color = Color.white;
        UpdateCardDescription();
    }
    public void LockImage()
    {
        artwork.color = Color.gray;
        artwork.transform.parent.GetComponent<Image>().color = Color.gray;
        artwork.transform.parent.parent.GetComponent<Image>().color = Color.gray;
        cardNameText.text = "???";
        descriptionText.text = "Bu kart kilitli.";
        playButton.interactable = false;
    }
}
