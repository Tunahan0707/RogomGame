using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class SkillMapAutoResizer : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform content;
    private RectTransform viewport;

    [SerializeField] private float padding = 300f; // harita etrafına biraz boşluk

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        UpdateContentBounds();
        CenterContent();
    }

    /// <summary>
    /// Tüm çocukların pozisyonuna göre content boyutunu hesaplar.
    /// </summary>
    public void UpdateContentBounds()
    {
        if (content.childCount == 0) return;

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue);

        foreach (RectTransform child in content)
        {
            Vector3 pos = child.anchoredPosition;
            Vector2 size = child.sizeDelta * 0.5f;

            min.x = Mathf.Min(min.x, pos.x - size.x);
            min.y = Mathf.Min(min.y, pos.y - size.y);
            max.x = Mathf.Max(max.x, pos.x + size.x);
            max.y = Mathf.Max(max.y, pos.y + size.y);
        }

        Vector2 newSize = (max - min) + Vector3.one * padding;
        content.sizeDelta = newSize;

        // Content pivotu ortada olmalı
        content.pivot = new Vector2(0.5f, 0.5f);
        content.anchorMin = new Vector2(0.5f, 0.5f);
        content.anchorMax = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// ScrollView'in içeriğini ekranın ortasında başlatır.
    /// </summary>
    public void CenterContent()
    {
        if (scrollRect == null || content == null || viewport == null)
            return;

        Vector2 contentSize = content.rect.size;
        Vector2 viewportSize = viewport.rect.size;

        Vector2 offset = (contentSize - viewportSize) / 2f;

        float normX = 0.5f;
        float normY = 0.5f;

        if (scrollRect.horizontal && contentSize.x > viewportSize.x)
            normX = offset.x / (contentSize.x - viewportSize.x);

        if (scrollRect.vertical && contentSize.y > viewportSize.y)
            normY = offset.y / (contentSize.y - viewportSize.y);

        scrollRect.normalizedPosition = new Vector2(normX, 1f - normY);
    }
}
