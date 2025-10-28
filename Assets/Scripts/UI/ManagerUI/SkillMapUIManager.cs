using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class SkillMapUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillMapController skillMapController;
    [SerializeField] private RectTransform mapParent;
    [SerializeField] private GameObject skillNodePrefab;
    [SerializeField] private GameObject connectionLinePrefab; // Çizgi prefab (UI Image)
    [SerializeField] private TextMeshProUGUI PUANText;

    private readonly List<SkillNodeUI> nodes = new();
    private readonly List<SkillMapConnection> connections = new();

    public SkillMapSO MapData => SkillMapManager.Instance?.GetCurrentMapSO();

    private void OnEnable()
    {
        BuildMap();
        SkillMapManager.OnSkillUnlocked += _ => RefreshAll();
    }

    private void OnDisable()
    {
        SkillMapManager.OnSkillUnlocked -= _ => RefreshAll();
    }

    private void Start()
    {
        PUANText.text = $"PUAN: {PUANManager.puan}";
    }

    private void BuildMap()
    {
        if (MapData == null) return;

        foreach (Transform child in mapParent) Destroy(child.gameObject);
        nodes.Clear();
        connections.Clear();

        SkillNodeDef center = MapData.skillNodes.Find(n => n != null && n.isCenter);
        Vector2 centerOffset = center != null ? center.position : Vector2.zero;

        foreach (var def in MapData.skillNodes)
        {
            if (def == null) continue;

            var go = Instantiate(skillNodePrefab, mapParent);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = def.isCenter ? Vector2.zero : def.position - centerOffset;

            if (def.isCenter)
            {
                skillMapController.centerNode = rect;
                HideCenterNode(go);
                continue;
            }

            var ui = go.GetComponent<SkillNodeUI>();
            ui.Setup(def, this);
            nodes.Add(ui);
        }

        CreateConnections();
        skillMapController.UpdateContentBounds();
        skillMapController.AutoZoomToFit();
        skillMapController.CenterOnNode(skillMapController.centerNode);
    }

    private void CreateConnections()
    {
        // 1️⃣ NodeDef → RectTransform
        Dictionary<SkillNodeDef, RectTransform> nodeMap = new();
        // 2️⃣ SkillSO → NodeDef
        Dictionary<SkillSO, SkillNodeDef> skillToNodeDef = new();

        foreach (var def in MapData.skillNodes)
        {
            if (def == null) continue;
            if (def.skill != null && !skillToNodeDef.ContainsKey(def.skill))
                skillToNodeDef[def.skill] = def;
        }

        foreach (var n in nodes)
        {
            var def = n.GetNodeDef();
            if (def != null)
                nodeMap[def] = n.GetComponent<RectTransform>();
        }

        // 3️⃣ Bağlantıları oluştur
        foreach (var def in MapData.skillNodes)
        {
            if (def == null || def.skill == null || def.prerequisites == null)
                continue;

            if (!nodeMap.TryGetValue(def, out RectTransform targetRect))
                continue;

            foreach (var preSkill in def.prerequisites)
            {
                if (preSkill == null) continue;

                if (!skillToNodeDef.TryGetValue(preSkill.skill, out SkillNodeDef preDef))
                    continue;

                if (!nodeMap.TryGetValue(preDef, out RectTransform sourceRect))
                    continue;

                bool exists = connections.Exists(c =>
                    (c.From == sourceRect && c.To == targetRect) ||
                    (c.From == targetRect && c.To == sourceRect));
                if (exists) continue;

                var lineObj = Instantiate(connectionLinePrefab, mapParent);
                var conn = lineObj.AddComponent<SkillMapConnection>();
                conn.Setup(sourceRect, targetRect, preSkill.skill, def.skill, skillMapController);
                connections.Add(conn);
            }

        }
    }




    private static void HideCenterNode(GameObject go)
    {
        if (!go.TryGetComponent(out CanvasGroup group))
            group = go.AddComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void RefreshAll()
    {
        foreach (var n in nodes)
            n.UpdateVisual();

        foreach (var c in connections)
            c.RefreshLine();
    }

    public void HighlightConnectionsForSkill(SkillSO skill)
    {
        foreach (var c in connections)
        {
            if (c.ConnectsSkill(skill))
                c.HighlightHover(new Color(1f, 1f, 0.4f)); // parlak sarı
        }
    }

    public void ResetAllHighlights()
    {
        foreach (var c in connections)
            c.ResetHighlight();
    }



    // === Highlight Sistem ===
    public void HighlightExclusiveGroup(string group, SkillNodeUI source)
    {
        foreach (var n in nodes)
        {
            if (n == source) continue;
            if (n.GetExclusiveGroup() == group)
                n.GetComponent<Image>().color = Color.gray * 0.5f; // gri efekt
        }
    }

    public void ResetExclusiveHighlights()
    {
        foreach (var n in nodes)
            n.UpdateVisual();
    }
}
