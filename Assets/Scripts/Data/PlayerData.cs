using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string playerID;
    public int level;
    public int xp;
    public float maxHP;
    public float extraHP;
    public string currentPlayerID;

    public PlayerData()
    {
        playerID = Guid.NewGuid().ToString();
        level = 1;
        xp = 0;
        maxHP = 100;
        extraHP = 0;
    }
}
