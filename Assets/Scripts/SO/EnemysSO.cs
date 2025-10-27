using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemysSO : ScriptableObject, IUnlockable
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
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }

    [Header("Enemy Abilities")]
    public List<string> specialAbilities = new();

    [Header("Enemy Rewards")]
    public int coinGive;
    public int experienceGive;

    [HideInInspector] public string enemyID;

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
