using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class PlayerDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<PlayersSO> OnPlayerSelected;

    [Header("UI References")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI playerShieldText;
    [SerializeField] public Image playerSprite;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerShieldBar;
    [SerializeField] private Image playerShieldBar2;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Button button;

    public static Dictionary<string, PlayerDisplay> AllPlayers = new();

    private PlayersSO playerData;

    private void Awake()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        if (button != null)
            button.onClick.AddListener(OnClick);
        else
            Debug.LogWarning("PlayerDisplay: Button not assigned.", this);

        if (playerNameText != null)
            playerNameText.gameObject.SetActive(false);
        else
            Debug.LogWarning("PlayerDisplay: TextMeshProUGUI not assigned or found.", this);
    }

    private void OnClick()
    {
        if (!playerData.isLocked)
        {
            OnPlayerSelected?.Invoke(playerData);
        }
            
    }

    public void SetPlayer(PlayersSO player)
    {
        playerData = player;
        if (playerData == null)
            return;
        playerNameText.text = playerData.playersName;
        playerSprite.sprite = playerData.playerSprite;
        if (playerData.isLocked)
        {
            playerSprite.color = Color.black;
            playerNameText.text = "Bu Karakter Kilitli";
        }
            
        if (!AllPlayers.ContainsKey(playerData.playerID))
            AllPlayers.Add(playerData.playerID, this);
        if (playerManager != null)
        {
            playerShieldBar2.fillAmount = 0f;
            playerShieldBar.fillAmount = 0f;
            UpdateShieldDisplay(playerManager.shield, playerManager.maxHealth);
            UpdateHealthDisplay(PlayerManager.playerHealth, playerManager.maxHealth);
        }
    }
            
    public void DestroyOthers()
    {
        Destroy(healthBar);
    }
    public static PlayerDisplay GetPlayerDisplay(string id)
    {
        if (AllPlayers.TryGetValue(id, out var player))
            return player;
        return null;
    }
    public void UpdateShieldDisplay(int currentShield, int maxHealth)
    {
        float x = (float)currentShield / maxHealth;
        float y;
        playerShieldText.text = currentShield.ToString();
        if (x >= 1)
        {
            y = x - 1;
            playerShieldBar2.fillAmount = y;
            playerShieldBar.fillAmount = 1;
        }
        else
            playerShieldBar.fillAmount = x;
        
    }

    public void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        playerHealthText.text = currentHealth.ToString();
        playerHealthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerNameText != null)
            playerNameText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerNameText != null)
            playerNameText.gameObject.SetActive(false);
    }
}
