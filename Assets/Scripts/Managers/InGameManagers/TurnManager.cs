using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, Enemy , Off}
    public static Turn currentTurn = Turn.Player;
    public static bool isForStart = true;

    [Header("Managers")]
    public CardManager cardManager;
    public EnemyManager enemyManager;

    private FightData loadedData => FightDataHolder.Instance.fightData;


    [Header("UI")]
    public Button endTurnButton;

    public static Button endTurnButton1;

    private int plan;

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
        if (currentTurn == Turn.Enemy)
            StartEnemyTurn();
    }

    public void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        if (GameStartManager.currentGameType == GameType.NewGame && isForStart)
        {
            StartCoroutine(cardManager.DrawCards(cardManager.handSize));
            ManaManager.currentMana = ManaManager.maxMana;
            ManaManager.Equalize();
        }  
        if (!isForStart)
        {
            ManaManager.currentMana = ManaManager.maxMana;
            ManaManager.Equalize();
            StartCoroutine(cardManager.DrawCards(cardManager.handSize));
        }   
    }

    public void EndPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        endTurnButton1.gameObject.SetActive(false);
        cardManager.DiscardHand();
        isForStart = false;
        enemyManager.EndTurn();
        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;

        EnemyAlgoritmController ai = FindAnyObjectByType<EnemyAlgoritmController>();

        if (ai != null)
            yield return StartCoroutine(ai.ExecuteCurrentPlan());
        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        StartPlayerTurn();
        PlayerManager.Instance.NextTurn();
    }
}
