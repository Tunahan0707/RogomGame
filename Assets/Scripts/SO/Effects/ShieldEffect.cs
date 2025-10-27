using UnityEngine;

[CreateAssetMenu(fileName = "GetShield", menuName = "ScriptableObjects/CardEffects/Shield")]
public class ShieldEffect : CardEffect
{
    public int shield;
    
    public override void Apply(EnemyAlgoritmController ai)
    {
        player.AddShield(shield);
    }

    public override string GetDescription()
    {
        if (player == null)
        {
            return $"{shield} kalkan kazan";
        }
        if (player.resistance < 0)
            description = $"<color=#FF5555>{(shield + player.resistance)}</color> kalkan kazan";
        else if (player.resistance > 0)
            description = $"<color=#00CC66>{(shield + player.resistance)}</color> kalkan kazan";
        else
            description = $"{(shield + player.resistance)} kalkan kazan";
        return description;
    }
}
