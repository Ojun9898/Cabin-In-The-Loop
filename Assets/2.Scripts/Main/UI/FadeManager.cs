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

    // 추가한 함수 : 특정 GameObject를 페이드 인/아웃
    public IEnumerator FadeGameObject(GameObject target, float fromAlpha, float toAlpha)
    {
        if (target == null)
            yield break;

        // CanvasGroup 가져오기 또는 추가하기
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        cg.alpha = fromAlpha;
        cg.interactable = toAlpha > 0.5f;
        cg.blocksRaycasts = toAlpha > 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = toAlpha;
        cg.interactable = toAlpha > 0.5f;
        cg.blocksRaycasts = toAlpha > 0.5f;
    }
}
