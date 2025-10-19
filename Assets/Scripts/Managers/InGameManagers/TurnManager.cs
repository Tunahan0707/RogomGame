using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, Enemy }
    public static Turn currentTurn = Turn.Player;
    public static bool isForStart = true;

    [Header("Managers")]
    public CardManager cardManager;
    public EnemyManager enemyManager;

    private FightData loadedData => FightDataHolder.Instance.fightData;


    [Header("UI")]
    public Button endTurnButton;

    public static Button endTurnButton1;

    private int x;

    private void Awake()
    {
        cardManager = FindObjectsByType<CardManager>(FindObjectsSortMode.None)[0];
        enemyManager = FindObjectsByType<EnemyManager>(FindObjectsSortMode.None)[0];
        endTurnButton1 = endTurnButton;
    }

    private void Start()
    {
        endTurnButton1.onClick.AddListener(EndPlayerTurn);
        isForStart = true;
        currentTurn = loadedData.turn;
        if (currentTurn == Turn.Player)
            StartPlayerTurn();
        else
            StartEnemyTurn();
    }

    public void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        if (GameStartManager.currentGameType == GameType.NewGame && isForStart)
            StartCoroutine(cardManager.DrawCards(cardManager.handSize));
        if (!isForStart)
            StartCoroutine(cardManager.DrawCards(cardManager.handSize));
        x = UnityEngine.Random.Range(0, 7);
    }

    public void EndPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        endTurnButton1.gameObject.SetActive(false);
        cardManager.DiscardHand();
        isForStart = false;
        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;

        yield return new WaitForSeconds(1f);

        enemyManager.EnemyAlgoritm(x);
        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        StartPlayerTurn();
    }
}
