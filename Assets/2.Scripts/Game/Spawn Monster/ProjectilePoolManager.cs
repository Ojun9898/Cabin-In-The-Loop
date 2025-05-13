using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int initialSize = 10;
        [HideInInspector] public Queue<GameObject> projectiles;
    }
    
    [Header("풀링할 투사체들")]
    public List<Pool> pools;
    
    // prefab → 실제 풀(Queue) 매핑
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    
    void Awake()
    {
        // 1. 싱글턴 중복방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 2. Dictionary 초기화
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        // 3. 각 풀 초기화 (모두 this.transform의 자식으로 생성)
        foreach (var pool in pools)
        {
            // 빈큐 생성
            var q = new Queue<GameObject>();
            // 초기 인스턴스 생성
            for (int i = 0; i < pool.initialSize; i++)
            {
                var obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.SetActive(false);

                var poolable = obj.AddComponent<PoolableProjectile>();
                poolable.originPrefab = pool.prefab;
                // 큐에 등록
                q.Enqueue(obj);
            }
            // 만든 Queue 를 Pool 구조체 내부의 projectiles에 저장
            pool.projectiles = q;
            // “pool.prefab = Key, q = Value"
            poolDictionary[pool.prefab] = q;
        }
    }
    
    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 1) 풀에 해당 prefab이 등록되어 있는지 확인
        if (!poolDictionary.TryGetValue(prefab, out var q))
        {
            Debug.LogWarning($"[{nameof(ProjectilePoolManager)}] 풀에 등록되지 않은 prefab: {prefab.name}");
            return Instantiate(prefab, position, rotation, transform);
        }

        GameObject obj;
        // 2) 큐에 사용 가능한 객체가 남아 있으면
        if (q.Count > 0)
        {
            // 큐에서 꺼내기
            obj = q.Dequeue();
        }
        else
        {
            // 풀 사이즈 초과 시 새로 생성하면서 부모 지정
            obj = Instantiate(prefab, position, rotation, transform);
            var poolable = obj.AddComponent<PoolableProjectile>();
            poolable.originPrefab = prefab;
        }

        // 위치/회전을 초기화 및 활성화
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        // Rigidbody 초기화 (velocity 0)
        if (obj.TryGetComponent<Rigidbody>(out var rb))
<<<<<<< HEAD
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // ★ 추가: 회전 잔여값 제거
        }

        // 자동으로 일정 시간 뒤 되돌려놓기
        if (obj.TryGetComponent<PoolableProjectile>(out var p)) p.inPool = false;

        float lifetime = ProjectileLife.GetLifetime(obj);
        StartCoroutine(ReturnToPoolAfterTime(obj, lifetime));
=======
            rb.velocity = Vector3.zero;

        // 자동으로 일정 시간 뒤 되돌려놓기
        float lifetime = ProjectileLife.GetLifetime(obj);
        StartCoroutine(ReturnToPoolAfterTime(obj, lifetime));

>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
        return obj;
    }
    
    IEnumerator ReturnToPoolAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj == null) yield break;
        // 풀로 반환
        ReturnToPool(obj);
    }
    
    /// <summary>
    /// 오브젝트를 비활성화하고 해당 풀에 다시 넣음
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

<<<<<<< HEAD
        var poolable = obj.GetComponent<PoolableProjectile>();
        // ★ 이미 풀에 들어간 상태라면 무시
        if (poolable != null && poolable.inPool) return;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.SetActive(false);

        if (poolable != null && poolDictionary.TryGetValue(poolable.originPrefab, out var q))
        {
            poolable.inPool = true;  // ★ 이제 풀 상태
=======
        obj.SetActive(false);
        var poolable = obj.GetComponent<PoolableProjectile>();
        // 풀링 시스템이 관리하는 객체이며, poolDictionary에 originPrefab 키가 있다면 해당 풀(큐)을 q에 가져옴
        if (poolable != null && poolDictionary.TryGetValue(poolable.originPrefab, out var q))
        {
            // 비활성화된 오브젝트를 큐에 넣기
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
            q.Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}


// Spawn/ReturnToPool 시 원본 prefab을 식별하기 위한 컴포넌트

public class PoolableProjectile : MonoBehaviour
{
    [HideInInspector] public GameObject originPrefab;
<<<<<<< HEAD
    [HideInInspector] public bool inPool;
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
}

// 투사체들의 수명
public static class ProjectileLife
{
    public static float GetLifetime(GameObject obj)
    {
        if (obj.TryGetComponent<Projectile>(out var proj))
        {
            return 4.0f;
        }
        // Insect 등 다른 투사체 기본 수명
        return 5f;
    }
}
