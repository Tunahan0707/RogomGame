using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SkillMapEditorWindow : EditorWindow
{
    // === Data ===
    private SkillMapSO currentMap;
    private SkillNodeDef selectedNode;
    private SkillNodeDef linkingFrom;
    private SkillNodeDef centerNode;
    private bool linkingExclusive;

    // === View Settings ===
    private const float BASE_NODE_SIZE = 200f;
    private const float GRID_SIZE = 20f;
    private Vector2 scrollPos;
    private float zoom = 1f;
    private const float zoomMin = 0.3f, zoomMax = 2.5f;

    // === Mini Map ===
    private const float MINIMAP_SIZE = 220f;
    private const float MINIMAP_SCALE = 0.1f;
    private bool draggingMiniMap;
    private Rect MinimapRect => new(position.width - MINIMAP_SIZE - 10, position.height - MINIMAP_SIZE - 30, MINIMAP_SIZE, MINIMAP_SIZE);

    // === Tooltip ===
    private string tooltipText = "";
    private Vector2 tooltipPos;

    // === Hover ===
    private readonly Dictionary<SkillNodeDef, float> hoverScale = new();

    [MenuItem("Window/Skill Map Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<SkillMapEditorWindow>("Skill Map Editor");
        window.minSize = new Vector2(1000, 700);
    }

    private void OnGUI()
    {
        HandleEvents();
        DrawToolbar();

        if (currentMap == null)
        {
            EditorGUILayout.HelpBox("Bir SkillMapSO seç.", MessageType.Info);
            return;
        }

        DrawGrid();

        Matrix4x4 oldMatrix = GUI.matrix;
        Vector3 pivot = new(position.width / 2f, position.height / 2f, 0);
        GUIUtility.ScaleAroundPivot(Vector2.one * zoom, pivot);

        DrawConnections();
        DrawNodes();

        GUI.matrix = oldMatrix;

        DrawTooltip();
        DrawMiniMap();

        Repaint();
    }

    #region === GRID ===
    private void DrawGrid()
    {
        Handles.color = new Color(0.15f, 0.15f, 0.15f, 0.7f);
        float grid = GRID_SIZE * zoom;
        for (float x = -scrollPos.x % grid; x < position.width; x += grid)
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, position.height));
        for (float y = -scrollPos.y % grid; y < position.height; y += grid)
            Handles.DrawLine(new Vector3(0, y), new Vector3(position.width, y));
        Handles.color = Color.white;
    }
    #endregion

    #region === TOOLBAR ===
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        currentMap = (SkillMapSO)EditorGUILayout.ObjectField(currentMap, typeof(SkillMapSO), false);

        if (GUILayout.Button("Kaydet", EditorStyles.toolbarButton))
        {
            if (currentMap != null)
            {
                EditorUtility.SetDirty(currentMap);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("Merkez Node Seç", EditorStyles.toolbarButton))
        {
            if (selectedNode != null)
            {
                foreach (var n in currentMap.skillNodes)
                    n.isCenter = false;
                selectedNode.isCenter = true;
                centerNode = selectedNode;
                Debug.Log($"🎯 Merkez node atandı: {selectedNode.skill?.skillName ?? "Unnamed"}");
            }
            else Debug.LogWarning("⚠️ Önce bir node seçmelisin!");
        }

        if (GUILayout.Button("Daire Yerleşimi", EditorStyles.toolbarButton))
            AutoArrangeCircle();

        if (GUILayout.Button("Grid Yerleşimi", EditorStyles.toolbarButton))
            AutoArrangeGrid();

        EditorGUILayout.EndHorizontal();

        if (currentMap != null && centerNode == null)
            centerNode = currentMap.skillNodes.Find(n => n.isCenter);
    }
    #endregion

    #region === AUTO ARRANGE ===
    private void AutoArrangeCircle()
    {
        if (currentMap == null || centerNode == null)
        {
            Debug.LogWarning("⚠️ Önce merkez node’u belirle!");
            return;
        }

        var levelGroups = new Dictionary<int, List<SkillNodeDef>>();
        foreach (var node in currentMap.skillNodes)
        {
            if (node == centerNode) continue;
            int lvl = Mathf.Max(1, node.requiredLevel);
            if (!levelGroups.ContainsKey(lvl))
                levelGroups[lvl] = new List<SkillNodeDef>();
            levelGroups[lvl].Add(node);
        }

        const float BASE_RADIUS = 300f;
        const float LEVEL_RADIUS_STEP = 250f;

        foreach (var kvp in levelGroups)
        {
            int level = kvp.Key;
            var group = kvp.Value;
            float radius = BASE_RADIUS + (level - 1) * LEVEL_RADIUS_STEP;
            float step = 360f / group.Count;
            for (int i = 0; i < group.Count; i++)
            {
                float angle = i * step * Mathf.Deg2Rad;
                group[i].position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
        }
    }

    private void AutoArrangeGrid()
    {
        if (currentMap == null || centerNode == null)
        {
            Debug.LogWarning("⚠️ Önce merkez node’u belirle!");
            return;
        }

        const float GRID_X = 350f, GRID_Y = 250f;
        const int COLS = 5;
        int index = 0;

        foreach (var node in currentMap.skillNodes)
        {
            if (node == centerNode) continue;
            int col = index % COLS;
            int row = index / COLS;
            float x = (col - (COLS / 2f)) * GRID_X;
            float y = -(row + 1) * GRID_Y;
            node.position = new Vector2(x, y);
            index++;
        }
    }
    #endregion

    #region === NODE RENDERING ===
    private void DrawNodes()
    {
        foreach (var node in currentMap.skillNodes)
        {
            if (!hoverScale.ContainsKey(node)) hoverScale[node] = 1f;

            float nodeSize = BASE_NODE_SIZE * zoom * hoverScale[node];
            Vector2 screenPos = (node.position + scrollPos) * zoom;
            Rect rect = new(screenPos.x - nodeSize / 2, screenPos.y - nodeSize / 2, nodeSize, nodeSize);

            bool hovered = rect.Contains(Event.current.mousePosition);
            if (Event.current.type == EventType.Repaint)
                hoverScale[node] = Mathf.Lerp(hoverScale[node], hovered ? 1.12f : 1f, 0.15f);

            // === Dark color palette ===
            Color baseColor = new(0.18f, 0.18f, 0.18f, 1f);
            if (node.skill != null)
            {
                baseColor = node.skill.type == SkillType.Passive ? new Color(0.1f, 0.35f, 0.2f) : new Color(0.1f, 0.2f, 0.4f);
            }
            if (node == centerNode) baseColor = new Color(0.45f, 0.35f, 0.1f);
            if (selectedNode == node) baseColor = new Color(0.35f, 0.15f, 0.15f);

            EditorGUI.DrawRect(rect, baseColor);

            GUIStyle labelStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 11,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(rect.x, rect.y + 5, rect.width, 20),
                node.skill?.skillName ?? "Empty", labelStyle);

            if (node.skill?.icon != null)
                GUI.DrawTexture(new Rect(rect.x + rect.width / 2 - 32, rect.y + 28, 64, 64),
                    node.skill.icon.texture, ScaleMode.ScaleToFit);

            if (selectedNode == node)
            {
                node.skill = (SkillSO)EditorGUI.ObjectField(
                    new Rect(rect.x + 10, rect.y + 100, rect.width - 20, 18),
                    node.skill, typeof(SkillSO), false);
                node.requiredLevel = EditorGUI.IntField(
                    new Rect(rect.x + 10, rect.y + 122, rect.width - 20, 16),
                    "Lvl", node.requiredLevel);
                node.isStarter = EditorGUI.Toggle(
                    new Rect(rect.x + 10, rect.y + 142, rect.width - 20, 16),
                    "Starter", node.isStarter);
                node.exclusiveGroup = EditorGUI.TextField(
                    new Rect(rect.x + 10, rect.y + 162, rect.width - 20, 16),
                    "Grup", node.exclusiveGroup);
            }

            HandleNodeInput(node, rect);
        }
    }

    private void HandleNodeInput(SkillNodeDef node, Rect rect)
    {
        Event e = Event.current;

        // Left Click
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            if (e.button == 0)
            {
                if (linkingFrom != null && linkingFrom != node)
                {
                    if (linkingExclusive)
                    {
                        string group = string.IsNullOrEmpty(linkingFrom.exclusiveGroup)
                            ? System.Guid.NewGuid().ToString()
                            : linkingFrom.exclusiveGroup;
                        linkingFrom.exclusiveGroup = node.exclusiveGroup = group;
                    }
                    else if (linkingFrom.skill != null && !node.prerequisites.Contains(linkingFrom.skill))
                    {
                        node.prerequisites.Add(linkingFrom.skill);
                    }
                    linkingFrom = null;
                    e.Use();
                }
                else
                {
                    selectedNode = node;
                    GUI.FocusControl(null);
                    e.Use();
                }
            }

            // Right Click
            else if (e.button == 1)
            {
                GenericMenu menu = new();

                menu.AddItem(new GUIContent("Node Sil"), false, () =>
                {
                    currentMap.skillNodes.Remove(node);
                    if (selectedNode == node) selectedNode = null;
                    GUI.changed = true;
                    Repaint();
                });

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Bağlantı Başlat (Prereq)"), false, () => { linkingFrom = node; linkingExclusive = false; });
                menu.AddItem(new GUIContent("Bağlantı Başlat (Exclusive)"), false, () => { linkingFrom = node; linkingExclusive = true; });

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Merkez Olarak Ata"), false, () =>
                {
                    foreach (var n in currentMap.skillNodes) n.isCenter = false;
                    node.isCenter = true;
                    centerNode = node;
                    Repaint();
                });

                menu.ShowAsContext();
                e.Use();
            }
        }

        // Drag Node
        if (e.type == EventType.MouseDrag && selectedNode == node && e.button == 0)
        {
            node.position += e.delta / zoom;
            node.position.x = Mathf.Round(node.position.x / GRID_SIZE) * GRID_SIZE;
            node.position.y = Mathf.Round(node.position.y / GRID_SIZE) * GRID_SIZE;
            GUI.changed = true;
            e.Use();
        }
    }
    #endregion

    #region === CONNECTIONS ===
    private void DrawConnections()
    {
        tooltipText = "";

        foreach (var node in currentMap.skillNodes)
        {
            foreach (var pre in node.prerequisites)
            {
                if (pre == null) continue;
                var from = currentMap.skillNodes.Find(n => n.skill == pre);
                if (from == null) continue;
                DrawConnection(from.position, node.position, new Color(0.2f, 0.7f, 0.2f), node, pre);
            }

            foreach (var other in currentMap.skillNodes)
            {
                if (other == node) continue;
                if (!string.IsNullOrEmpty(other.exclusiveGroup) && node.exclusiveGroup == other.exclusiveGroup)
                    DrawConnection(node.position, other.position, new Color(0.7f, 0.2f, 0.2f), node, null, true);
            }
        }

        if (linkingFrom != null)
        {
            Handles.color = linkingExclusive ? Color.red : Color.green;
            Handles.DrawLine((linkingFrom.position + scrollPos) * zoom, Event.current.mousePosition);
        }
    }

    private void DrawConnection(Vector2 from, Vector2 to, Color color, SkillNodeDef node, SkillSO pre, bool exclusive = false)
    {
        Vector2 p1 = (from + scrollPos) * zoom;
        Vector2 p2 = (to + scrollPos) * zoom;

        Handles.DrawBezier(p1, p2, p1 + Vector2.right * 50, p2 + Vector2.left * 50, color, null, 3f);

        if (Vector2.Distance(Event.current.mousePosition, (p1 + p2) / 2) < 10f)
        {
            tooltipText = exclusive ? "Birini açmak diğerini kapatır." : $"Gereken seviye: {node.requiredLevel}";
            tooltipPos = Event.current.mousePosition;

            if (Event.current.type == EventType.ContextClick)
            {
                GenericMenu m = new();
                m.AddItem(new GUIContent("Bağlantıyı Sil"), false, () =>
                {
                    if (pre != null) node.prerequisites.Remove(pre);
                    GUI.changed = true;
                    Repaint();
                });
                m.ShowAsContext();
                Event.current.Use();
            }
        }
    }
    #endregion

    #region === TOOLTIP & MINIMAP ===
    private void DrawTooltip()
    {
        if (string.IsNullOrEmpty(tooltipText)) return;
        Rect r = new(tooltipPos.x + 10, tooltipPos.y - 30, 220, 25);
        EditorGUI.DrawRect(r, new Color(0, 0, 0, 0.85f));
        GUI.Label(new Rect(r.x + 5, r.y + 5, r.width - 10, r.height - 10), tooltipText, EditorStyles.whiteLabel);
    }

    private void DrawMiniMap()
    {
        Rect r = MinimapRect;
        Event e = Event.current;

        GUI.BeginGroup(r, EditorStyles.helpBox);
        EditorGUI.DrawRect(new Rect(0, 0, r.width, r.height), new Color(0.05f, 0.05f, 0.05f, 0.8f));
        Vector2 center = r.size / 2f;

        foreach (var node in currentMap.skillNodes)
        {
            Vector2 pos = node.position * MINIMAP_SCALE + center;
            Color c = node.skill != null
                ? (node.skill.type == SkillType.Passive ? Color.green : Color.cyan)
                : Color.gray;
            if (node == centerNode) c = Color.yellow;
            EditorGUI.DrawRect(new Rect(pos.x - 3, pos.y - 3, 6, 6), c);
        }
        GUI.EndGroup();

        // === Dragging inside minimap ===
        if (e.type == EventType.MouseDown && r.Contains(e.mousePosition)) draggingMiniMap = true;
        if (e.type == EventType.MouseUp) draggingMiniMap = false;
        if (draggingMiniMap && e.type == EventType.MouseDrag)
        {
            scrollPos -= e.delta / MINIMAP_SCALE;
            e.Use();
        }
    }
    #endregion

    #region === EVENTS ===
    private void HandleEvents()
    {
        Event e = Event.current;

        if ((e.type == EventType.MouseDrag && e.button == 2) ||
            (e.type == EventType.MouseDrag && e.button == 0 && e.control))
        {
            scrollPos += e.delta / zoom;
            e.Use();
        }

        if (e.type == EventType.ScrollWheel)
        {
            float speed = e.alt ? 0.03f : 0.01f;
            float oldZoom = zoom;
            zoom = Mathf.Clamp(zoom - e.delta.y * speed, zoomMin, zoomMax);
            Vector2 mouse = e.mousePosition;
            scrollPos += (mouse / oldZoom - mouse / zoom);
        }

        if (e.type == EventType.ContextClick && FindNodeAt(e.mousePosition) == null)
        {
            GenericMenu m = new();
            m.AddItem(new GUIContent("Yeni Node Ekle"), false, () => AddNodeAt(e.mousePosition));
            m.ShowAsContext();
            GUIUtility.ExitGUI();
        }
    }

    private SkillNodeDef FindNodeAt(Vector2 mouse)
    {
        foreach (var node in currentMap.skillNodes)
        {
            float size = BASE_NODE_SIZE * zoom;
            Vector2 pos = (node.position + scrollPos) * zoom;
            if (new Rect(pos.x - size / 2, pos.y - size / 2, size, size).Contains(mouse))
                return node;
        }
        return null;
    }

    private void AddNodeAt(Vector2 mouse)
    {
        if (currentMap == null)
        {
            Debug.LogError("❌ Önce bir SkillMapSO seçmelisin!");
            return;
        }

        var newNode = new SkillNodeDef
        {
            position = (mouse / zoom) - scrollPos,
            requiredLevel = 1
        };
        newNode.position.x = Mathf.Round(newNode.position.x / GRID_SIZE) * GRID_SIZE;
        newNode.position.y = Mathf.Round(newNode.position.y / GRID_SIZE) * GRID_SIZE;
        currentMap.skillNodes.Add(newNode);
        selectedNode = newNode;
    }
    #endregion
}
