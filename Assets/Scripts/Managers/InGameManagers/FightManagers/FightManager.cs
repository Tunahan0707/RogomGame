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
        GameSceneManager.OnRoomEntered += EnterTheRoom;
    }

    private void OnDisable()
    {
        GameSceneManager.OnRoomEntered -= EnterTheRoom;
    }


    private void EnterTheRoom()
    {
        if (!loadedData.isNewSave)
            enemyManager.SpawnEnemyByID(loadedData.enemyID);
        else if (RandomRoomSelector.selectedRoom == RoomType.Fight)
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
