using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;

    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void Awake()
    {
        if (enemyManager == null)
            enemyManager = FindAnyObjectByType<EnemyManager>();
    }
    private void OnEnable()
    {
        GameSceneManager.OnContinueButtonClicked += HandleContinueButtonClicked;
    }

    private void OnDisable()
    {
        GameSceneManager.OnContinueButtonClicked -= HandleContinueButtonClicked;
    }

    private void Start()
    {
        if (GameStartManager.currentGameType == GameType.NewGame)
            enemyManager.SelectEnemy(EnemyType.Normal);
        else if (GameStartManager.currentGameType == GameType.ContinueGame)
            enemyManager.SpawnEnemyByID(loadedData.enemyID);
    }

    private void HandleContinueButtonClicked()
    {
        if (RandomRoomSelector.selectedRoom == RoomType.Fight)
        {
            enemyManager.SelectEnemy(EnemyType.Normal);
        }
        else if (RandomRoomSelector.selectedRoom == RoomType.MiniBoss)
        {
            enemyManager.SelectEnemy(EnemyType.MiniBoss);
        }
        else if (RandomRoomSelector.selectedRoom == RoomType.Boss)
        {
            enemyManager.SelectEnemy(EnemyType.Boss);
        }
    }
}
