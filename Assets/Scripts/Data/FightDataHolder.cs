using UnityEngine;

public class FightDataHolder : MonoBehaviour
{
    public static FightDataHolder Instance { get; private set; }

    public FightData fightData;
    private EnemyManager enemyManager;
    private CardManager cardManager;
    private EnemyAlgoritmController ai;
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

    private void OnEnable()
    {
        EnemyManager.OnEnemySelected += Equalize;
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemySelected -= Equalize;
    }

    public void Equalize(EnemysSO sO)
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        cardManager = FindFirstObjectByType<CardManager>();
        ai = enemyManager.enemyDisplay.gameObject.GetComponent<EnemyAlgoritmController>();
    }
    public void SaveDatas()
    {
        fightData.enemysStrenght = ai.strenght;
        fightData.playersStrenght = PlayerManager.Instance.strenght;
        fightData.enemysResistance = ai.resistance;
        fightData.playersResistance = PlayerManager.Instance.resistance;
        fightData.enemyID = enemyManager.GetEnemyID();
        fightData.currentRoomIndex = RandomRoomSelector.currentRoomIndex;
        fightData.currentFloorIndex = RandomRoomSelector.currentFloorIndex;
        fightData.currentRoomType = RandomRoomSelector.selectedRoom;
        fightData.currentEnemyHP = enemyManager.GetEnemyHealth();
        fightData.currentEnemyShield = enemyManager.GetEnemyShield();
        fightData.currentHP = PlayerManager.Instance.playerHealth;
        fightData.coin = CoinManager.GetCurrentCoins();
        fightData.turn = TurnManager.currentTurn;
        fightData.currentMana = ManaManager.currentMana;
        fightData.currentPlayerShield = PlayerManager.Instance.shield;
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
