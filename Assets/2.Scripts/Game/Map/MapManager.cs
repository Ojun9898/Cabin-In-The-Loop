using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] GameObject skyAndFogGlobalVolume;

    private AltarDoor altarDoor;
    private Volume volume;
    private Transform targetLocation;
    private bool isFogRoutineRunning = false;

    [HideInInspector] public bool isFogEliminated = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        altarDoor = FindObjectOfType<AltarDoor>();
        
        // 초기에는 skyAndFogGlobalVolume을 찾지 않고, 씬이 로드될 때마다 찾아서 할당
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드될 때마다 skyAndFogGlobalVolume을 찾아서 할당
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        skyAndFogGlobalVolume = GameObject.Find("Sky And Fog Global Volume");

        if (skyAndFogGlobalVolume != null)
        {
            volume = skyAndFogGlobalVolume.GetComponent<Volume>(); // Volume 컴포넌트 할당
        }
    }

    // Laboratory 제단 안개 서서히 제거
    public void StartFog()
    {
        if (!isFogRoutineRunning)
            StartCoroutine(GraduallyEliminateFog());
    }

    private IEnumerator GraduallyEliminateFog()
    {
        isFogRoutineRunning = true;

        float duration = 2.0f;
        float elapsed = 0f;
        float startWeight = volume.weight;
        float targetWeight = 0.7f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            volume.weight = Mathf.Lerp(startWeight, targetWeight, elapsed / duration);
            yield return null;
        }

        volume.weight = targetWeight;
        isFogEliminated = true;
        isFogRoutineRunning = false;
    }

    
}
