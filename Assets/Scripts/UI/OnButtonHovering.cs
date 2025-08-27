using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class OnButtonHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scaling")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    private Transform buttonTransform;

    private void Awake()
    {
        buttonTransform = this.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTransform.DOKill();
        buttonTransform.DOScale(scaleMultiplier, 0.2f).SetEase(Ease.OutBack);
        AudioManager.Instance?.PlaySound(AudioManager.Instance.hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonTransform == null) return;
        buttonTransform.DOKill();
        buttonTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }


}
