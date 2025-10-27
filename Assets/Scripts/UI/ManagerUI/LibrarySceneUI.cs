using System;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LibrarySceneUI : MonoBehaviour
{
    public static LibrarySceneUI Instance { get; private set; }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform startCardContainer;
    [SerializeField] private Transform normalCardContainer;
    [SerializeField] private Transform rareCardContainer;
    [SerializeField] private Transform epicCardContainer;
    [SerializeField] private Transform legendaryCardContainer;
    [SerializeField] private Transform bossCardContainer;
    [SerializeField] private Button backButton;
    [SerializeField] private Button blackGround;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private Sprite checkedUpgradeSprite;
    [SerializeField] private Sprite uncheckedUpgradeSprite;
    private CardDisplay currentCardDisplay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        blackGround.onClick.AddListener(OnBlackGroundClicked);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        backButton.onClick.AddListener(() => SceneManager.LoadScene(Consts.Scenes.MAIN_MENU));
    }

    private void OnEnable()
    {
        CardDisplay.OnCardClicked += SummonCardDetailPanel;
    }
    private void OnDisable()
    {
        CardDisplay.OnCardClicked -= SummonCardDetailPanel;
    }

    private void Start()
    {
        blackGround.gameObject.SetActive(false);
    }

    public void SpawnCard(CardsSO cardData)
    {
        Transform cardContainer = cardData.rarity switch
        {
            CardRarelitys.Start => startCardContainer,
            CardRarelitys.Normal => normalCardContainer,
            CardRarelitys.Rare => rareCardContainer,
            CardRarelitys.Epic => epicCardContainer,
            CardRarelitys.Legendary => legendaryCardContainer,
            CardRarelitys.Boss => bossCardContainer,
            _ => normalCardContainer,
        };
        GameObject cardGO = Instantiate(cardPrefab, cardContainer);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardData);
        if (cardData.isLocked)
            cardDisplay.LockImage();
    }

    private void OnUpgradeButtonClicked()
    {
        currentCardDisplay?.UpgradeCard();
        Image upgradeButtonImage = upgradeButton.gameObject.GetComponent<Image>();
        if (currentCardDisplay != null && currentCardDisplay.cardData.isUpgradedVersion)
        {
            upgradeButtonImage.sprite = checkedUpgradeSprite;
        }
        else
        {
            upgradeButtonImage.sprite = uncheckedUpgradeSprite;
        }
    }

    private void SummonCardDetailPanel(CardDisplay display)
    {
        blackGround.gameObject.SetActive(true);
        GameObject cardGO = Instantiate(cardPrefab, cardSpawnPoint);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        cardDisplay.SetCard(display.cardData);
        currentCardDisplay = cardDisplay;
    }
    private void OnBlackGroundClicked()
    {
        if (currentCardDisplay != null)
            Destroy(currentCardDisplay.gameObject);
        currentCardDisplay = null;
        blackGround.gameObject.SetActive(false);
    }
}
