using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSceneManager : MonoBehaviour
{
    [SerializeField] private PlayersDataBase playersDB;
    [SerializeField] private PlayerManagerUI playerManagerUI;
    public static PlayersSO currentPlayer;

    private void OnEnable()
    {
        PlayerDisplay.OnPlayerSelected += PlayerChanced;
    }

    private void PlayerChanced(PlayersSO player)
    {
        currentPlayer = player;
        PlayerDataHolder.Instance.playerData.currentPlayerID = player.playerID;
        SceneManager.LoadScene(Consts.Scenes.MAIN_MENU);
        if (currentPlayer != null)
            PlayerDataHolder.Instance.SaveDatas();
    }

    private void Start()
    {
        foreach (PlayersSO player in playersDB.players)
        {
            playerManagerUI.SpawnPlayerForCharacterScene(player);
        }
    }
}
