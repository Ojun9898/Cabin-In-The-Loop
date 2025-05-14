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

        skyAndFogGlobalVolume = GameObject.Find("Sky And Fog Global Volume");

        if (skyAndFogGlobalVolume == null)
            Debug.LogError("SkyAndFogGlobalVolume 오브젝트를 찾을 수 없습니다.");

        else
        {
            Debug.Log("SkyAndFogGlobalVolume 오브젝트가 정상적으로 존재합니다.");
            volume = skyAndFogGlobalVolume.GetComponent<Volume>(); // Volume 컴포넌트 할당
            if (volume == null)
            {
                Debug.LogError("SkyAndFogGlobalVolume 오브젝트에 Volume 컴포넌트가 없습니다.");
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += PlayerPositionSetting;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= PlayerPositionSetting;
    }

    // 씬 로드될 때마다 플레이어 포지션 잡기
    void PlayerPositionSetting(Scene scene, LoadSceneMode mode)
    {
        // 씬마다 Player를 다시 찾아야 할 수 있음
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없습니다.");
            return;
        }
        
        CharacterController characterController = player.GetComponent<CharacterController>();
        
        Vector3 targetLocation = transform.position; // 기본값

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "(Bake)Cavin")
        {
            targetLocation = new Vector3(71f, 0f, 42f);
        }
        else if (sceneName == "(Bake)Laboratory")
        {
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

        Debug.Log("Fog eliminated");
    }
}