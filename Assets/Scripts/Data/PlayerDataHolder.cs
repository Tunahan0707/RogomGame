using System;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    private int xp;
    public static PlayerDataHolder Instance;
    public PlayerData playerData;

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
        playerData.maxHP = Mathf.RoundToInt(playerData.maxHP * 1.05f);
        SaveDatas();
    }

    public void SaveDatas()
    {
        playerData.xp = xp;
        Save();
    }
    public void Save()
    {
        SaveManager.Save<PlayerData>(playerData, Consts.FileNames.PlayerDataFile);
    }
}
