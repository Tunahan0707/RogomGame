using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class OnButtonHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scaling")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    private Transform buttonTransform;
    private Vector3 originalScale = Vector3.one;

    private void Awake()
    {
        buttonTransform = transform;
        originalScale = buttonTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTransform.DOKill();
        buttonTransform.DOScale(originalScale * scaleMultiplier, 0.2f).SetEase(Ease.OutBack);
        AudioManager.Instance?.PlaySound(AudioManager.Instance.hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonTransform == null) return;
        buttonTransform.DOKill();
        buttonTransform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }


}
