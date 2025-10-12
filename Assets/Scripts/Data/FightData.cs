using System.Collections.Generic;
using UnityEngine;

public class FightData
{
    public int coin;
    public float currentHP;
    public string enemyID;
    public int currentRoomIndex;
    public int currentFloorIndex;
    public RoomType currentRoomType;
    public List<string> deck;
    public List<string> inDrawDeckCardIDs;
    public List<string> inHandCardIDs;
    public List<string> inDiscardCardIDs;

    public FightData()
    {
        coin = 0;
        currentHP = PlayerDataHolder.Instance.playerData.maxHP;
        enemyID = "0";
        currentRoomIndex = 1;
        currentFloorIndex = 1;
        currentRoomType = RoomType.Fight;
        deck = new();
        inDrawDeckCardIDs = new();
        inHandCardIDs = new();
        inDiscardCardIDs = new();
    }
}
