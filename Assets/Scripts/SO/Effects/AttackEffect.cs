using UnityEngine;

[CreateAssetMenu(fileName = "AttackonEnemy", menuName = "ScriptableObjects/CardEffects/Attack")]
public class AttackEffect : CardEffect
{
    public int damage;
    public override void Apply(EnemyAlgoritmController ai)
    {
        enemy.TakeDamage(damage + player.strenght);
    }

    public override string GetDescription()
    {
        if (player == null)
        {
            return $"Düşmana {damage} hasar ver";
        }
        if (player.strenght < 0)
            description = $"Düşmana <color=#FF5555>{(damage + player.strenght)}</color> hasar ver";
        else if (player.strenght > 0)
            description = $"Düşmana <color=#00CC66>{(damage + player.strenght)}</color> hasar ver";
        else
            description = $"{(damage + player.resistance)} kalkan kazan";
        return description;
    }
}
