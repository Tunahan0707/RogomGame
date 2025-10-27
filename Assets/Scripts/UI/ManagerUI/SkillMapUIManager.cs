using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillMapUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform mapParent;
    [SerializeField] private GameObject skillNodePrefab;

    private readonly List<SkillNodeUI> nodes = new();

    public SkillMapSO MapData => SkillMapManager.Instance != null ? SkillMapManager.Instance.GetCurrentMapSO() : null;

    private void OnEnable()
    {
        BuildMap();
        SkillMapManager.OnSkillUnlocked += OnSkillUnlocked;
        SkillMapManager.OnCurseChanged += OnCurseChanged;
    }

    private void OnDisable()
    {
        SkillMapManager.OnSkillUnlocked -= OnSkillUnlocked;
        SkillMapManager.OnCurseChanged -= OnCurseChanged;
    }

    private void BuildMap()
    {
        var mapData = MapData;
        if (mapData == null)
        {
            Debug.LogWarning("SkillMapUIManager: MapData yok (aktif oyuncu/harita atanmamış).");
            return;
        }

        foreach (Transform child in mapParent) Destroy(child.gameObject);
        nodes.Clear();

        SkillNodeDef centerDef = null;
        foreach (var node in mapData.skillNodes)
        {
            if (node != null && node.isCenter)
            {
                centerDef = node;
                break;
            }
        }

        Vector2 centerOffset = centerDef != null ? centerDef.position : Vector2.zero;

        foreach (var nodeDef in mapData.skillNodes)
        {
            if (nodeDef == null) continue;
            var go = Instantiate(skillNodePrefab, mapParent);
            var rect = go.GetComponent<RectTransform>();
            Vector2 anchoredPos = nodeDef.position - centerOffset;

            if (nodeDef.isCenter)
                anchoredPos = Vector2.zero;

            rect.anchoredPosition = anchoredPos;

            if (nodeDef.isCenter)
            {
                HideCenterNode(go);
                continue;
            }

            var ui = go.GetComponent<SkillNodeUI>();
            if (ui == null) continue;

            ui.Setup(nodeDef, this);
            nodes.Add(ui);
        }
    }

    private static void HideCenterNode(GameObject go)
    {
        if (!go.TryGetComponent(out CanvasGroup group))
            group = go.AddComponent<CanvasGroup>();

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        if (go.TryGetComponent(out Button button))
            button.interactable = false;
    }

    private void OnSkillUnlocked(SkillSO _)
    {
        RefreshAll();
    }

    private void OnCurseChanged(int _)
    {
        // İstersen lanet yüzdesini UI’da göster
    }

    public void RefreshAll()
    {
        foreach (var n in nodes)
            n.UpdateVisual();
    }
}
