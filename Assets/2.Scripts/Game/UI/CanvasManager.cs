using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : Singleton<CanvasManager>
{
    private Canvas currentSceneCanvas;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Singleton 처리
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // CanvasManager 자체 유지

        // 현재 Canvas 가져오기 (이 스크립트가 붙은 Canvas 기준)
        currentSceneCanvas = GetComponent<Canvas>();
        if (currentSceneCanvas != null)
            DontDestroyOnLoad(currentSceneCanvas.gameObject); // Canvas 유지
    }

    public Canvas GetCanvas()
    {
        return currentSceneCanvas;
    }
}