using UnityEngine;
using UnityEngine.UI;

public class SkillMapConnection : MonoBehaviour
{
    public RectTransform From { get; private set; }
    public RectTransform To { get; private set; }

    private Image lineImage;
    private SkillSO fromSkill;
    private SkillSO toSkill;
    private RectTransform rect;
    private SkillMapController controller;

    private const float BASE_THICKNESS = 5f;
    private Color targetColor;
    private Color currentColor;

    private float fadeSpeed = 5f;

    public void Setup(RectTransform from, RectTransform to, SkillSO fromSO, SkillSO toSO, SkillMapController ctrl)
    {
        From = from;
        To = to;
        fromSkill = fromSO;
        toSkill = toSO;
        controller = ctrl;

        rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        lineImage = GetComponent<Image>() ?? gameObject.AddComponent<Image>();

        currentColor = targetColor = GetColorByState();
        lineImage.color = currentColor;
        RefreshLine();
    }

    public void RefreshLine()
    {
        if (From == null || To == null) return;

        Vector2 dir = To.anchoredPosition - From.anchoredPosition;
        float distance = dir.magnitude;
        rect.sizeDelta = new Vector2(distance, GetThickness());
        rect.anchoredPosition = From.anchoredPosition + dir / 2f;
        rect.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        targetColor = GetColorByState();
    }

    private float GetThickness()
    {
        if (controller == null) return BASE_THICKNESS;
        float zoom = controller.GetZoom();
        return BASE_THICKNESS / Mathf.Clamp(zoom, 0.6f, 2f);
    }

    private Color GetColorByState()
    {
        var mgr = SkillMapManager.Instance;
        if (mgr == null) return Color.gray;

        bool fromUnlocked = mgr.IsUnlocked(fromSkill);
        bool toUnlocked = mgr.IsUnlocked(toSkill);
        bool toBlocked = mgr.IsBlocked(toSkill);
        bool canUnlock = mgr.CanUnlock(toSkill);

        if (fromUnlocked && toUnlocked) return new Color(0.3f, 0.7f, 1f); // mavi
        if (toBlocked) return new Color(0.6f, 0.1f, 0.1f);                // koyu kırmızı
        if (fromUnlocked && canUnlock) return new Color(1f, 0.8f, 0.2f);   // sarı
        return new Color(0.3f, 0.3f, 0.3f);                               // gri
    }


    // 🔹 Hover sırasında çizgi parlaması
    private void Update()
    {
        if (lineImage == null) return;

        // Fade geçişi
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * fadeSpeed);
        lineImage.color = currentColor;
    }

    // === Hover efektleri ===
    public void HighlightHover(Color color)
    {
        targetColor = color; // parlak renk hedefi
    }

    public void ResetHighlight()
    {
        targetColor = GetColorByState(); // eski haline dön
    }

    public bool ConnectsSkill(SkillSO skill)
    {
        return skill == fromSkill || skill == toSkill;
    }
}
