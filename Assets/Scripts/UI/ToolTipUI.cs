using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform root;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
        Hide();
    }

    public void Show(string content, Vector3 worldPos)
    {
        if (string.IsNullOrEmpty(content)) { Hide(); return; }
        text.text = content;
        root.position = worldPos;
        group.alpha = 1f; 
        group.blocksRaycasts = false; 
        group.interactable = false;
    }

    public void Hide()
    {
        group.alpha = 0f;
    }
}
