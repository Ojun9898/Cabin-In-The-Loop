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
        SceneManager.sceneLoaded += PlayerPositionSetting;
        SceneManager.sceneLoaded += RemoveAudioListener;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= PlayerPositionSetting;
        SceneManager.sceneLoaded -= RemoveAudioListener;
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

    // 씬 로드될 때마다 플레이어 포지션 잡기
    void PlayerPositionSetting(Scene scene, LoadSceneMode mode)
    {
        // 씬마다 Player를 다시 찾아야 할 수 있음
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return;
        }
        
        CharacterController characterController = player.GetComponent<CharacterController>();
        
        Vector3 targetLocation = transform.position; // 기본값

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "(Bake)Cavin")
        {
            BGMManager.Instance.Play("Cavin BGM");
            targetLocation = new Vector3(71f, 0f, 42f);
        }
        else if (sceneName == "(Bake)Laboratory")
        {
            BGMManager.Instance.Play("Lab BGM");
            targetLocation = new Vector3(46.7f, 0.5f, 14.8f);
        }

        // CharacterController 위치 수정
        characterController.enabled = false;
        characterController.transform.position = targetLocation; // 위치 수정
        characterController.enabled = true;
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

    void RemoveAudioListener(Scene scene, LoadSceneMode mode)
    {
        // AudioListener 중복 방지
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
    }
}
