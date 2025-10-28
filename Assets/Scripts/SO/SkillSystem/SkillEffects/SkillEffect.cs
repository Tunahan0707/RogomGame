using UnityEngine;

public abstract class SkillEffect: ScriptableObject
{
    public CoinManager coinManager => CoinManager.Instance;
    public ManaManager manaManager => ManaManager.Instance;
    public CardManager cardManager => CardManager.Instance;
    public PlayerManager player => PlayerManager.Instance;
    public EnemyManager enemy => EnemyManager.Instance;
    public string description;
    public abstract void Apply(EnemyAlgoritmController ai);
    public abstract string GetDescription();
}
