using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillMapManager : MonoBehaviour
{
    public static SkillMapManager Instance;

    [Header("Aktif Oyuncu ve Harita (TANIM)")]
    private PlayersSO currentPlayer; // dışarıdan atanır (seçilen karakter)
    private SkillMapSO currentMapSO; // genelde currentPlayer.skillMap

    private SkillMapState state; // runtime + save

    // --- EVENT'ler ---
    public static event Action<SkillSO> OnSkillUnlocked;
    public static event Action<int> OnCurseChanged;
    public static event Action<SkillSO> OnPassiveGained;
    public static event Action<SkillSO> OnActiveGained;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (currentPlayer != null && currentMapSO == null)
            currentMapSO = currentPlayer.skillMap;

        if (currentMapSO != null)
            LoadOrCreateState();
    }

    // === Dış API ===
    public void SetCurrentPlayer(PlayersSO player)
    {
        currentPlayer = player;
        currentMapSO = player != null ? player.skillMap : null;
        LoadOrCreateState();
        BroadcastFullRefresh();
    }

    public SkillMapSO GetCurrentMapSO() => currentMapSO;
    public SkillMapState GetCurrentState() => state;

    public bool IsUnlocked(SkillSO s) => s != null && state != null && state.unlocked.Contains(s.skillID);
    public bool IsBlocked(SkillSO s) => s != null && state != null && state.blocked.Contains(s.skillID);

    public bool CanUnlock(SkillSO s)
    {
        if (s == null || currentMapSO == null || state == null) return false;
        var def = FindNodeDef(s);
        if (def == null) return false;
        if (state.unlocked.Contains(s.skillID)) return false;
        if (state.blocked.Contains(s.skillID)) return false;

        // Level kontrolü
        int level = XPManager.Instance != null ? XPManager.Instance.GetPlayerLevel() : 1;
        if (level < def.requiredLevel) return false;

        // Gereken skiller
        if (def.prerequisites != null)
        {
            foreach (var pre in def.prerequisites)
            {
                if (pre == null) continue;
                if (!state.unlocked.Contains(pre.skillID))
                    return false;
            }
        }

        // PUAN kontrolü
        if (PUANManager.puan < s.cost) return false;

        return true;
    }

    public bool TryUnlock(SkillSO s)
    {
        if (!CanUnlock(s)) return false;
        if (!PUANManager.Instance.SpendPUAN(s.cost)) return false;

        state.unlocked.Add(s.skillID);

        // Exclusive grup kontrolü
        var def = FindNodeDef(s);
        if (!string.IsNullOrEmpty(def.exclusiveGroup))
        {
            foreach (var other in currentMapSO.skillNodes)
            {
                if (other.skill == null || other.skill == s) continue;
                if (other.exclusiveGroup == def.exclusiveGroup)
                    state.blocked.Add(other.skill.skillID);
            }
        }

        // Gizli lanet ekle
        AddCurse(s.curseGain);

        // Event tetikleme
        OnSkillUnlocked?.Invoke(s);
        if (s.type == SkillType.Passive) OnPassiveGained?.Invoke(s);
        else OnActiveGained?.Invoke(s);

        SaveState();
        BroadcastFullRefresh();
        return true;
    }

    // === Internal ===
    private SkillNodeDef FindNodeDef(SkillSO s)
    {
        if (currentMapSO == null || s == null) return null;
        return currentMapSO.skillNodes.Find(n => n != null && n.skill == s);
    }

    private void AddCurse(int amount)
    {
        if (state == null) return;
        int before = state.cursePercent;
        state.cursePercent = Mathf.Clamp(before + Mathf.Max(0, amount), 0, 100);

        if (state.cursePercent != before)
            OnCurseChanged?.Invoke(state.cursePercent);

        if (state.cursePercent >= 100)
            TriggerCurseEvent();
    }

    private void TriggerCurseEvent()
    {
        // 🔥 Burayı senin özel lanet sisteminle dolduracağız
        Debug.Log("☠️ Lanet %100 — özel olay tetiklendi!");
        state.cursePercent = 0;
        OnCurseChanged?.Invoke(state.cursePercent);
        SaveState();
    }

    private void BroadcastFullRefresh()
    {
        // UI yenilemek için istersen event ekleyebilirsin
    }

    // === Save ===
    private string GetStateFileName()
    {
        string owner = currentPlayer != null ? currentPlayer.playerID : "Global";
        return $"SkillMapState_{owner}";
    }

    private void LoadOrCreateState()
    {
        if (currentMapSO == null)
        {
            state = null;
            return;
        }

        var loaded = SaveManager.Load<SkillMapState>(GetStateFileName());
        if (loaded != null)
        {
            state = loaded;
        }
        else
        {
            state = new SkillMapState();
            state.ownerPlayerID = currentPlayer != null ? currentPlayer.playerID : null;

            // Başlangıç skillerini otomatik aç
            foreach (var n in currentMapSO.skillNodes)
            {
                if (n != null && n.isStarter && n.skill != null)
                    state.unlocked.Add(n.skill.skillID);
            }

            SaveState();
        }
    }

    private void SaveState()
    {
        if (state == null) return;
        SaveManager.Save(state, GetStateFileName());
    }
}
