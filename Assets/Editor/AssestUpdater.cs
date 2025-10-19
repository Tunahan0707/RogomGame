using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;

public class DatabaseAutoUpdater : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Dosya değişikliklerinde kontrol et
        if (ShouldUpdate(importedAssets) || ShouldUpdate(deletedAssets) || ShouldUpdate(movedAssets))
        {
            UpdateAllDatabases();
        }

        if (movedFromAssetPaths is null)
        {
            throw new System.ArgumentNullException(nameof(movedFromAssetPaths));
        }
    }

    private static bool ShouldUpdate(string[] paths)
    {
        // Resources içindeki ScriptableObject klasörlerini kontrol eder
        return paths.Any(path =>
            path.Contains("Resources/" + Consts.FileWays.CardsSO) ||
            path.Contains("Resources/" + Consts.FileWays.EnemiesSO) ||
            path.Contains("Resources/" + Consts.FileWays.OzsSO) ||
            path.Contains("Resources/"+ Consts.FileWays.PlayersSO));
    }

    private static void UpdateAllDatabases()
    {
        UpdateDatabase<CardsSO, CardsDataBase>(Consts.FileWays.CardsDB, Consts.FileWays.CardsSO);
        UpdateDatabase<EnemysSO, EnemiesDataBase>(Consts.FileWays.EnemiesDB, Consts.FileWays.EnemiesSO);
        UpdateDatabase<OzsSO, OzsDataBase>(Consts.FileWays.OzsDB, Consts.FileWays.OzsSO);
        UpdateDatabase<PlayersSO, PlayersDataBase>(Consts.FileWays.PlayersDB, Consts.FileWays.PlayersSO);
    }

    private static void UpdateDatabase<TSO, TDB>(string dbPath, string soFolder)
        where TSO : ScriptableObject
        where TDB : ScriptableObject
    {
        // Database’i Resources içinden yükle
        var database = Resources.Load<TDB>(dbPath);
        if (database == null)
        {
            Debug.LogWarning($"Database bulunamadı: {dbPath}");
            return;
        }

        // Yeni verileri topla
        var allData = new List<TSO>(Resources.LoadAll<TSO>(soFolder));

        // İlgili listeyi reflection ile bul
        var field = typeof(TDB).GetFields()
            .FirstOrDefault(f => f.FieldType == typeof(List<TSO>));

        if (field == null)
        {
            Debug.LogWarning($"{typeof(TDB).Name} içinde {typeof(TSO).Name} listesi yok.");
            return;
        }

        // Listeyi güncelle
        field.SetValue(database, allData);

        // Database’i kaydet
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"{typeof(TDB).Name} otomatik güncellendi → {allData.Count} kayıt bulundu.");
    }
}
