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
        if (scene.name == "Main") return; // 메인에서는 복제 X

        SpawnCanvasForScene();

        if (!transform.Find("HUD").gameObject.activeSelf)
            transform.Find("HUD").gameObject.SetActive(true);
    }

    private void SpawnCanvasForScene()
    {
        if (currentSceneCanvas != null)
            Destroy(currentSceneCanvas.gameObject);

        if (originalCanvas == null)
            return;

        // Canvas의 루트 GameObject 전체를 복제
        GameObject canvasRoot = originalCanvas.transform.root.gameObject;
        GameObject clone = Instantiate(canvasRoot);

        SceneManager.MoveGameObjectToScene(clone, SceneManager.GetActiveScene());

        // 복제된 Canvas 참조 저장
        currentSceneCanvas = clone.GetComponentInChildren<Canvas>();

        // 복제된 CanvasManager 제거
        CanvasManager cloneManager = clone.GetComponentInChildren<CanvasManager>();
        if (cloneManager != null)
            Destroy(cloneManager);
    }

    public Canvas GetCanvas()
    {
        if (currentSceneCanvas == null)
            SpawnCanvasForScene();

        return currentSceneCanvas;
    }
}