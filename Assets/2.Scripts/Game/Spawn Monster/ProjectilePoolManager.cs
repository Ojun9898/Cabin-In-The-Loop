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
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogError("[ProjectilePoolManager] Pool에 prefab 이 비어 있습니다.");
                continue;
            }

            string key = pool.prefab.name; // ★ 이름을 키로 사용

            var q = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                var obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);

                var poolable = obj.GetComponent<PoolableProjectile>();
                if (poolable == null) poolable = obj.AddComponent<PoolableProjectile>();

                poolable.originPrefab = pool.prefab;
                poolable.poolKey      = key;   // ★ 어떤 풀에서 나왔는지 기록
                poolable.inPool       = true;

                q.Enqueue(obj);
            }

            pool.projectiles = q;
            poolDictionary[key] = q; // ★ 이름 -> 큐
            Debug.Log($"[ProjectilePoolManager] 등록된 풀 키: {key}");
        }
    }
    
    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("[ProjectilePoolManager] SpawnFromPool 에 null prefab 이 들어왔습니다.");
            return null;
        }

        string key = prefab.name;  // ★ 이름을 키로

        if (!poolDictionary.TryGetValue(key, out var q))
        {
            // 여기서도 여전히 "정말 등록이 안 돼 있는 경우"에만 경고
            Debug.LogWarning($"[ProjectilePoolManager] 풀에 등록되지 않은 prefab: {key}");
            return Instantiate(prefab, position, rotation, transform);
        }

        GameObject obj;
        if (q.Count > 0)
        {
            obj = q.Dequeue();
        }
        else
        {
            // 풀에 부족하면 새로 생성해서 같은 풀에 넣을 준비
            obj = Instantiate(prefab, position, rotation, transform);
            var poolable = obj.GetComponent<PoolableProjectile>();
            if (poolable == null) poolable = obj.AddComponent<PoolableProjectile>();

            poolable.originPrefab = prefab;
            poolable.poolKey      = key;   // ★ 이 투사체도 같은 키를 사용
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity        = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (obj.TryGetComponent<PoolableProjectile>(out var p))
            p.inPool = false;

        float lifetime = ProjectileLife.GetLifetime(obj);
        StartCoroutine(ReturnToPoolAfterTime(obj, lifetime));

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

        var poolable = obj.GetComponent<PoolableProjectile>();
        if (poolable != null && poolable.inPool) return;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity        = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.SetActive(false);

        if (poolable != null && !string.IsNullOrEmpty(poolable.poolKey)
                             && poolDictionary.TryGetValue(poolable.poolKey, out var q))
        {
            poolable.inPool = true;
            q.Enqueue(obj);
        }
        else
        {
            // 어떤 풀에도 속하지 않는다면 그냥 파괴
            Destroy(obj);
        }
    }
}


// Spawn/ReturnToPool 시 원본 prefab을 식별하기 위한 컴포넌트
public class PoolableProjectile : MonoBehaviour
{
    [HideInInspector] public GameObject originPrefab;
    [HideInInspector] public bool inPool;
    
    [HideInInspector] public string poolKey;
}

// 투사체들의 수명
public static class ProjectileLife
{
    public static float GetLifetime(GameObject obj)
    {
        if (obj.TryGetComponent<V_Projectile>(out var proj))
        {
            return 4.0f;
        }
        // Insect 등 다른 투사체 기본 수명
        return 5f;
    }
}
