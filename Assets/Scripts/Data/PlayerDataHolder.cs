using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    private int xp;
    public static PlayerDataHolder Instance;
    public PlayerData playerData;
    [SerializeField] private CardsDataBase cDB;
    [SerializeField] private PlayersDataBase pDB;
    [SerializeField] private OzsDataBase oDB;
    [SerializeField] private EnemiesDataBase eDB;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Eğer save varsa yükle
        var data = SaveManager.Load<PlayerData>(Consts.FileNames.PlayerDataFile);
        playerData = data ?? new PlayerData();
    }
    private void OnEnable()
    {
        PlayerProgress.OnPlayerLevelUp += OnLevelUp; // 🔥 Event’e abone ol
    }

    private void OnDisable()
    {
        PlayerProgress.OnPlayerLevelUp -= OnLevelUp;
    }

    public void OnLevelUp(int newLevel)
    {
        playerData.extraHP += newLevel * 0.6f + 5;
        playerData.extraMana += Mathf.FloorToInt((newLevel + 1) * 0.1f);
        UnlockALL(newLevel);
        SaveDatas();
        Debug.Log($"📈 Oyuncu seviye atladı: {newLevel}");
    }

    private void UnlockALL(int level)
    {
        Unlock(oDB.UnlockLevel(level));
        Unlock(pDB.UnlockLevel(level));
        Unlock(eDB.UnlockLevel(level));
        Unlock(cDB.UnlockLevel(level));
    }

    private void Unlock<T>(List<T> list) where T : IUnlockable
    {
        if (list == null) return;
        foreach (var item in list)
            item.IsLocked = false;
    }


    public void SaveDatas()
    {
        playerData.PUAN = PUANManager.puan;
        Save();
    }
    public void Save()
    {
        if (playerData.currentPlayerID == null)
            playerData.currentPlayerID = pDB.startingPlayer.playerID;
        SaveManager.Save<PlayerData>(playerData, Consts.FileNames.PlayerDataFile);
    }
}
