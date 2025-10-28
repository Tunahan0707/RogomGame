using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        DrawConnectionHUD();

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

        if (GUILayout.Button("Grid Yerleşimi", EditorStyles.toolbarButton))
            AutoArrangeGrid();

        if (GUILayout.Button("Akıllı Yerleşim", EditorStyles.toolbarButton))
            AutoArrangeSmart();

        EditorGUILayout.EndHorizontal();

        if (currentMap != null && centerNode == null)
            centerNode = currentMap.skillNodes.Find(n => n.isCenter);
    }

    private void DrawConnectionHUD()
    {
        if (linkingFrom == null) return;

        GUIStyle infoStyle = new(EditorStyles.helpBox)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            richText = true
        };

        GUILayout.BeginHorizontal(infoStyle);
        GUILayout.Label($"<b>Bağlantı Modu:</b> {linkingFrom.skill?.skillName ?? "Empty"} → (hedef seç)", GUILayout.ExpandWidth(true));
        if (Event.current.alt)
            GUILayout.Label("<color=orange>ALT: Ters yön</color>", GUILayout.Width(120));
        if (GUILayout.Button("İptal", GUILayout.Width(70)))
            linkingFrom = null;
        GUILayout.EndHorizontal();
    }
    #endregion
    #region === NODE RENDERING ===
    private void DrawNodes()
    {
        foreach (var node in currentMap.skillNodes)
        {
            if (node == null) continue;
            if (!hoverScale.ContainsKey(node)) hoverScale[node] = 1f;

            // MERKEZ NODE
            if (node == centerNode)
            {
                //node.position = Vector2.zero;
                float size = BASE_NODE_SIZE * zoom;
                Vector2 screenPos = (node.position + scrollPos) * zoom;
                Rect rect = new(screenPos.x - size / 2, screenPos.y - size / 2, size, size);
                EditorGUI.DrawRect(rect, new Color(0.45f, 0.35f, 0.1f));

                GUIStyle style = new(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                GUI.Label(rect, "★ MERKEZ ★", style);
                continue;
            }

            float nodeSize = BASE_NODE_SIZE * zoom * hoverScale[node];
            Vector2 screen = (node.position + scrollPos) * zoom;
            Rect r = new(screen.x - nodeSize / 2, screen.y - nodeSize / 2, nodeSize, nodeSize);

            bool hovered = r.Contains(Event.current.mousePosition);
            if (Event.current.type == EventType.Repaint)
                hoverScale[node] = Mathf.Lerp(hoverScale[node], hovered ? 1.15f : 1f, 0.18f);

            Color baseColor = new(0.18f, 0.18f, 0.18f);
            if (node.skill != null)
                baseColor = node.skill.type == SkillType.Passive ? new Color(0.1f, 0.35f, 0.2f) : new Color(0.1f, 0.2f, 0.4f);
            if (selectedNode == node) baseColor = new Color(0.35f, 0.15f, 0.15f);

            EditorGUI.DrawRect(r, baseColor);

            GUIStyle labelStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 11,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(r.x, r.y + 5, r.width, 20), node.skill?.skillName ?? "Empty", labelStyle);

            if (node.skill?.icon != null)
                GUI.DrawTexture(new Rect(r.x + r.width / 2 - 32, r.y + 28, 64, 64),
                    node.skill.icon.texture, ScaleMode.ScaleToFit);

            // Detay Panel
            if (selectedNode == node)
            {
                Matrix4x4 prevMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.identity;

                Vector2 nodeScreenPos = (node.position + scrollPos) * zoom;
                float fieldWidth = BASE_NODE_SIZE * 0.9f;
                float fieldHeight = 85f;
                float offsetY = BASE_NODE_SIZE * 0.55f * zoom;

                Rect panelRect = new(
                    nodeScreenPos.x - fieldWidth / 2f,
                    nodeScreenPos.y + offsetY,
                    fieldWidth,
                    fieldHeight
                );

                EditorGUI.DrawRect(panelRect, new Color(0.08f, 0.08f, 0.08f, 0.92f));

                float innerX = panelRect.x + 6;
                float innerW = panelRect.width - 12;
                float y = panelRect.y + 4;

                node.skill = (SkillSO)EditorGUI.ObjectField(
                    new Rect(innerX, y, innerW, 18),
                    node.skill, typeof(SkillSO), false);
                y += 20;

                node.requiredLevel = EditorGUI.IntField(
                    new Rect(innerX, y, innerW, 18),
                    "Lvl", node.requiredLevel);
                y += 20;

                node.isStarter = EditorGUI.Toggle(
                    new Rect(innerX, y, innerW, 18),
                    "Starter", node.isStarter);
                y += 20;

                node.exclusiveGroup = EditorGUI.TextField(
                    new Rect(innerX, y, innerW, 18),
                    "Grup", node.exclusiveGroup);

                GUI.matrix = prevMatrix;
            }

            HandleNodeInput(node, r);
        }
    }

    private void HandleNodeInput(SkillNodeDef node, Rect rect)
    {
        if (node == centerNode) return;
        Event e = Event.current;

        // Sol tık
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
                        Debug.Log($"🔴 Exclusive grup: {group}");
                    }
                    else
                    {
                        bool inverse = e.alt;
                        bool ok = inverse
                            ? TryConnectPrereq(node, linkingFrom)
                            : TryConnectPrereq(linkingFrom, node);
                        if (!ok) Debug.LogWarning("⚠ Bağlantı kurulamadı.");
                    }

                    linkingFrom = null;
                    e.Use();
                    GUI.changed = true;
                    Repaint();
                    return;
                }
                else
                {
                    selectedNode = node;
                    GUI.FocusControl(null);
                    e.Use();
                }
            }
            // Sağ tık menü
            else if (e.button == 1)
            {
                GenericMenu menu = new();

                menu.AddItem(new GUIContent("Node Sil"), false, () =>
                {
                    Undo.RecordObject(currentMap, "Delete Node");
                    currentMap.skillNodes.Remove(node);
                    if (selectedNode == node) selectedNode = null;
                    EditorUtility.SetDirty(currentMap);
                    AssetDatabase.SaveAssets();
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

        // Drag
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
            if (node == null) continue;

            // prerequisite (A → node)
            foreach (var pre in node.prerequisites)
            {
                if (pre == null) continue;
                DrawConnection(pre.position, node.position, new Color(0.2f, 0.7f, 0.2f), node, pre);
            }

            // exclusive
            foreach (var other in currentMap.skillNodes)
            {
                if (other == node) continue;
                if (!string.IsNullOrEmpty(other.exclusiveGroup) && node.exclusiveGroup == other.exclusiveGroup)
                    DrawConnection(node.position, other.position, new Color(0.7f, 0.2f, 0.2f), node, null, true);
            }
        }

        // linking preview
        if (linkingFrom != null)
        {
            Handles.color = linkingExclusive ? Color.red : Color.green;
            Handles.DrawLine((linkingFrom.position + scrollPos) * zoom, Event.current.mousePosition);
        }
    }

    private void DrawConnection(Vector2 from, Vector2 to, Color color, SkillNodeDef node, SkillNodeDef pre, bool exclusive = false)
    {
        Vector2 p1 = (from + scrollPos) * zoom;
        Vector2 p2 = (to + scrollPos) * zoom;
        Vector2 mid = (p1 + p2) / 2;
        Event e = Event.current;

        float dist = Vector2.Distance(e.mousePosition, mid);
        bool hovered = dist < 18f;

        Color drawColor = color;
        float width = 3f;

        if (hovered)
        {
            drawColor = exclusive ? new Color(1f, 0.4f, 0.4f) : Color.white;
            width = 5f;
        }

        Handles.DrawBezier(p1, p2, p1 + Vector2.right * 50f, p2 + Vector2.left * 50f, drawColor, null, width);

        if (hovered)
        {
            tooltipText = exclusive
                ? "🔴 Exclusive bağlantı — Shift + Sol Tık → Sil"
                : "🟢 Prerequisite (A → B) — Shift + Sol Tık → Sil";
            tooltipPos = e.mousePosition;

            if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
            {
                Undo.RecordObject(currentMap, "Remove Prerequisite");
                node.prerequisites.Remove(pre);
                Debug.Log($"❌ Bağlantı silindi: {pre.skill?.skillName ?? "?"} → {node.skill?.skillName ?? "?"}");
                EditorUtility.SetDirty(currentMap);
                AssetDatabase.SaveAssets();
                Repaint();
                e.Use();
            }
        }
    }
    #endregion
    #region === TOOLTIP & MINIMAP ===
    private void DrawTooltip()
    {
        if (string.IsNullOrEmpty(tooltipText)) return;
        Rect r = new(tooltipPos.x + 10, tooltipPos.y - 30, 260, 25);
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

        // Pan
        if ((e.type == EventType.MouseDrag && e.button == 2) ||
            (e.type == EventType.MouseDrag && e.button == 0 && e.control))
        {
            scrollPos += e.delta / zoom;
            e.Use();
        }

        // Zoom
        if (e.type == EventType.ScrollWheel)
        {
            float speed = e.alt ? 0.03f : 0.01f;
            float oldZoom = zoom;
            zoom = Mathf.Clamp(zoom - e.delta.y * speed, zoomMin, zoomMax);
            Vector2 mouse = e.mousePosition;
            scrollPos += (mouse / oldZoom - mouse / zoom);
        }

        // Boş alana sağ tık
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
            if (node == null) continue;
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

    // yeni node oluştur
    var newNode = new SkillNodeDef
    {
        position = (mouse / zoom) - scrollPos,
        requiredLevel = 1,
        isStarter = false,
        isCenter = false,
        exclusiveGroup = "",
        prerequisites = new List<SkillNodeDef>()
    };

    // grid hizalaması
    newNode.position.x = Mathf.Round(newNode.position.x / GRID_SIZE) * GRID_SIZE;
    newNode.position.y = Mathf.Round(newNode.position.y / GRID_SIZE) * GRID_SIZE;

    // Undo kayıt + listeye ekleme
    Undo.RecordObject(currentMap, "Add Node");

    // yeni node'u SerializeReference olarak ekle
    if (currentMap.skillNodes == null)
        currentMap.skillNodes = new List<SkillNodeDef>();

    currentMap.skillNodes.Add(newNode);

    selectedNode = newNode;
    EditorUtility.SetDirty(currentMap);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    Debug.Log("✅ Yeni node eklendi (SerializeReference uyumlu).");
}

    #endregion

    #region === AUTO ARRANGE ===
    private void AutoArrangeSmart()
    {
        if (currentMap == null || currentMap.skillNodes == null || currentMap.skillNodes.Count == 0)
        {
            Debug.LogWarning("⚠️ Önce bir SkillMapSO seç!");
            return;
        }

        // 1️⃣ Merkez node belirle
        if (centerNode == null)
            centerNode = currentMap.skillNodes.Find(n => n != null && n.isCenter);
        if (centerNode == null)
            centerNode = currentMap.skillNodes.Find(n => n != null && (n.prerequisites == null || n.prerequisites.Count == 0))
                        ?? currentMap.skillNodes[0];

        // 2️⃣ Pozisyonları sıfırla
        foreach (var n in currentMap.skillNodes)
            if (n != null) n.position = Vector2.zero;
        centerNode.position = Vector2.zero; // merkez tam orijin

        // 3️⃣ adjacency (parent → children)
        Dictionary<SkillNodeDef, List<SkillNodeDef>> adjacency = new();
        foreach (var n in currentMap.skillNodes)
            if (n != null) adjacency[n] = new();

        foreach (var node in currentMap.skillNodes)
        {
            if (node == null || node.prerequisites == null) continue;
            foreach (var pre in node.prerequisites)
            {
                if (pre == null) continue;
                if (!adjacency.ContainsKey(pre))
                    adjacency[pre] = new List<SkillNodeDef>();
                if (!adjacency[pre].Contains(node))
                    adjacency[pre].Add(node);
            }
        }

        // 4️⃣ Kök nodelar (merkez hariç prerequisites olmayanlar)
        List<SkillNodeDef> roots = new();
        foreach (var n in currentMap.skillNodes)
        {
            if (n == null || n == centerNode) continue;
            if (n.prerequisites == null || n.prerequisites.Count == 0)
                roots.Add(n);
        }

        // 5️⃣ Kökleri merkez etrafında dairesel diz
        float rootRadius = 230f;
        for (int i = 0; i < roots.Count; i++)
        {
            float angle = (Mathf.PI * 2f * i) / Mathf.Max(1, roots.Count);
            Vector2 pos = centerNode.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * rootRadius;
            roots[i].position = pos;
        }

        // 6️⃣ BFS ile dallanma yerleşimi
        const float CHILD_DISTANCE = 170f;
        const float ANGLE_SPREAD = 30f;

        Queue<SkillNodeDef> q = new();
        HashSet<SkillNodeDef> visited = new();

        // merkezden başla
        q.Enqueue(centerNode);
        visited.Add(centerNode);

        // merkezden sonra kökleri ekle
        foreach (var r in roots)
        {
            if (r != null && !visited.Contains(r))
            {
                q.Enqueue(r);
                visited.Add(r);
            }
        }

        while (q.Count > 0)
        {
            var parent = q.Dequeue();
            if (!adjacency.TryGetValue(parent, out var children) || children.Count == 0)
                continue;

            Vector2 parentPos = parent.position;
            Vector2 baseDir = (parent == centerNode)
                ? Vector2.up
                : (parent.position - centerNode.position).normalized;
            if (baseDir == Vector2.zero) baseDir = Vector2.up;

            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x);
            float spread = Mathf.Deg2Rad * Mathf.Min(ANGLE_SPREAD * children.Count, 120f);
            float angleStep = (children.Count > 1) ? spread / (children.Count - 1) : 0f;
            float startAngle = baseAngle - spread / 2f;

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (child == null || visited.Contains(child)) continue;

                float angle = startAngle + i * angleStep;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 target = parentPos + dir * CHILD_DISTANCE;
                target += UnityEngine.Random.insideUnitCircle * 10f; // çakışmayı azalt

                child.position = target;
                visited.Add(child);
                q.Enqueue(child);
            }
        }

        EditorUtility.SetDirty(currentMap);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Akıllı Yerleşim (merkez referanslı, kaymasız) tamamlandı. Node sayısı: {currentMap.skillNodes.Count}");
    }


    private void AutoArrangeGrid()
    {
        if (currentMap == null)
        {
            Debug.LogWarning("⚠️ Önce bir SkillMapSO seç!");
            return;
        }
        if (centerNode == null)
            centerNode = currentMap.skillNodes.Find(n => n.isCenter);

        centerNode.position = Vector2.zero;

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

        EditorUtility.SetDirty(currentMap);
        AssetDatabase.SaveAssets();
    }
    #endregion

    #region === GRAPH HELPERS ===
    private bool TryConnectPrereq(SkillNodeDef fromParent, SkillNodeDef toChild)
    {
        if (fromParent == null || toChild == null) return false;
        if (fromParent == toChild) return false;
        if (toChild.prerequisites.Contains(fromParent))
        {
            Debug.LogWarning("⚠️ Bu bağlantı zaten var!");
            return false;
        }
        if (WouldCreateCycle(fromParent, toChild))
        {
            Debug.LogWarning("⛔ Döngü oluştu, bağlantı iptal.");
            return false;
        }

        Undo.RecordObject(currentMap, "Add Prerequisite");
        toChild.prerequisites.Add(fromParent);
        EditorUtility.SetDirty(currentMap);
        AssetDatabase.SaveAssets();
        Debug.Log($"🟢 Bağlantı: {fromParent.skill?.skillName ?? "?"} → {toChild.skill?.skillName ?? "?"}");
        return true;
    }

    private bool WouldCreateCycle(SkillNodeDef parent, SkillNodeDef child)
    {
        var adj = new Dictionary<SkillNodeDef, List<SkillNodeDef>>();
        foreach (var n in currentMap.skillNodes) adj[n] = new List<SkillNodeDef>();
        foreach (var n in currentMap.skillNodes)
            foreach (var pre in n.prerequisites)
                if (pre != null)
                    adj[pre].Add(n);

        if (!adj[parent].Contains(child))
            adj[parent].Add(child);

        var stack = new Stack<SkillNodeDef>();
        var seen = new HashSet<SkillNodeDef>();
        stack.Push(child);
        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            if (cur == parent) return true;
            if (!seen.Add(cur)) continue;
            foreach (var nxt in adj[cur]) stack.Push(nxt);
        }
        return false;
    }
    #endregion
}
