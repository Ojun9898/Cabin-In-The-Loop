using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.SceneManagement;

using SpawnManager_PlayerTransformCheck;


// MonsterType enum을 이곳에 선언
public enum MonsterType
{
    Zombie,
    Insectoid,
    Ripper,
    Vendigo,
    Beast
}

public class SpawnManager : MonoBehaviour
{
    [Header("스폰 장소")]
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("플레이어 Transform")]
    [SerializeField] private Transform playerTransform;
    
    [Header("몬스터 세팅")]
    [Tooltip("Monsters 에서 설정한 Zombie Label")]
    [SerializeField] private string zombieLabel;
    [SerializeField] private int zombiePoolSize = 11;
    
    [Tooltip("Monsters 에서 설정한 Ripper Label")]
    [SerializeField] private string insectoidLabel;
    [SerializeField] private int insectoidPoolSize = 3;
    
    [Tooltip("Monsters 에서 설정한 Ripper Label")]
    [SerializeField] private string ripperLabel;
    [SerializeField] private int ripperPoolSize = 6;
    
    [Tooltip("Monsters 에서 설정한 Vendigo Label")]
    [SerializeField] private string vendigoLabel;
    [SerializeField] private int vendigoPoolSize = 5;
    
    [Tooltip("Monsters 에서 설정한 Vendigo Label")]
    [SerializeField] private string beastLabel;
    [SerializeField] private int beastPoolSize = 3;
    
    
    [Tooltip("생성 가능한 몬스터의 총합")]
    [SerializeField] private int totalPoolSize = 68;
    

    [Header("몬스터 보관위치")]
    [SerializeField] private Transform PrefabsContainer;
    
    private static SpawnManager _instance;
    
    // 1. 라벨로 불러온 프리팹 목록
    private List<GameObject> zombieLabels = new List<GameObject>();
    private List<GameObject> insectoidLabels = new List<GameObject>();
    private List<GameObject> ripperLabels = new List<GameObject>();
    private List<GameObject> vendigoLabels = new List<GameObject>();
    private List<GameObject> beastLabels = new List<GameObject>();
    
    // 2. 1.에서 불러운 프리팹들을 Pool로 넣기
    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> insectoidPool = new List<GameObject>();
    private List<GameObject> ripperPool = new List<GameObject>();
    private List<GameObject> vendigoPool = new List<GameObject>();
    private List<GameObject> beastPool = new List<GameObject>();
    
    private int TotalPoolCount() => zombiePool.Count + insectoidPool.Count + ripperPool.Count + vendigoPool.Count 
                                    + beastPool.Count;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // 라벨로 프리팹들을 한꺼번에 로드
        LabelsLoad();
    }

    #region 라벨 로드 및 초기화
    private void LabelsLoad()
    {
        // 좀비 라벨 불러오기
        Addressables.LoadAssetsAsync<GameObject>
            (zombieLabel, prefab => zombieLabels.Add(prefab)).Completed += process =>
        {
            if (process.Status == AsyncOperationStatus.Succeeded)
            {
                // 풀에 채울 총 몬스터 수를 변수에 담기
                int totalzombie = zombiePoolSize; 
                // 리스트에 있는 프리팹 (인덱스 0번) 부터 순서대로 적용
                for (int i = 0; i < totalzombie; i++)
                {
                    var prefab = zombieLabels[i % zombieLabels.Count];
                    PreloadPool(prefab, zombiePool);
                }
                
            }
            else Debug.LogError($"[{nameof(SpawnManager)}] zombie assets 로드 실패");
        };
        // 곤충 라벨 불러오기
        Addressables.LoadAssetsAsync<GameObject>
            (insectoidLabel, prefab => insectoidLabels.Add(prefab)).Completed += process =>
        {
            if (process.Status == AsyncOperationStatus.Succeeded)
            {
                // 풀에 채울 총 몬스터 수를 변수에 담기
                int totalinsectoid = insectoidPoolSize; 
                // 리스트에 있는 프리팹 (인덱스 0번) 부터 순서대로 적용
                for (int i = 0; i < totalinsectoid; i++)
                {
                    var prefab = insectoidLabels[i % insectoidLabels.Count];
                    PreloadPool(prefab, insectoidPool);
                }
                
            }
            else Debug.LogError($"[{nameof(SpawnManager)}] insectoid assets 로드 실패");
        };
        
        // 리퍼 라벨 불러오기
        Addressables.LoadAssetsAsync<GameObject>
            (ripperLabel, prefab => ripperLabels.Add(prefab)).Completed += process =>
        {
            if (process.Status == AsyncOperationStatus.Succeeded)
            {
                // 풀에 채울 총 몬스터 수를 변수에 담기
                int totalripper = ripperPoolSize; 
                // 리스트에 있는 프리팹 (인덱스 0번) 부터 순서대로 적용
                for (int i = 0; i < totalripper; i++)
                {
                    var prefab = ripperLabels[i % ripperLabels.Count];
                    PreloadPool(prefab, ripperPool);
                }
                
            }
            else Debug.LogError($"[{nameof(SpawnManager)}] Ripper assets 로드 실패");
        };
        
        // 밴디고 라벨 불러오기
        Addressables.LoadAssetsAsync<GameObject>
            (vendigoLabel,prefab => vendigoLabels.Add(prefab)).Completed += process =>
        {
            if (process.Status == AsyncOperationStatus.Succeeded)
            {
                // 풀에 채울 총 몬스터 수를 변수에 담기
                int totalvendigo = vendigoPoolSize; 
                // 리스트에 있는 프리팹 (인덱스 0번) 부터 순서대로 적용
                for (int i = 0; i < totalvendigo; i++)
                {
                    var prefab = vendigoLabels[i % vendigoLabels.Count];
                    PreloadPool(prefab, vendigoPool);
                }
                
            }
            else Debug.LogError($"[{nameof(SpawnManager)}] Vendigo assets 로드 실패");
        };
        
        // 비스트 라벨 불러오기
        Addressables.LoadAssetsAsync<GameObject>
            (beastLabel,prefab => beastLabels.Add(prefab)).Completed += process =>
        {
            if (process.Status == AsyncOperationStatus.Succeeded)
            {
                // 풀에 채울 총 몬스터 수를 변수에 담기
                int totalbeast = beastPoolSize; 
                // 리스트에 있는 프리팹 (인덱스 0번) 부터 순서대로 적용
                for (int i = 0; i < totalbeast; i++)
                {
                    var prefab = beastLabels[i % beastLabels.Count];
                    PreloadPool(prefab, beastPool);
                }
                
            }
            else Debug.LogError($"[{nameof(SpawnManager)}] Beast assets 로드 실패");
        };
    }
    #endregion
    
    private void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReturnAllToPool();
    }

    /// <summary>
    /// 풀에 보관된 몬스터 리스트를 다 돌며, 활성화된 애들은 꺼서 풀로 되돌림
    /// </summary>
    private void ReturnAllToPool()
    {
        // 모든 풀 리스트를 하나씩 순회
        var allPools = new List<List<GameObject>> {
            zombiePool, insectoidPool, ripperPool, vendigoPool, beastPool
        };

        foreach (var pool in allPools)
        {
            foreach (var go in pool)
            {
                if (go.activeInHierarchy)
                    go.SetActive(false);
            }
        }
    }
    // 풀 미리 생성
    private void PreloadPool(GameObject prefab, List<GameObject> pool)
    {
        // 1) 전체 풀 크기 한계 체크
        if (TotalPoolCount() >= totalPoolSize)
            return;

        // 2) 인스턴스 생성 및 세팅
        // Monster 와 각각의 StateMachine 스크립트에서 player응 참조할수 있도록 설정

        var monster = Instantiate(prefab, PrefabsContainer);

        monster.AssignTransform(playerTransform);

        // 3) 비활성화 후 풀에 보관
        monster.SetActive(false);
        pool.Add(monster);
    }
    
    /// <summary>
    /// 지정한 MonsterType의 풀에서 꺼내어 스폰
    /// </summary>
    public void Spawn(MonsterType type)
    {
        if (spawnPoints == null || spawnPoints.Length == 0) 
            return;

        // 1) 스폰 위치 결정
        var spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        // 2) 타입별 풀에서 꺼내 쓰기
        GameObject m = null;
        switch (type)
        {
            case MonsterType.Zombie:
                m = ActivateFromList(zombiePool, zombieLabels, zombiePoolSize);
                break;
            case MonsterType.Insectoid:
                m = ActivateFromList(insectoidPool, insectoidLabels, insectoidPoolSize);
                break;
            case MonsterType.Ripper:
                m = ActivateFromList(ripperPool, ripperLabels, ripperPoolSize);
                break;
            case MonsterType.Vendigo:
                m = ActivateFromList(vendigoPool, vendigoLabels, vendigoPoolSize);
                break;
            case MonsterType.Beast:
                m = ActivateFromList(beastPool, beastLabels, beastPoolSize);
                break;
        }

        // 3) 실제 스폰
        if (m != null)
        {
            m.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            m.SetActive(true);
            m.tag = "Monster";
            m.AssignTransform(playerTransform);
            
        }
    }
    
    private GameObject ActivateFromList(List<GameObject> pool, List<GameObject> variantList,int maxCount)
    {
        // 비활성화된 오브젝트가 있으면 즉시 반환
        foreach (var go in pool)
            if (!go.activeInHierarchy)
                return go;
    
        // 2) 여유가 있으면 새 인스턴스 생성
        if (pool.Count < maxCount && TotalPoolCount() < totalPoolSize)
        {
            var prefab = variantList[Random.Range(0, variantList.Count)];
            var go = Instantiate(prefab, PrefabsContainer);
            go.SetActive(false);
            pool.Add(go);
            return go;
        }
        
        return null;
    }
    
}

