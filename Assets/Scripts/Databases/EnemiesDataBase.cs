using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesDataBase", menuName = "ScriptableObjects/Databases/EnemiesDataBase", order = 1)]
public class EnemiesDataBase : ScriptableObject
{
    [Header("Enemy Data")]
    public List<EnemysSO> enemies;
    public List<EnemysSO> normalEnemies;
    public List<EnemysSO> miniBossEnemies;
    public List<EnemysSO> bossEnemies;
    public List<EnemysSO> specialEnemies;

    private void OnEnable()
    {
        enemies = new(Resources.LoadAll<EnemysSO>(Consts.FileWays.EnemiesSO));
        normalEnemies = new();
        miniBossEnemies = new();
        bossEnemies = new();
        specialEnemies = new();
        foreach (var enemy in enemies)
        {
            switch (enemy.enemyType)
            {
                case EnemyType.Normal: normalEnemies.Add(enemy); break;
                case EnemyType.MiniBoss: miniBossEnemies.Add(enemy); break;
                case EnemyType.Boss: bossEnemies.Add(enemy); break;
                case EnemyType.Special: specialEnemies.Add(enemy); break;
            }
        }
    }

    public EnemysSO GetEnemyByType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Normal => normalEnemies[Random.Range(0, normalEnemies.Count)],
            EnemyType.MiniBoss => miniBossEnemies[Random.Range(0, miniBossEnemies.Count)],
            EnemyType.Boss => bossEnemies[Random.Range(0, bossEnemies.Count)],
            EnemyType.Special => specialEnemies[Random.Range(0, specialEnemies.Count)],
            _ => null,
        };
    }
    public EnemysSO GetEnemyByID(string id)
    {
        return enemies.Find(enemy => enemy.enemyID == id);
    }

    public List<EnemysSO> UnlockLevel(int lvl)
    {
        return enemies.FindAll(enemy => enemy.unlockLevel == lvl);
    }
}
