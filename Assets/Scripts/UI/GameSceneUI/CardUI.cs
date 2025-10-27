using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI deckText;
    [SerializeField] private TextMeshProUGUI discardText;

    private void Update()
    {
        int deckCount = CardZoneManager.GetByZone(CardZone.Draw).Count();
        int discardCount = CardZoneManager.GetByZone(CardZone.Discard).Count();

        deckText.text = "x" + deckCount;
        discardText.text = discardCount + "x";
    }
}
