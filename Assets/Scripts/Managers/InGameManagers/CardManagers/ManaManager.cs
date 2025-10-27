using System;
using TMPro;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    public static event Action OnManaSpent;
    public static int currentMana;
    public static int maxMana;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private PlayersDataBase pDB;
    private static TextMeshProUGUI ManaText;
    private FightData loadedData => FightDataHolder.Instance.fightData;
    private PlayerData playerData => PlayerDataHolder.Instance.playerData;

    private void Awake()
    {
        ManaText = manaText;
        maxMana = pDB.GetPlayerByID(playerData.currentPlayerID).maxMana + playerData.extraMana;
        if (loadedData.isNewSave)
            currentMana = maxMana;
        else
            currentMana = loadedData.currentMana;
    }
    private void Start()
    {
        Equalize();
    }
    public static void Equalize()
    {
        ManaText.text = currentMana.ToString() + "/" + maxMana.ToString();
        OnManaSpent?.Invoke();
    }
}
