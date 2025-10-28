using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    [HideInInspector]
    public string description;
    public PlayerManager player => PlayerManager.Instance;
    public EnemyManager enemy => EnemyManager.Instance;

    public abstract void Apply(EnemyAlgoritmController ai);
    public abstract string GetDescription();
}
