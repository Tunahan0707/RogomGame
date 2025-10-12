using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI deckText;
    [SerializeField] private TextMeshProUGUI discardText;

    [SerializeField] private CardManager cardManager;

    private void Update()
    {
        deckText.text = "x" + cardManager.drawDeck.Count.ToString();
        discardText.text = cardManager.discardPile.Count.ToString() + "x";
    }
}
