using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class SkillMapController : MonoBehaviour, IScrollHandler
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;
    public RectTransform centerNode; // merkez node referansı

    [Header("Settings")]
    [SerializeField] private float padding = 150f;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float maxZoom = 2.5f;

    private RectTransform content;
    private RectTransform viewport;
    private float currentZoom = 1f;
    private float minZoom = 1f;

    private Vector2 nodeCenterOffset = Vector2.zero;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    private void Start()
    {
        StartCoroutine(InitializeAfterFrame());
    }

    private System.Collections.IEnumerator InitializeAfterFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return null;

        // Ölçekleri sıfırla
        content.localScale = Vector3.one;

        UpdateContentBounds();
        AutoZoomToFit();

        if (centerNode != null)
            CenterOnNode(centerNode);
    }

    /// <summary>
    /// Tüm node’ları kapsayacak şekilde Content boyutunu ayarlar (Canvas dışındakiler dahil).
    /// </summary>
    public void UpdateContentBounds()
    {
        if (content.childCount == 0) return;

        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);

        foreach (RectTransform child in content)
        {
            Vector2 pos = child.anchoredPosition;
            Vector2 size = child.sizeDelta * 0.5f;

            min.x = Mathf.Min(min.x, pos.x - size.x);
            min.y = Mathf.Min(min.y, pos.y - size.y);
            max.x = Mathf.Max(max.x, pos.x + size.x);
            max.y = Mathf.Max(max.y, pos.y + size.y);
        }

        // Viewport boyutunu al
        Vector2 viewportSize = viewport.rect.size;
        Vector2 canvasExtra = Vector2.zero;

        // Sadece node’lar Viewport’un sınırını aşıyorsa ek alan ver
        if (max.x > viewportSize.x / 2f) canvasExtra.x = viewportSize.x / 2f;
        if (min.x < -viewportSize.x / 2f) canvasExtra.x = viewportSize.x / 2f;

        if (max.y > viewportSize.y / 2f) canvasExtra.y = viewportSize.y / 2f;
        if (min.y < -viewportSize.y / 2f) canvasExtra.y = viewportSize.y / 2f;

        Vector2 totalMin = min - canvasExtra;
        Vector2 totalMax = max + canvasExtra;

        Vector2 newSize = (totalMax - totalMin) + Vector2.one * padding;

        content.sizeDelta = newSize;
        content.pivot = content.anchorMin = content.anchorMax = new Vector2(0.5f, 0.5f);
    }



    /// <summary>
    /// Haritayı tamamen ekrana sığdıracak minimum zoom değerini belirler.
    /// </summary>
    public void AutoZoomToFit()
    {
        Vector2 contentSize = content.rect.size;
        Vector2 viewportSize = viewport.rect.size;

        float zoomX = viewportSize.x / contentSize.x;
        float zoomY = viewportSize.y / contentSize.y;

        minZoom = Mathf.Min(zoomX, zoomY);
        currentZoom = minZoom;
        content.localScale = Vector3.one * currentZoom;
    }

    /// <summary>
    /// Fare tekeri ile yakınlaştırma/uzaklaştırma.
    /// </summary>
    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y * zoomSpeed;
        Zoom(scroll);
    }

    public void Zoom(float delta)
    {
        currentZoom = Mathf.Clamp(currentZoom + delta, minZoom, maxZoom);
        content.localScale = Vector3.one * currentZoom;
        ClampContentPosition();
    }

    public float GetZoom() => currentZoom;

    /// <summary>
    /// Zoom sonrası içerik taşmasını engeller.
    /// </summary>
    private void ClampContentPosition()
    {
        Vector2 contentSize = content.rect.size * currentZoom;
        Vector2 viewportSize = viewport.rect.size;
        Vector2 pos = content.anchoredPosition;

        float limitX = Mathf.Max(0, (contentSize.x - viewportSize.x) / 2f);
        float limitY = Mathf.Max(0, (contentSize.y - viewportSize.y) / 2f);

        pos.x = Mathf.Clamp(pos.x, -limitX, limitX);
        pos.y = Mathf.Clamp(pos.y, -limitY, limitY);

        content.anchoredPosition = pos;
    }

    /// <summary>
    /// Belirli bir node'u viewport merkezine hizalar.
    /// </summary>
    public void CenterOnNode(RectTransform target)
    {
        if (target == null) return;

        // Node’un Content içindeki pozisyonuna göre merkezle
        Vector2 nodePos = (target.anchoredPosition - nodeCenterOffset) * currentZoom;
        content.anchoredPosition = -nodePos;
        ClampContentPosition();
    }
}
