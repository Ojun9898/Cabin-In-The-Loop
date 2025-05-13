using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    private GameObject canvasObj;

    private void Awake()
    {
        Instance = this;
        CreateFadeImage();
    }

    private void CreateFadeImage()
    {
        canvasObj = new GameObject("FadeCanvas");
        canvasObj.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imgObj.AddComponent<Image>();

        // 클릭 막지 않도록 Raycast Target 끔
        fadeImage.raycastTarget = false;
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f)); // 페이드 아웃
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return StartCoroutine(Fade(1f, 0f)); // 페이드 인

        // 다 끝났으면 제거
        Destroy(canvasObj);
        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}
