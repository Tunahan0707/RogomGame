using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;

public class PlayerDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<PlayersSO> OnPlayerSelected;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI playerShieldText;
    [SerializeField] public Image playerSprite;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerShieldBar;
    [SerializeField] private Image playerShieldBar2;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Button button;
    private HashSet<EffectsDisplay> effectDisplays;
    [SerializeField] private Transform effectList;
    [SerializeField] private EffectsDisplayUI effectsUI;

    public static Dictionary<string, PlayerDisplay> AllPlayers = new();

    public PlayersSO playerData { get; private set; }

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
        else
            Debug.LogWarning("PlayerDisplay: Button not assigned.", this);

        if (playerNameText != null)
            playerNameText.gameObject.SetActive(false);
        else
            Debug.LogWarning("PlayerDisplay: TextMeshProUGUI not assigned or found.", this);
        effectDisplays = new();
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
        if (PlayerManager.Instance != null)
        {
            playerShieldBar2.fillAmount = 0f;
            playerShieldBar.fillAmount = 0f;
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
        if (currentShield == 0)
            playerShieldText.gameObject.SetActive(false);
        else
        {
            if (!playerShieldText.gameObject.activeSelf)
                playerShieldText.gameObject.SetActive(true);
            playerShieldText.text = currentShield.ToString();
        }
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
        playerHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
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

    public void SetEffects(int iconIndex, int value)
    {
        if (effectsUI == null)
            effectsUI = FindAnyObjectByType<EffectsDisplayUI>();

        if (effectsUI == null)
        {
            Debug.LogError("EffectsDisplayUI sahnede yok!");
            return;
        }

        // Önce effectDisplays'te var mı diye bak
        var existingEffect = effectDisplays.FirstOrDefault(e => e.iconIndex == iconIndex);

        if (existingEffect != null)
        {
            if (value == 0)
            {
                existingEffect.Destroy();
                effectDisplays.Remove(existingEffect);
            }
            else
            {
                existingEffect.SetDatas(effectsUI.Icons[iconIndex], value, iconIndex);
            }
        }
        else
        {
            // Effect yok ve value 0 değilse yeni spawn et
            if (value != 0)
            {
                var newEffect = effectsUI.SpawnEffect(iconIndex, value, effectList);
                if (newEffect != null)
                    effectDisplays.Add(newEffect);
            }
        }
    }
}
