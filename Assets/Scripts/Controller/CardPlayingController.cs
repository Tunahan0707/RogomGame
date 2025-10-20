using System;
using UnityEngine;

public class CardPlayingController : MonoBehaviour
{
    private EnemyManager enemyManager;
    private PlayerManager playerManager;
    private EnemyAlgoritmController ai;


    private void OnEnable()
    {
        CardDisplay.OnCardClicked += CardPlayed;
        EnemyManager.OnEnemySelected += Equalize;
    }
    private void OnDisable()
    {
        CardDisplay.OnCardClicked -= CardPlayed;
        EnemyManager.OnEnemySelected -= Equalize;
    }

    private void Equalize(EnemyAlgoritmController Ai)
    {
        if (enemyManager == null)
            enemyManager = FindFirstObjectByType<EnemyManager>();
        if (playerManager == null)
            playerManager = FindFirstObjectByType<PlayerManager>();
        ai = Ai;
    }

    private void CardPlayed(CardDisplay cardDisplay)
    {
        if (cardDisplay.cardData.cost != 0)
        {
            if (ManaManager.currentMana < cardDisplay.cardData.cost) return;
            ManaManager.currentMana -= cardDisplay.cardData.cost; 
        }
        ManaManager.Equalize();
        cardDisplay.PlayCard();
        ai.UpdateCurrentPlan();   
        if (cardDisplay.cardData.attackValue != 0)
            enemyManager.TakeDamage(cardDisplay.cardData.attackValue + playerManager.strenght);
        if (cardDisplay.cardData.shieldValue != 0)
            playerManager.AddShield(cardDisplay.cardData.shieldValue);
        if (cardDisplay.cardData.healValue != 0)
            playerManager.Heal(cardDisplay.cardData.healValue);
        if (cardDisplay.cardData.addingResistnace != 0)
            playerManager.BuffResistance(cardDisplay.cardData.addingResistnace);
        if (cardDisplay.cardData.addingStrenght != 0)
            playerManager.BuffStrenght(cardDisplay.cardData.addingStrenght);
        if (cardDisplay.cardData.debuffingEnemysResistance != 0)
            ai.DebuffResistance(cardDisplay.cardData.debuffingEnemysResistance);
        if (cardDisplay.cardData.debuffingEnemysStrenght != 0)
            ai.DebuffStrenght(cardDisplay.cardData.debuffingEnemysStrenght);
        // if (cardDisplay.cardData.cardDisplay.cardDataName == Consts.CardNames.)
    }
}
