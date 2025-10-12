using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemysSO : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public int health;
    public int damage;
    public Sprite artwork;
    public EnemyType enemyType;
    [Header("Enemy Abilities")]
    public List<string> specialAbilities = new();
    [Header("Enemy Rewards")]
    public int coinGive;
    public int experienceGive;
    public string enemyID;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
