using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "ScriptableObjects/Player")]
public class PlayersSO : ScriptableObject, IGameObject
{
    public string playersName;
    public Sprite playerSprite;
    public int health;
    public int inStartCoins;
    public int baseShield;
    public bool isLocked;
    public int unlockLevel;
    [HideInInspector] public string playerID;
    public List<CardsSO> extraStartingCards = new();

    public string Name { get => playersName; set => playersName = value; }
    public int UnlockLevel { get => unlockLevel; set => unlockLevel = value; }
    public string ID { get => playerID; set => playerID = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
