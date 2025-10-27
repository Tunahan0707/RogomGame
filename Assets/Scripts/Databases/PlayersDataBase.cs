using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersDataBase", menuName = "ScriptableObjects/Databases/PlayersDataBase", order = 0)]
public class PlayersDataBase : ScriptableObject
{
    public List<PlayersSO> players;
    public PlayersSO startingPlayer;
    private void OnEnable()
    {
        players = new(Resources.LoadAll<PlayersSO>(Consts.FileWays.PlayersSO));
    }

    public PlayersSO GetPlayerByID(string id)
    {
        return players.Find(player => player.playerID == id);
    }

    public List<PlayersSO> UnlockLevel(int lvl)
    {
        return players.FindAll(player => player.unlockLevel == lvl);
    }
}
