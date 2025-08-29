using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemysSO : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public int health;
    public int damage;
    public float speed;
    public EnemyType enemyType;
    [Header("Enemy Abilities")]
    public List<string> specialAbilities = new();
}
