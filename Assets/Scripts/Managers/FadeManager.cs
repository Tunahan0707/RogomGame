using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    private static FadeManager instance;
    private CanvasGroup fadeCanvas;
    private float defaultDuration = 0.5f;

    public static FadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("FadeManager");
                instance = obj.AddComponent<FadeManager>();
                instance.SetupFadeCanvas();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        // Sahne ilk yüklendiğinde siyah başlasın
        if (fadeCanvas != null)
        {
            Color c = fadeCanvas.GetComponent<Image>().color;
            c.a = 1f; // tamamen siyah
            fadeCanvas.GetComponent<Image>().color = c;
        }

    }
    private void SetupFadeCanvas()
    {
        var fadeObj = new GameObject("FadeCanvas");
        fadeObj.transform.SetParent(transform, false);

        fadeCanvas = fadeObj.AddComponent<CanvasGroup>();
        var canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeObj.AddComponent<Image>().color = Color.black;

        fadeCanvas.alpha = 0;

        var rect = fadeObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration < 0) duration = defaultDuration;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        fadeCanvas.alpha = 1;
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration < 0) duration = defaultDuration;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }
        fadeCanvas.alpha = 0;
    }
}

