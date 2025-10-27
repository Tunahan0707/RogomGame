using UnityEngine;

public class CardPlayingController : MonoBehaviour
{
    private EnemyManager enemyManager => EnemyManager.Instance;
    private PlayerManager playerManager => PlayerManager.Instance;
    private EnemyAlgoritmController ai;

    private void OnEnable()
    {
        CardDisplay.OnCardClicked += OnCardPlayed;
        EnemyManager.OnEnemySelected += (AI) => ai = AI;
    }

    private void OnDisable()
    {
        CardDisplay.OnCardClicked -= OnCardPlayed;
    }

    private void OnCardPlayed(CardDisplay cardDisplay)
    {
        if (TurnManager.currentTurn != Turn.Player) return;

        var card = cardDisplay.cardData;
        if (ManaManager.currentMana < card.cost) return;

        ManaManager.currentMana -= card.cost;
        ManaManager.Equalize();
        if (card.isUpgradedVersion)
        {
            foreach (var effect in card.upgradedEffects)
                effect.Apply(ai);
        }
        else
        {
            foreach (var effect in card.effects)
                effect.Apply(ai);
        }    
        CardZoneManager.SetZone(cardDisplay.GetCardID(), CardZone.Discard);
        CardManagerUI.Instance.MoveCard(cardDisplay.GetCardID(), CardZone.Discard);

        ai?.UpdateCurrentPlan();
    }
}
