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
        XPManager.OnXPChanged += (x) => xp = x;
        XPManager.OnPlayerLevelUp += LevelUp;
    }

    private void LevelUp()
    {
        playerData.level++;
        playerData.extraHP += playerData.extraHP * playerData.level * 0.6f + 5;
        UnlockALL();
        SaveDatas();
    }

    private void UnlockALL()
    {
        Unlock(oDB.UnlockLevel(playerData.level));
        Unlock(pDB.UnlockLevel(playerData.level));
        Unlock(eDB.UnlockLevel(playerData.level));
        Unlock(cDB.UnlockLevel(playerData.level));
    }

    private void Unlock<T>(List<T> y) where T : IGameObject
    {
        if (y == null) return;
        foreach (var x in y)
        {
            if (x.IsLocked)
                x.IsLocked = false;
        }
    }
    public void SaveDatas()
    {
        playerData.xp = xp;
        if (CharacterSceneManager.currentPlayer != null)
            playerData.currentPlayerID = CharacterSceneManager.currentPlayer.playerID;
        if (playerData.currentPlayerID == null)
            playerData.currentPlayerID = pDB.startingPlayer.playerID;
        playerData.maxHP = Mathf.RoundToInt(pDB.GetPlayerByID(playerData.currentPlayerID).health + playerData.extraHP);
        Save();
    }
    public void Save()
    {
        SaveManager.Save<PlayerData>(playerData, Consts.FileNames.PlayerDataFile);
    }
}
