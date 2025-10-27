using UnityEngine;

[CreateAssetMenu(fileName = "GetStrenght", menuName = "ScriptableObjects/CardEffects/Buff/BuffPlayer/Strenght")]
public class GetStrenght : CardEffect
{
    public int getStrenght;
    public override void Apply(EnemyAlgoritmController ai)
    {
        player.BuffStrenght(getStrenght);
    }

    public override string GetDescription()
    {
        description = $"{getStrenght} güç kazan";
        return description;
    }
}