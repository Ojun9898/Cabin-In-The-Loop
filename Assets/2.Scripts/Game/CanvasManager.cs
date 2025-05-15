using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : Singleton<CanvasManager>
{
    private Canvas originalCanvas;
    private Canvas currentSceneCanvas;

    void Awake()
    {
        // 자기 자신이 원본인지 검사 (씬이 "DontDestroyOnLoad"가 아니면 복제본임)
        if (SceneManager.GetActiveScene() != gameObject.scene)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // 원본 Canvas 가져오기
        if (originalCanvas == null)
            originalCanvas = GetComponent<Canvas>();

        // 복제될 원본 Canvas만 DontDestroy
        if (originalCanvas != null)
            DontDestroyOnLoad(originalCanvas.gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnCanvasForScene();
    }

    private void SpawnCanvasForScene()
    {
        // 이전 씬에서 복제된 Canvas가 있다면 파괴
        if (currentSceneCanvas != null)
        {
            Destroy(currentSceneCanvas.gameObject);
        }

        if (originalCanvas == null)
            return;

        // 복제 생성
        currentSceneCanvas = Instantiate(originalCanvas);

        // 복제된 Canvas는 현재 씬에 속하게 이동
        SceneManager.MoveGameObjectToScene(currentSceneCanvas.gameObject, SceneManager.GetActiveScene());

        // 복제된 Canvas에 붙은 CanvasManager 제거
        CanvasManager cloneManager = currentSceneCanvas.GetComponent<CanvasManager>();
        if (cloneManager != null)
        {
            Destroy(cloneManager);
        }
    }

    public Canvas GetCanvas()
    {
        if (currentSceneCanvas == null)
            SpawnCanvasForScene();

        return currentSceneCanvas;
    }
}