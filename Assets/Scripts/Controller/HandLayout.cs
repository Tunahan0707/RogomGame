using UnityEngine;
using System.Collections.Generic;

public class HandLayout : MonoBehaviour
{
    public List<GameObject> handCards = new(); // aktif kart objeleri
    [SerializeField] private float cardSpacing = 150f;
    [SerializeField] private float maxSpread = 800f;
    [SerializeField] private float curveHeight = 80f;
    [SerializeField] private float maxRotation = 15f;

    public void UpdateHandLayout()
    {
        int count = handCards.Count;
        if (count == 0) return;

        // Kart sayısına göre spacing azalt (sıkışma)
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

            // X konumu
            float x = (count == 1) ? 0f : startX + i * spacing;

            // Y konumu (kavis)
            float normalizedX = (totalWidth == 0) ? 0 : x / (totalWidth / 2f);
            float y = curveHeight * (1 - normalizedX * normalizedX);
            rect.anchoredPosition = new Vector2(x, y);


            // Rotation
            float rotation = 0f;
            if (totalWidth != 0f)
                rotation = -(x / (totalWidth / 2f)) * maxRotation;

            rect.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    public void AddCard(GameObject card)
    {
        handCards.Add(card);
        UpdateHandLayout();
    }

    public void RemoveCard(GameObject card)
    {
        handCards.Remove(card);
        UpdateHandLayout();
    }
    public void ClearHand()
    {
        handCards.Clear();
        UpdateHandLayout();
    }
}
