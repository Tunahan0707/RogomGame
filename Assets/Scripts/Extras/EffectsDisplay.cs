using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectsDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    public int iconIndex { get; private set; } = -1;

    public void SetDatas(Sprite Icon, int Text, int IconIndex)
    {
        icon.sprite = Icon;
        text.text = "  " + Text.ToString();
        if (Text < 0)
            text.color = Color.red;
        else
            text.color = Color.white;
        iconIndex = IconIndex;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
