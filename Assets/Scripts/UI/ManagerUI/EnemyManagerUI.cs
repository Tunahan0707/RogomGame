using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    public static HashSet<string> currentEnemys;

    private void Awake()
    {
        currentEnemys = new();
    }

    public void SpawnEnemy(EnemysSO enemy)
    {
        GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint, false);
        EnemyDisplay enemyDisplay = enemyGO.GetComponent<EnemyDisplay>();
        enemyDisplay.SetEnemyData(enemy);
        currentEnemys.Add(enemyDisplay.GetEnemyID());
    }
}
