using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public static GameType currentGameType;
    PlayerData loadedData;
    private void Awake()
    {
        if (!SaveManager.SaveExists(Consts.FileNames.PlayerDataFile))
        {
            loadedData = new();
            return;
        }
        loadedData = SaveManager.Load<PlayerData>(Consts.FileNames.PlayerDataFile);
    }
    public void NewGame()
    {
        SaveManager.DeleteSave(Consts.FileNames.FightDataFile);
        FightData newFightData = new();

        // Başlangıç destesi ekle
        var cardsDB = Resources.Load<CardsDataBase>(Consts.FileWays.CardsDB);
        foreach (var card in cardsDB.startingCards)
        {
            for (int i = 0; i < card.howManyHaveOnStart; i++)
            {
                newFightData.deck.Add(card.cardID);
            }
        }
        PlayersSO player;
        if (loadedData.currentPlayerID != null)
            player = PlayersDataBase.Instance.GetPlayerByID(loadedData.currentPlayerID);
        else
            player = PlayersDataBase.Instance.startingPlayer;
        foreach (var card in player.extraStartingCards)
        {
            newFightData.deck.Add(card.cardID);
        }
        

        // Kaydet
        SaveManager.Save(newFightData, Consts.FileNames.FightDataFile);

        // Runtime holder varsa güncelle
        var holder = FindFirstObjectByType<FightDataHolder>();
        if (holder != null)
            holder.fightData = newFightData;

        SceneManager.LoadScene(Consts.Scenes.GAME);
        currentGameType = GameType.NewGame;
    }

    public void ContinueGame()
    {
        if (!SaveManager.AllSavesExists())
        {
            Debug.LogWarning("Kayıt yok, yeni oyun başlatılıyor.");
            NewGame();
            return;
        }

        // Kaydı yükle
        FightData loadedFightData = SaveManager.Load<FightData>(Consts.FileNames.FightDataFile);

        var holder = FindFirstObjectByType<PlayerDataHolder>();
        if (holder != null)
            holder.playerData = loadedData;
        var fightHolder = FindFirstObjectByType<FightDataHolder>();
        if (fightHolder != null)
            fightHolder.fightData = loadedFightData;
        SceneManager.LoadScene(Consts.Scenes.GAME);
        currentGameType = GameType.ContinueGame;
    }
}
