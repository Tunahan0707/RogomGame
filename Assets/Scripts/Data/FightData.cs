using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FightData
{
    public List<string> deck = new();
    public List<string> drawDeck = new();
    public List<string> hand = new();
    public List<string> discardPile = new();
    public int currentHP;
    public int coin;
    public int currentMana;
    public int currentRoomIndex;
    public int currentFloorIndex;
    public int currentEnemyHP;
    public int currentEnemyShield;
    public int currentPlayerShield;
    public int enemysCurrentPlan;
    public int enemysResistance;
    public int enemysStrenght;
    public int playersStrenght;
    public int playersResistance;
    public RoomType currentRoomType;
    public Turn turn;
    public string enemyID = "-1";
    public bool isNewSave = true;
}
