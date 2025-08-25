using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
public class CharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scaling")]
    [SerializeField] private float scaleMultiplier = 1.1f;

    private Vector3 originalScale;
    private Transform buttonTransform;

    private void Awake()
    {
        buttonTransform = this.transform;
        originalScale = buttonTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.hoverSound);
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
