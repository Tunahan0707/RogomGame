using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, Enemy }
    public Turn currentTurn = Turn.Player;

    [Header("Managers")]
    public CardManager cardManager;
    public EnemyManager enemyManager;

    [Header("UI")]
    public Button endTurnButton;

    private void Awake()
    {
        cardManager = FindObjectsByType<CardManager>(FindObjectsSortMode.None)[0];
        enemyManager = FindObjectsByType<EnemyManager>(FindObjectsSortMode.None)[0];
    }

    private void Start()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        Debug.Log("Player Turn Started");

        cardManager.DrawCards(cardManager.handSize);
    }

    public void EndPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        Debug.Log("Player Turn Ended");
        cardManager.DiscardHand();

        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;
        Debug.Log("Enemy Turn Started");

        // Basit enemy aksiyonu
        yield return new WaitForSeconds(1f);
        enemyManager.TakeDamage(enemyManager.currentEnemy.damage);

        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn Ended");
        StartPlayerTurn();
    }
}
