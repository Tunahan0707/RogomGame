using UnityEngine;

public class FightDataHolder : MonoBehaviour
{
    public static FightDataHolder Instance { get; private set; }

    public FightData fightData;
    private EnemyManager enemyManager;
    private CardManager cardManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        var data = SaveManager.Load<FightData>(Consts.FileNames.FightDataFile);
        fightData = data ?? new FightData();
    }
    public void Equalize()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        cardManager = FindFirstObjectByType<CardManager>();
    }
    public void SaveDatas()
    {
        fightData.enemyID = enemyManager.GetEnemyID();
        fightData.currentRoomIndex = RandomRoomSelector.currentRoomIndex;
        fightData.currentFloorIndex = RandomRoomSelector.currentFloorIndex;
        fightData.currentRoomType = RandomRoomSelector.selectedRoom;
        fightData.currentEnemyHP = enemyManager.GetEnemyHealth();
        fightData.currentEnemyShield = enemyManager.GetEnemyShield();
        fightData.currentHP = PlayerManager.playerHealth;
        fightData.coin = CoinManager.GetCurrentCoins();
        fightData.turn = TurnManager.currentTurn;
        fightData.deck = cardManager.GetCurrent(Consts.ListNames.DECK);
        fightData.drawDeck = cardManager.GetCurrent(Consts.ListNames.DRAW_DECK);
        fightData.discardPile = cardManager.GetCurrent(Consts.ListNames.DISCARD_PILE);
        fightData.hand = cardManager.GetCurrent(Consts.ListNames.HAND);
        Save();
    }
    private void Save()
    {
        SaveManager.Save<FightData>(fightData, Consts.FileNames.FightDataFile);
    }
}
