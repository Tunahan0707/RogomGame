using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "OzsDataBase", menuName = "ScriptableObjects/Databases/OzsDataBase", order = 2)]
public class OzsDataBase : ScriptableObject
{
    [Header("Oz Data")]
    public List<OzsSo> ozs;

    private void OnEnable()
    {
        ozs = new List<OzsSo>(Resources.LoadAll<OzsSo>(Consts.FileWays.OzSO));
    }
}
