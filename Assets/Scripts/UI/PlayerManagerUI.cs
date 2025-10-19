using UnityEngine;

public class PlayerManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;


    public void SpawnPlayer(PlayersSO player)
    {
        GameObject playerGO = Instantiate(playerPrefab);
        PlayerDisplay playerDisplay = playerGO.GetComponent<PlayerDisplay>();
        playerDisplay.SetPlayer(player);
        playerGO.transform.SetParent(spawnPoint);
        PlayerManager.playerDisplay = playerDisplay;
    }
    public void SpawnPlayerForCharacterScene(PlayersSO player)
    {
        GameObject playerGO = Instantiate(playerPrefab);
        PlayerDisplay playerDisplay = playerGO.GetComponent<PlayerDisplay>();
        playerDisplay.SetPlayer(player);
        playerGO.transform.SetParent(spawnPoint);
        playerDisplay.DestroyOthers();
    }
}
