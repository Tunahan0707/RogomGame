using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    void OnEnable()
    {
        GameSceneManager.OnContinueButtonClicked += HandleContinueButtonClicked;
    }

    void OnDisable()
    {
        GameSceneManager.OnContinueButtonClicked -= HandleContinueButtonClicked;
    }

    void Start()
    {
        enemyManager.SelectEnemy(EnemyType.Normal);
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
