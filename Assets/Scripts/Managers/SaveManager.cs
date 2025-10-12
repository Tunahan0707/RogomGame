using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName + ".json");

    // Dosya adları
    private static readonly string PlayerDataFile = "PlayerData";
    private static readonly string FightDataFile = "FightData";

    // Generic Save
    public static void Save<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(fileName), json);
        Debug.Log($"💾 Kaydedildi: {fileName}");
    }

    // Generic Load
    public static T Load<T>(string fileName) where T : class
    {
        string path = GetPath(fileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"📂 {fileName} kaydı bulunamadı.");
            return null;
        }
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    // Delete tek dosya
    public static void DeleteSave(string fileName)
    {
        string path = GetPath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"🗑️ {fileName} kaydı silindi.");
        }
    }

    // Delete hem PlayerData hem FightData
    public static void DeleteAllSaves()
    {
        DeleteSave(PlayerDataFile);
        DeleteSave(FightDataFile);
    }

    // SaveExists tek dosya
    public static bool SaveExists(string fileName) => File.Exists(GetPath(fileName));

    // SaveExists hem PlayerData hem FightData
    public static bool AllSavesExists()
    {
        return SaveExists(PlayerDataFile) && SaveExists(FightDataFile);
    }
}
