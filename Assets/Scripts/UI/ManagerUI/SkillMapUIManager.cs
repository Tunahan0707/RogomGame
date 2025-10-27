using UnityEngine;
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

        foreach (var nodeDef in mapData.skillNodes)
        {
            if (nodeDef == null) continue;
            var go = Instantiate(skillNodePrefab, mapParent);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = nodeDef.position;

            var ui = go.GetComponent<SkillNodeUI>();
            ui.Setup(nodeDef, this);
            nodes.Add(ui);
        }
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
