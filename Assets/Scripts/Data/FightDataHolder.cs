using UnityEngine;

public class FightDataHolder : MonoBehaviour
{
    public static FightDataHolder Instance { get; private set; }

    public FightData fightData;
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

    public void Equalize(EnemyAlgoritmController Ai)
    {
        ai = Ai;
    }
    public void SaveDatas()
    {
        ai.Save();
        CardManager.Instance.Save();
        EnemyManager.Instance.Save();
        PlayerManager.Instance.Save();
        fightData.currentRoomIndex = RandomRoomSelector.currentRoomIndex;
        fightData.currentFloorIndex = RandomRoomSelector.currentFloorIndex;
        fightData.currentRoomType = RandomRoomSelector.selectedRoom;
        fightData.currentMana = ManaManager.currentMana;
        fightData.coin = CoinManager.Coins;
        fightData.turn = TurnManager.currentTurn;
        Save();
    }
    private void Save()
    {
        fightData.isNewSave = false;
        SaveManager.Save<FightData>(fightData, Consts.FileNames.FightDataFile);
    }
}
