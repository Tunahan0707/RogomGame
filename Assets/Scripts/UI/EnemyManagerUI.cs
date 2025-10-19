using Unity.VisualScripting;
using UnityEngine;

public class EnemyManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;


    public void SpawnEnemy(EnemysSO enemy)
    {
        GameObject enemyGO = Instantiate(enemyPrefab);
        EnemyDisplay enemyDisplay = enemyGO.GetComponent<EnemyDisplay>();
        enemyDisplay.SetEnemyData(enemy);
        enemyGO.transform.SetParent(spawnPoint);
    }
}
