using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemysSO : ScriptableObject, IGameObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public int health = 100;
    public int damage;
    public int baseShield = 0;
    public int addingShield = 6;
    public int baseStrenght = 0;
    public int addingStrenght;
    public int baseResistance = 0;
    public int addingResistnace;
    public Sprite artwork;
    public EnemyType enemyType;
    public bool isLocked;
    public int unlockLevel;

    [Header("Enemy Abilities")]
    public List<string> specialAbilities = new();

    [Header("Enemy Rewards")]
    public int coinGive;
    public int experienceGive;

    [HideInInspector] public string enemyID;

    public string Name { get => enemyName; set => enemyName = value; }
    public int UnlockLevel { get => unlockLevel; set => unlockLevel = value; }
    public string ID { get => enemyID; set => enemyID = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }


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
