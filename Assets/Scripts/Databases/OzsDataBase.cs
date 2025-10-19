using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "OzsDataBase", menuName = "ScriptableObjects/Databases/OzsDataBase", order = 2)]
public class OzsDataBase : ScriptableObject
{
    [Header("Oz Data")]
    public List<OzsSO> ozs;

    private void OnEnable()
    {
        ozs = new List<OzsSO>(Resources.LoadAll<OzsSO>(Consts.FileWays.OzsSO));
    }

    public OzsSO GetOzByID(string id)
    {
        return ozs.Find(oz => oz.ozsID == id);
    }

    public List<OzsSO> UnlockLevel(int lvl)
    {
        return ozs.FindAll(oz => oz.unlockLevel == lvl);
    }
}
