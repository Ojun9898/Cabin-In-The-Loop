using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Transform playerTransform;
    public Transform cameraTransform;

    // 인스펙터에서 등록할 무기 데이터 리스트 (ScriptableObject)
    public List<WeaponData> weaponDataList;

    // 데미지 필드 풀과 관련된 변수들
    [SerializeField] private GameObject damageFieldPrefab;
    private Queue<GameObject> _damageFieldPool = new();

    // 무기 풀 저장소 (무기 타입별 Queue)
    private Dictionary<WeaponType, Queue<GameObject>> _weaponPools = new();

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
                var obj = Instantiate(data.prefab);
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
}
