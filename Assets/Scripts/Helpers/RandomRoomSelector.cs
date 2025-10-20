using System.Collections.Generic;
using UnityEngine;

public class RandomRoomSelector : MonoBehaviour
{
    private const int totalRoomCount = 14;

    [HideInInspector] public static int currentRoomIndex = 1;
    public static int currentFloorIndex = 1;
    public static RoomType selectedRoom;
    private List<RoomType> addableRooms = new();
    private List<RoomType> possibleRooms = new();
    private int randomIndex;

    void Awake()
    {
        Debug.Log(GameStartManager.currentGameType);
        if (GameStartManager.currentGameType == GameType.ContinueGame) return;
        AddAddableRoomsForGameStart();
        AddAddableRoomsForAwake();
        AddAddableRoomsForAwake();
    }

    private void OnEnable()
    {
        GameSceneManager.OnContinueButtonClicked += OnContinueButtonClicked;
    }
    private void OnContinueButtonClicked()
    {
        if (currentRoomIndex <= totalRoomCount)
        {
            if (currentRoomIndex == totalRoomCount)
            {
                selectedRoom = RoomType.Boss;
            }
            else if (currentRoomIndex == 13)
            {
                selectedRoom = RoomType.RestRoom;
            }
            else if (currentRoomIndex == 9)
            {
                selectedRoom = RoomType.MiniBoss;
                possibleRooms.AddRange(addableRooms);
                possibleRooms.Add(RoomType.Fight);
                possibleRooms.Add(RoomType.Fight);
                addableRooms.Clear();
            }
            else if (currentRoomIndex == 6)
            {
                List<RoomType> fightRooms = addableRooms.FindAll(room => room == RoomType.Fight);
                possibleRooms.AddRange(fightRooms);
                addableRooms.RemoveAll(room => room == RoomType.Fight);
                fightRooms.Clear();
                selectedRoom = RoomType.GainOz;
                AddAddableRooms();
            }
            else if (currentRoomIndex == 4)
            {
                selectedRoom = RoomType.MiniBoss;
                addableRooms.Add(RoomType.Fight);
                addableRooms.Add(RoomType.Fight);
                addableRooms.Add(RoomType.Market);
                AddAddableRooms();
            }
            else
            {
                SelectRandomRoom();
            }
            currentRoomIndex++;
        }
        else
        {
            currentRoomIndex = 1;
            currentFloorIndex++;
            selectedRoom = RoomType.Fight;
            possibleRooms.Clear();
            addableRooms.Clear();
            AddAddableRoomsForGameStart();
            AddAddableRoomsForAwake();
            AddAddableRoomsForAwake();
        }
        Debug.Log($"Selected Room: {selectedRoom}, Current Room Index: {currentRoomIndex}, Current Floor Index: {currentFloorIndex}");
    }

    private void SelectRandomRoom()
    {
        randomIndex = UnityEngine.Random.Range(0, possibleRooms.Count);
        selectedRoom = possibleRooms[randomIndex];
        possibleRooms.RemoveAt(randomIndex);
    }
    private void AddAddableRooms()
    {
        randomIndex = UnityEngine.Random.Range(0, addableRooms.Count);
        if (addableRooms[randomIndex] == RoomType.CardChoice)
        {
            addableRooms.RemoveAll(room => room == RoomType.CardUpgrade);
        }
        else if (addableRooms[randomIndex] == RoomType.CardUpgrade)
        {
            addableRooms.RemoveAll(room => room == RoomType.CardChoice);
        }
        possibleRooms.Add(addableRooms[randomIndex]);
        addableRooms.RemoveAt(randomIndex);
    }
    private void AddAddableRoomsForAwake()
    {
        randomIndex = UnityEngine.Random.Range(0, addableRooms.Count);
        possibleRooms.Add(addableRooms[randomIndex]);
        addableRooms.RemoveAt(randomIndex);
    }
    private void AddAddableRoomsForGameStart()
    {
        addableRooms.AddRange(new List<RoomType>
        {
            RoomType.CardChoice,
            RoomType.CardUpgrade,
            RoomType.CardChoice,
            RoomType.CardUpgrade
        });
        possibleRooms.Add(RoomType.Fight);
    }
}
