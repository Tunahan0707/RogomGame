using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum Turn { Player, Enemy, Off }

public class TurnManager : MonoBehaviour
{

    public static Turn currentTurn = Turn.Player;
    public static bool isForStart = true;

    public static TurnManager Instance;

    private FightData loadedData => FightDataHolder.Instance.fightData;

    [Header("UI")]
    public Button endTurnButton;


    private EnemyAlgoritmController ai;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        EnemyManager.OnEnemySelected += (AI) =>
        {
            ai = AI;
            if (currentTurn == Turn.Enemy)
            StartCoroutine(StartEnemyTurn());
        };
        EnemyManager.OnEnemyDied += () => endTurnButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => StartCoroutine(EndPlayerTurn()));

        isForStart = true;
        if (!loadedData.isNewSave)
            currentTurn = loadedData.turn;

        endTurnButton.gameObject.SetActive(currentTurn == Turn.Player);

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
        endTurnButton.gameObject.SetActive(true);

        isForStart = false;
        currentTurn = Turn.Player;
    }


    public IEnumerator EndPlayerTurn()
    {
        if (currentTurn != Turn.Player)
            yield break;

        Debug.Log("Oyuncu turu bitti");
        endTurnButton.gameObject.SetActive(false);

        yield return StartCoroutine(CardManager.Instance.DiscardHand());

        currentTurn = Turn.Enemy;
        yield return StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        Debug.Log("Düşman turu başladı");

        currentTurn = Turn.Enemy;

        if (ai != null)
            yield return StartCoroutine(ai.ExecuteCurrentPlan());

        yield return new WaitForSeconds(0.5f); // küçük geçiş gecikmesi
        yield return StartCoroutine(StartPlayerTurn());

        PlayerManager.Instance.NextTurn();
    }
}
