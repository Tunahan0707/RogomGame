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
    
    [Header("Rooms")]
    [SerializeField] private GameObject marketRoom;
    [SerializeField] private GameObject fightRoom;
    [SerializeField] private GameObject gainRoom;
    [SerializeField] private GameObject upgradeRoom;
    [SerializeField] private GameObject restRoom;
    [SerializeField] private GameObject background;

    [Header("Animations")]
    [SerializeField] private int onFightYValue = 5;
    [SerializeField] private int offFightYValue = -5;
    [SerializeField] private float animationDuration = 0.5f;

    private RoomType currentRoomType = RoomType.Fight;
    private int floor;
    private int room;

    public static event Action OnContinueButtonClicked;
    private EnemyManager enemyManager;

    private TurnManager turnManager => GetComponent<TurnManager>();

    public int HandleDrawDeckCompleted { get; private set; }

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
        enemyManager = GetComponent<EnemyManager>();
    }
    private void Start()
    {
        EnterRoom(currentRoomType);
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
        FightDataHolder.Instance.SaveDatas();
        if (isFirstRoom) { return; }
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

        // 3. Gösterilecek mi? Sadece fight odalarında animasyon
        bool isFightRoom = roomType == RoomType.Fight || roomType == RoomType.MiniBoss || roomType == RoomType.Boss;
        float targetY = isFightRoom ? onFightYValue : offFightYValue;

        if (isFightRoom)
        {
            // Background'u önce aşağıya koy (offFightYValue) → sonra yukarıya animasyonla çıkart
            background.transform.localPosition = new Vector3(
                background.transform.localPosition.x,
                offFightYValue,
                background.transform.localPosition.z);

            // Fade açılırken yukarıya hareket etsin
            StartCoroutine(FadeManager.Instance.FadeIn());
            background.transform.DOLocalMoveY(onFightYValue, animationDuration).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(animationDuration);
        }
        else
        {
            // Fight değil → direkt yeni pozisyona ışınla
            background.transform.DOLocalMoveY(targetY, 0f);
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
        }
        
    }


}
