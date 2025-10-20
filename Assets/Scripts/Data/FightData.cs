using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FightData
{
    public int coin;
    public float currentHP;
    public string enemyID;
    public int currentEnemyHP;
    public int currentRoomIndex;
    public int currentFloorIndex;
    public int currentEnemyShield;
    public int currentPlayerShield;
    public int currentMana;
    public int enemysStrenght;
    public int enemysResistance;
    public int playersStrenght;
    public int playersResistance;
    public TurnManager.Turn turn;
    public RoomType currentRoomType;
    public List<string> deck;
    public List<string> hand;
    public List<string> discardPile;
    public List<string> drawDeck;

    public FightData()
    {
        coin = 0;
        enemyID = "0";
        enemysStrenght = 96963169;
        playersStrenght = 96963169;
        currentHP = 96963169;
        currentRoomIndex = 1;
        currentFloorIndex = 1;
        turn = TurnManager.Turn.Player;
        currentRoomType = RoomType.Fight;
        deck = new();
        hand = new();
        discardPile = new();
        drawDeck = new();
    }
}
