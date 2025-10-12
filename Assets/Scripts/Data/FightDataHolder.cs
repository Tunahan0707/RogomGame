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
        while (true)
        {
            enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
                break;
            cardManager = FindFirstObjectByType<CardManager>();
            if (cardManager != null)
                break;
        }
    }

    public void SaveDatas()
    {
        fightData.enemyID = enemyManager.GetEnemyID();
        fightData.currentRoomIndex = RandomRoomSelector.currentRoomIndex;
        fightData.currentFloorIndex = RandomRoomSelector.currentFloorIndex;
        fightData.currentRoomType = RandomRoomSelector.selectedRoom;
        fightData.currentHP = HealthManager.GetHealth();
        fightData.coin = CoinManager.GetCurrentCoins();
        fightData.deck = cardManager.GetCurrent(Consts.ListNames.DECK);
        fightData.inDrawDeckCardIDs = cardManager.GetCurrent(Consts.ListNames.DRAW_DECK);
        fightData.inHandCardIDs = cardManager.GetCurrent(Consts.ListNames.HAND);
        fightData.inDiscardCardIDs = cardManager.GetCurrent(Consts.ListNames.DISCARD_PILE);
        Save();
    }
    private void Save()
    {
        SaveManager.Save<FightData>(fightData, Consts.FileNames.FightDataFile);
    }
}
