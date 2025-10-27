using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnMarketButtonClicked : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scaling")]
    [SerializeField] private float scaleMultiplier = 1.1f;

    private Vector3 originalScale;
    private Transform buttonTransform;
    private Animator marketAnimator;

    private void Awake()
    {
        marketAnimator = gameObject.GetComponent<Animator>();
        buttonTransform = transform;
        originalScale = buttonTransform.localScale;

        // Butona tıklandığında çağrılacak event
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            marketAnimator.SetBool(Consts.Animations.MARKET, true);
            marketAnimator.Play(Consts.Animations.DOOR_ANIMATION);
            _ = StartCoroutine(PlayAnimationAndLoadScene());
        });
    }

    private System.Collections.IEnumerator PlayAnimationAndLoadScene()
    {
        // Fade Out başlat
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        // Kapı animasyonu oynat
        marketAnimator.Play(Consts.Animations.DOOR_ANIMATION);
        yield return new WaitForSeconds(marketAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Animator parametresini resetle
        marketAnimator.SetBool(Consts.Animations.MARKET, false);

        // Sahneyi yükle
        SceneManager.LoadScene(Consts.Scenes.MARKET);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance?.PlaySound(AudioManager.Instance.hoverSound);
        buttonTransform.DOKill();
        buttonTransform.DOScale(originalScale * scaleMultiplier, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonTransform == null) return;
        buttonTransform.DOKill();
        buttonTransform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
