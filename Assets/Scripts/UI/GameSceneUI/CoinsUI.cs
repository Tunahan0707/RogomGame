using UnityEngine;
using TMPro;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private void OnEnable()
    {
        CoinManager.OnCoinsChanged += UpdateCoinsUI;
    }

    private void OnDisable()
    {
        CoinManager.OnCoinsChanged -= UpdateCoinsUI;
    }

    private void UpdateCoinsUI(int newCoinAmount)
    {
        coinsText.text = "x" + newCoinAmount;
    }
}
