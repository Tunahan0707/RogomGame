using UnityEngine;

[CreateAssetMenu(fileName = "GetHeal", menuName = "ScriptableObjects/CardEffects/Buff/BuffPlayer/Heal")]
public class HealEffect : CardEffect
{
    public int heal;
    public override void Apply(EnemyAlgoritmController ai)
    {
        player.Heal(heal);
    }
    public override string GetDescription()
    {
        description = (heal).ToString() + " puan iyileş";
        return description;
    }
}
