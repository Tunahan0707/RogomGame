using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsDisplayUI : MonoBehaviour
{
    [Header("Effects Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Icons for Effects")]
    [SerializeField] private List<Sprite> icons;
    public List<Sprite> Icons => icons; // sadece okunabilir property

    public EffectsDisplay SpawnEffect(int iconIndex, int value, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError("⚠️ EffectsDisplayUI: Prefab atanmadı!");
            return null;
        }

        if (parent == null)
        {
            Debug.LogError("⚠️ EffectsDisplayUI: Parent eksik!");
            return null;
        }

        if (iconIndex < 0 || iconIndex >= icons.Count)
        {
            Debug.LogWarning("⚠️ Geçersiz ikon indexi!");
            return null;
        }

        GameObject go = Instantiate(prefab, parent, false);
        var display = go.GetComponent<EffectsDisplay>();
        display.SetDatas(icons[iconIndex], value, iconIndex);
        return display;
    }
}
