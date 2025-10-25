using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public enum Turn { Player, Enemy, Off }

public class TurnManager : MonoBehaviour
{
    public static event Action OnPlayerTurnStarted;
    public static event Action OnEnemyTurnStarted;

    public static Turn currentTurn = Turn.Player;
    public static bool isForStart = true;


    private FightData loadedData => FightDataHolder.Instance.fightData;


    [Header("UI")]
    public Button endTurnButton;

    public static Button endTurnButton1;

    private EnemyAlgoritmController ai;

    public static void TriggerPlayerTurnStart() => OnPlayerTurnStarted?.Invoke();
    public static void TriggerEnemyTurnStart() => OnEnemyTurnStarted?.Invoke();


    private void Awake()
    {
        endTurnButton1 = endTurnButton;
    }
    private void OnEnable()
    {
        OnPlayerTurnStarted += StartPlayerCoroutine;
        OnEnemyTurnStarted += StartEnemyCoroutine;
        EnemyManager.OnEnemySelected += (AI) => ai = AI;
    }
    private void OnDisable()
    {
        OnPlayerTurnStarted -= StartPlayerCoroutine;
        OnEnemyTurnStarted -= StartEnemyCoroutine;
    }

    private void StartPlayerCoroutine()
    {
        StartCoroutine(StartPlayerTurn());
    }

    private void StartEnemyCoroutine()
    {
        StartCoroutine(StartEnemyTurn());
    }

    private void Start()
    {
        endTurnButton1.onClick.AddListener(EndPlayerTurn);
        isForStart = true;
        currentTurn = loadedData.turn;
        if (currentTurn == Turn.Enemy)
            StartEnemyCoroutine();
    }

    public IEnumerator StartPlayerTurn()
    {
        if (loadedData.isNewSave && isForStart)
        {
            ManaManager.currentMana = ManaManager.maxMana;
            ManaManager.Equalize();
            yield return StartCoroutine(CardManager.Instance.DrawCards(CardManager.Instance.handSize));
        }  
        if (!isForStart)
        {
            ManaManager.currentMana = ManaManager.maxMana;
            ManaManager.Equalize();
            yield return StartCoroutine(CardManager.Instance.DrawCards(CardManager.Instance.handSize));
        }
        currentTurn = Turn.Player;   
    }

    public void EndPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        endTurnButton1.gameObject.SetActive(false);
        CardManager.Instance.DiscardHand();
        if (isForStart)
            isForStart = false;
        EnemyManager.Instance.EndTurn();
        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;
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
