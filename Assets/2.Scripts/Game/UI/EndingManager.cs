using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : Singleton<EndingManager>
{
    [SerializeField] private Altar altar;
    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject deadEndingPrefab;
    [SerializeField] private GameObject aliveEndingPrefab;

    private CanvasGroup DeadCanvasGroup;
    private CanvasGroup AliveCanvasGroup;

    private bool isAlive = true;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "(Bake)Laboratory" && altar == null)
            altar = FindObjectOfType<Altar>();
        
        DeadCanvasGroup = deadEndingPrefab.GetComponent<CanvasGroup>();
        DeadCanvasGroup.alpha = 0f;
        deadEndingPrefab.SetActive(false);
        
        AliveCanvasGroup = aliveEndingPrefab.GetComponent<CanvasGroup>();
        AliveCanvasGroup.alpha = 0f;
        aliveEndingPrefab.SetActive(false);

        DontDestroyOnLoad(gameObject);
    }

    public void ShowDeadEnding()
    {
        // if (!deadEndingPrefab.activeSelf) return;
        
        isAlive = false;
        deadEndingPrefab.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(FadeIn());
    }

    public void ShowAliveEnding()
    {
        if (!aliveEndingPrefab.activeSelf) return;
        
        isAlive = true;
        aliveEndingPrefab.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(FadeIn());
    }

    public void OnClickHomeButton()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeIn()
    {
        float duration = 1.5f;
        float elapsed = 0f;

        CanvasGroup canvasGroup = isAlive ? AliveCanvasGroup : DeadCanvasGroup;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float duration = 1.5f;
        float elapsed = 0f;

        CanvasGroup canvasGroup = isAlive ? AliveCanvasGroup : DeadCanvasGroup;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        PauseManager.Instance.DestroyAllWithTag();
        SceneManager.LoadScene("Main");
    }
}
