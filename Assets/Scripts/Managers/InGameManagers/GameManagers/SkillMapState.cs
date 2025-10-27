using System;
using System.Collections.Generic;

[Serializable]
public class SkillMapState
{
    public HashSet<string> unlocked = new();
    public HashSet<string> blocked = new();

    // 0–100 arası gizli lanet yüzdesi
    public int cursePercent = 0;

    // İsteğe bağlı sahibi (Player/Character ID)
    public string ownerPlayerID;
}
