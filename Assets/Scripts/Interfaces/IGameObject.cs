using UnityEngine;

public interface IGameObject
{
    string Name { get; set; }
    int UnlockLevel { get; set; }
    string ID { get; set; }
    bool IsLocked { get; set; }
}
