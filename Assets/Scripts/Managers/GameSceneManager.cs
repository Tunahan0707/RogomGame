using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text roomCountText;
    [SerializeField] private TMP_Text floorCountText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject marketRoom;
    [SerializeField] private GameObject fightRoom;
    [SerializeField] private GameObject gainRoom;
    [SerializeField] private GameObject upgradeRoom;
    [SerializeField] private GameObject restRoom;
    [SerializeField] private GameObject background;
    [SerializeField] private int onFightYValue = 5;
    [SerializeField] private int offFightYValue = -5;
    [SerializeField] private float animationDuration = 0.5f;

    private RoomType currentRoomType = RoomType.Fight;
    private int floor;
    private int room;

    public static event Action OnContinueButtonClicked;

    void Awake()
    {
        continueButton.onClick.AddListener(ContinueButtonClicked);
        pauseButton.onClick.AddListener(() =>
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        });
        resumeButton.onClick.AddListener(() =>
        {
            pausePanel.SetActive(false);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(Consts.Scenes.MAIN_MENU);
        });
        roomCountText.text = "1. Oda";
        floorCountText.text = "1. Kat";
        currentRoomType = RoomType.Fight;
        pausePanel.SetActive(false);
        room = 1;
        floor = 1;
        fightRoom.SetActive(false);
        marketRoom.SetActive(false);
        upgradeRoom.SetActive(false);
        gainRoom.SetActive(false);
        restRoom.SetActive(false);

    }
    void Start()
    {
        StartCoroutine(FadingForStart());
    }

    private IEnumerator FadingForStart()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeIn());
        background.transform.DOLocalMoveY(onFightYValue, animationDuration).SetEase(Ease.OutCubic);
        fightRoom.SetActive(true);
    }

    private void ContinueButtonClicked()
    {
        bool isFirstRoom = (room == 1 && floor == 1);
        OnContinueButtonClicked?.Invoke();
        room = RandomRoomSelector.currentRoomIndex;
        floor = RandomRoomSelector.currentFloorIndex;
        currentRoomType = RandomRoomSelector.selectedRoom;
        roomCountText.text = room + ". Oda";
        floorCountText.text = floor + ". Kat";
        if (isFirstRoom)
        {
            return;
        }
        EnterRoom(currentRoomType);
    }
    private void EnterRoom(RoomType roomType)
    {
        StartCoroutine(SwitchRoomWithFade(roomType));
    }
    private IEnumerator SwitchRoomWithFade(RoomType roomType)
    {
        // 1. Fade Out (ekran kararır)
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        // 2. Oda aktifliklerini ayarla
        fightRoom.SetActive(roomType == RoomType.Fight || roomType == RoomType.MiniBoss || roomType == RoomType.Boss);
        marketRoom.SetActive(roomType == RoomType.Market);
        upgradeRoom.SetActive(roomType == RoomType.CardUpgrade);
        gainRoom.SetActive(roomType == RoomType.GainOz || roomType == RoomType.CardChoice);
        restRoom.SetActive(roomType == RoomType.RestRoom);

        // 3. Background hedef pozisyonunu belirle
        float targetY = (roomType == RoomType.Fight || roomType == RoomType.MiniBoss || roomType == RoomType.Boss)
            ? onFightYValue
            : offFightYValue;

        // 4. Gösterilsin mi gizlensin mi?
        bool showBackgroundMove = (roomType == RoomType.Fight || roomType == RoomType.MiniBoss || roomType == RoomType.Boss);

        if (showBackgroundMove)
        {
            // Kullanıcı görecek → Fade açılırken animasyonlu hareket
            StartCoroutine(FadeManager.Instance.FadeIn());
            background.transform.DOLocalMoveY(targetY, animationDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            // Kullanıcı görmeyecek → Anında hareket, sonra fade açılır
            background.transform.DOLocalMoveY(targetY, 0f);
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
        }

    }


}
