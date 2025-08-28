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
    private CanvasGroup fadeCanvas; // Moved declaration here

    private void Awake()
    {
        marketAnimator = gameObject.GetComponent<Animator>();
        buttonTransform = transform;
        originalScale = buttonTransform.localScale;
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            marketAnimator.SetBool(Consts.Animations.MARKET, true);
            marketAnimator.Play(Consts.Animations.DOOR_ANIMATION);
            _ = StartCoroutine(PlayAnimationAndLoadScene());
        });
        GameObject fadeObj = new("FadeCanvas");
        fadeObj.transform.SetParent(transform.root, false);
        fadeCanvas = fadeObj.AddComponent<CanvasGroup>();
        var canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeObj.AddComponent<Image>().color = Color.black;
        fadeCanvas.alpha = 0;
        fadeObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        fadeObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
        fadeObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        fadeObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        System.Collections.IEnumerator FadeOut()
        {
            fadeCanvas.gameObject.SetActive(true);
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(0, 1, elapsed / duration);
                yield return null;
            }
            fadeCanvas.alpha = 1;
        }

        System.Collections.IEnumerator PlayAnimationAndLoadScene()
        {
            yield return StartCoroutine(FadeOut());
            marketAnimator.Play(Consts.Animations.DOOR_ANIMATION);
            yield return new WaitForSeconds(marketAnimator.GetCurrentAnimatorStateInfo(0).length);
            marketAnimator.SetBool(Consts.Animations.MARKET, false);
            SceneManager.LoadScene(Consts.Scenes.MARKET);
        }
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
