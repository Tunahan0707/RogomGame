using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HandLayout : MonoBehaviour
{
    public static HandLayout Instance;
    [SerializeField] private float cardSpacing = 150f;
    [SerializeField] private float maxSpread = 800f;
    [SerializeField] private float curveHeight = 80f;
    [SerializeField] private float maxRotation = 15f;
    [SerializeField] private float layoutAnimDuration = 0.2f;

    public List<GameObject> handCards = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void UpdateHandLayout(GameObject highlightedCard = null)
    {
        int count = handCards.Count;
        if (count == 0) return;

        float spacing = cardSpacing;
        float totalWidth = spacing * (count - 1);
        if (totalWidth > maxSpread)
        {
            spacing = maxSpread / (count - 1);
            totalWidth = maxSpread;
        }

        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
{
    GameObject card = handCards[i];
    RectTransform rect = card.GetComponent<RectTransform>();

    float x = (count == 1) ? 0f : startX + i * spacing;
    float normalizedX = (totalWidth == 0) ? 0 : x / (totalWidth / 2f);
    float y = curveHeight * (1 - normalizedX * normalizedX);

    // Rotation güvenli şekilde hesaplanıyor
    float rotation = 0f;
    if (Mathf.Abs(totalWidth) > Mathf.Epsilon)
        rotation = -(x / (totalWidth / 2f)) * maxRotation;

    // Eğer hover edilen kartsa biraz yukarı çıkar
    if (card == highlightedCard)
        y += 60f;

    rect.DOAnchorPos(new Vector2(x, y), layoutAnimDuration).SetEase(Ease.OutCubic);
    rect.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, rotation), layoutAnimDuration).SetEase(Ease.OutCubic);
}

    }

    public void AddCard(GameObject card)
    {
        handCards.Add(card);
        UpdateHandLayout();
    }

    public void RemoveCard(GameObject card)
    {
        if (handCards.Contains(card))
            handCards.Remove(card);
        UpdateHandLayout();
    }
}
