using UnityEngine;

[CreateAssetMenu(fileName = "DebuffEnemyStrenght", menuName = "ScriptableObjects/CardEffects/Debuff/DebuffEnemy/Strenght)")]
public class DebuffEnemysStrenght : CardEffect
{
    public int debuff;
    public override void Apply(EnemyAlgoritmController ai)
    {
        ai.DebuffStrenght(debuff);
    }

    public override string GetDescription()
    {
        description = $"Düşman {debuff} güç kaybeder";
        return description;
    }
}