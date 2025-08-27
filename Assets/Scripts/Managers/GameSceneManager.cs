using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public enum GameState
{
    Playing,
    Paused,
    Win,
    GameOver
}

public enum RoomType
{
    Fight,
    Market,
    MiniBoss,
    Boss,
    CardChoice,
    RestRoom,
    CardUpgrade,
    GainOz
}
public class GameSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text roomCountText;
    [SerializeField] private TMP_Text floorCountText;
    [SerializeField] private GameObject pausePanel;
    private int floor;
    private int room;
    public static event Action OnContinueButtonClicked;

    void Awake()
    {
        continueButton.onClick.AddListener(ContinueButtonClicked);
        pauseButton.onClick.AddListener(() => pausePanel?.SetActive(true));
        roomCountText.text = "1. Oda";
        floorCountText.text = "1. Kat";

    }
    private void ContinueButtonClicked()
    {
        OnContinueButtonClicked?.Invoke();
        room = RandomRoomSelector.currentRoomIndex;
        floor = RandomRoomSelector.currentFloorIndex;
        roomCountText.text = room + ". Oda";
        floorCountText.text = floor + ". Kat";
    }
}
