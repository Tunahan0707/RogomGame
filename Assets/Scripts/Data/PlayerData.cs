using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string playerID;
    public int PUAN;
    public float extraHP;
    public int extraMana;
    public string currentPlayerID;

    public PlayerData()
    {
        playerID = Guid.NewGuid().ToString();
        extraHP = 0;
        PUAN = 0;
    }
}
