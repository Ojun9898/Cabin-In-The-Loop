using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public Transform playerTransform;
    public Transform cameraTransform;

    // Cameras, Player 프리팹
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject camerasPrefab;

    // 인스펙터에서 등록할 무기 데이터 리스트 (ScriptableObject)
    public List<WeaponData> weaponDataList;

    // 데미지 필드 풀과 관련된 변수들
    [SerializeField] private GameObject damageFieldPrefab;
    private Queue<GameObject> _damageFieldPool = new();

    // 무기 풀 저장소 (무기 타입별 Queue)
    private Dictionary<WeaponType, Queue<GameObject>> _weaponPools = new();
    [SerializeField] private Transform weaponParent;

    // 무기 타입과 무기 데이터 매핑 (빠른 접근용)
    private Dictionary<WeaponType, WeaponData> _weaponDataMap = new();

    // 무기 선택 UI
    public WeaponSelectionUI weaponSelectionUI;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 GameManager 객체 유지

        // 플레이어 상태 머신을 찾아서 Transform 저장
        PlayerStateMachine playerSM = FindObjectOfType<PlayerStateMachine>();
        if (playerSM != null)
            playerTransform = playerSM.transform;

        // 무기 풀 초기화
        InitializeWeaponPools();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }


    // 무기 풀 초기화: 각 무기별로 미리 인스턴스 생성해서 풀에 저장
    private void InitializeWeaponPools()
    {
        foreach (var data in weaponDataList)
        {
            // 무기 데이터 매핑 저장
            _weaponDataMap[data.weaponType] = data;

            // 새로운 풀 생성
            Queue<GameObject> pool = new Queue<GameObject>();

            // 초기 풀 사이즈만큼 무기 인스턴스를 생성
            for (int i = 0; i < 5; i++)
            {
                var obj = Instantiate(data.prefab, weaponParent);
                obj.SetActive(false); // 비활성화 상태로 풀에 저장
                pool.Enqueue(obj);
            }

            // 무기 타입별로 풀을 저장
            _weaponPools[data.weaponType] = pool;
        }
    }

    // 무기를 풀에서 가져옴 (없으면 새로 생성)
    public GameObject GetWeapon(WeaponType type)
    {
        if (!_weaponPools.TryGetValue(type, out var pool))
        {
            return null;
        }

        if (pool.Count > 0)
        {
            var weapon = pool.Dequeue();
            weapon.SetActive(true); // 활성화하여 반환
            return weapon;
        }

        // 풀에 없을 경우 새로 인스턴스 생성
        if (_weaponDataMap.TryGetValue(type, out var data))
            return Instantiate(data.prefab);

        return null;
    }

    // 무기를 다시 풀에 반환
    public void ReturnWeapon(WeaponType type, GameObject weapon)
    {
        if (!_weaponPools.ContainsKey(type))
        {
            Destroy(weapon); // 알 수 없는 타입이면 그냥 파괴
            return;
        }

        weapon.SetActive(false); // 비활성화 후
        _weaponPools[type].Enqueue(weapon); // 풀에 다시 저장
    }

    // 무기 타입에 해당하는 무기 데이터 반환
    public WeaponData GetWeaponData(WeaponType type)
    {
        _weaponDataMap.TryGetValue(type, out var data);
        return data;
    }

    // 데미지 필드 풀에서 가져옴
    public GameObject GetDamageField()
    {
        if (_damageFieldPool.Count > 0)
            return _damageFieldPool.Dequeue();

        return Instantiate(damageFieldPrefab);
    }

    // 데미지 필드를 풀에 반환
    public void ReturnDamageField(GameObject field)
    {
        field.SetActive(false);
        _damageFieldPool.Enqueue(field);
    }

    // 레벨업 시 무기 선택 UI 띄우기
    public void ShowWeaponSelection()
    {
        List<WeaponData> randomWeapons = new List<WeaponData>();
        List<WeaponData> availableWeapons = new List<WeaponData>(weaponDataList);
        int selectedCount = Mathf.Min(3, availableWeapons.Count); // 최소 3개 무기 선택

        // 랜덤으로 3개의 무기 선택
        for (int i = 0; i < selectedCount; i++)
        {
            int randomIndex = Random.Range(0, availableWeapons.Count);
            randomWeapons.Add(availableWeapons[randomIndex]);
        }

        // 무기 선택 UI 초기화
        weaponSelectionUI.Initialize(randomWeapons);
    }

    #region OnSceneLoaded 이벤트

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
{
    HandlePlayerAndCamera(scene);
    SetPlayerPosition(scene);
    CleanupAudioListeners();
}

private void HandlePlayerAndCamera(Scene scene)
{
    string sceneName = scene.name;

    // Main 씬에서는 재생성X
    if (sceneName == "Main")
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var camera = GameObject.Find("Cameras") ?? GameObject.Find("Cameras(Clone)");

        if (player != null)
        {
            Debug.Log("[GameManager] Main 씬 - Player 삭제");
            Destroy(player);
        }

        if (camera != null)
        {
            Debug.Log("[GameManager] Main 씬 - Camera 삭제");
            Destroy(camera);
        }

        return;
    }

    // Cavin, Laboratory 씬에서는 없을 때만 생성
    if (sceneName == "(Bake)Cavin" || sceneName == "(Bake)Laboratory")
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || playerObj.GetComponent<PlayerStateMachine>() == null)
        {
            playerObj = Instantiate(playerPrefab);
        }
        playerTransform = playerObj.transform;

        if (GameObject.Find("Cameras") == null && GameObject.Find("Cameras(Clone)") == null)
        {
            var cam = Instantiate(camerasPrefab);
            cameraTransform = cam.transform;
        }
        
        var wc = playerObj.GetComponent<WeaponController>();
        if (wc != null)
        {
            var ps = PlayerStatus.Ensure();
            // 저장된 무기 타입이 있으면 그걸, 없으면 디폴트(Axe) 장착
            var equipType = ps.CurrentWeaponType != 0 ? ps.CurrentWeaponType : WeaponType.Axe;
            wc.EquipWeapon(equipType);
        }
        else
        {
            Debug.LogWarning("[GameManager] WeaponController를 Player에서 찾지 못했습니다.");
        }
    }
}

private void SetPlayerPosition(Scene scene)
{
    GameObject player = GameObject.FindWithTag("Player");
    if (player == null) return;

    CharacterController cc = player.GetComponent<CharacterController>();
    if (cc == null) return;

    Vector3 targetPos = Vector3.zero;
    string sceneName = scene.name;

    if (sceneName == "(Bake)Cavin")
    {
        BGMManager.Instance.Play("Cavin BGM");
        targetPos = new Vector3(70f, 0f, 40f);
    }
    else if (sceneName == "(Bake)Laboratory")
    {
        BGMManager.Instance.Play("Lab BGM");
        targetPos = new Vector3(46.7f, 0.5f, 14.8f);
    }

    cc.enabled = false;
    cc.transform.position = targetPos;
    cc.enabled = true;
}

private void CleanupAudioListeners()
{
    AudioListener[] listeners = FindObjectsOfType<AudioListener>();
    for (int i = 1; i < listeners.Length; i++)
        Destroy(listeners[i]);
}

    #endregion
}