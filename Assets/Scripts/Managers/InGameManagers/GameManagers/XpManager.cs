using System;
using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance;

    [Header("Oyuncu Genel XP")]
    [SerializeField] private PlayerProgress playerProgress = new();

    [Header("Karakterler")]
    [SerializeField] private List<CharacterProgress> characterList = new();
    private Dictionary<string, CharacterProgress> characterMap = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        foreach (var c in characterList)
        {
            if (c != null && !string.IsNullOrEmpty(c.characterID) && !characterMap.ContainsKey(c.characterID))
                characterMap[c.characterID] = c;
        }
    }

    // --- Oyuncu XP ---
    public void AddPlayerXP(int amount)
    {
        playerProgress.AddXP(amount);
        SaveXPData();
    }

    public int GetPlayerLevel() => playerProgress.level;
    public int GetPlayerXP() => playerProgress.currentXP;
    public int GetPlayerNextXP() => playerProgress.xpToNextLevel;

    // --- Karakter XP ---
    public void RegisterCharacter(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (characterMap.ContainsKey(id)) return;

        var newChar = new CharacterProgress(id);
        characterList.Add(newChar);
        characterMap[id] = newChar;
    }

    public void AddCharacterXP(string id, int amount)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (!characterMap.ContainsKey(id)) RegisterCharacter(id);

        var data = characterMap[id];
        data.AddXP(amount);
        SaveXPData();
    }

    public CharacterProgress GetCharacterData(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (characterMap.TryGetValue(id, out var data)) return data;
        return null;
    }

    // --- Kayıt Sistemi ---
    private void SaveXPData()
    {
        SaveManager.Save(this, Consts.FileNames.XPDataFile);
    }

    public void LoadXPData()
    {
        var loaded = SaveManager.Load<XPManager>(Consts.FileNames.XPDataFile);
        if (loaded != null)
        {
            playerProgress = loaded.playerProgress;
            characterList = loaded.characterList;

            characterMap.Clear();
            foreach (var c in characterList)
            {
                if (c != null && !string.IsNullOrEmpty(c.characterID))
                    characterMap[c.characterID] = c;
            }
        }
    }
}
