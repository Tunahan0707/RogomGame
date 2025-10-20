using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsDisplayUI : MonoBehaviour
{
    [Header("Effects Prefab")]
    [SerializeField] private GameObject prefab;
    [Header("Icons for Effects")]
    [SerializeField] private List<Sprite> icons;
    public List<Sprite> Icons => icons; // sadece okumak için public property


    public EffectsDisplay SpawnEffect(int iconIndex, int y, Transform parent)
    {
        if (iconIndex < 0 || iconIndex >= icons.Count)
        {
            Debug.LogWarning("Geçersiz ikon indexi!");
            return null;
        }
        GameObject gO = Instantiate(prefab, parent);
        EffectsDisplay display = gO.GetComponent<EffectsDisplay>();
        display.SetDatas(icons[iconIndex], y, iconIndex);
        return display;
    }
}
