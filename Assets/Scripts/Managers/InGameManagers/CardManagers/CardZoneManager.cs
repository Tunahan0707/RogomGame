using System.Collections.Generic;
using UnityEngine;

public static class CardZoneManager
{
    private static readonly Dictionary<string, CardZone> zones = new();

    public static void SetZone(string id, CardZone zone)
    {
        zones[id] = zone;
    }

    public static CardZone GetZone(string id)
    {
        return zones.TryGetValue(id, out var z) ? z : CardZone.Draw;
    }

    public static IEnumerable<string> GetByZone(CardZone zone)
    {
        foreach (var kvp in zones)
            if (kvp.Value == zone)
                yield return kvp.Key;
    }

    public static CardDisplay GetRandomCardByZone(CardZone zone)
    {
        // 1. O zone'daki kart ID'lerini al
        var ids = new List<string>(CardZoneManager.GetByZone(zone));

        // 2. Eğer hiç kart yoksa null döndür
        if (ids.Count == 0)
            return null;

        // 3. Rastgele bir ID seç
        string randomId = ids[UnityEngine.Random.Range(0, ids.Count)];

        // 4. ID’ye göre CardDisplay nesnesini bul
        if (CardManagerUI.Instance.TryGetCardDisplay(randomId, out var display))
            return display;

        return null;
    }

    public static void ClearZone(CardZone zone)
    {
        var toRemove = new List<string>();

        // Önce o zone'daki tüm kart ID'lerini bul
        foreach (var kvp in zones)
        {
            if (kvp.Value == zone)
                toRemove.Add(kvp.Key);
        }

        // Sonra bu ID'leri sözlükten sil
        foreach (var id in toRemove)
            zones.Remove(id);
    }
    public static void Clear()
    {
        zones.Clear();
    }
}

public enum CardZone
{
    Draw,
    Hand,
    Discard
}
