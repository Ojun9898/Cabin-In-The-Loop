using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject[] poolItemPrefabs; // 여러 개의 프리팹 저장
    private Dictionary<string, IObjectPool<PoolItem>> d_pools = new Dictionary<string, IObjectPool<PoolItem>>();


    void Awake()
    {
        foreach (var poolItemPrefab in poolItemPrefabs)
        {
            string key = poolItemPrefab.name;
            
            d_pools[key] = new ObjectPool<PoolItem>(
                () => CreatePoolItem(poolItemPrefab),
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem,
                maxSize: 100
            );
        }
    }
    
    public GameObject GetPoolObject(GameObject prefab)
    {
        if (d_pools.ContainsKey(prefab.name))
        {
            PoolItem item = d_pools[prefab.name].Get();
            return item.gameObject;
        }

        return null;
    }
    
    private PoolItem CreatePoolItem(GameObject prefab)
    {
        PoolItem poolItem = Instantiate(prefab, this.transform).GetComponent<PoolItem>();
        poolItem.name = prefab.name;
        poolItem.SetPoolManager(d_pools[poolItem.name]); // 해당 프리팹의 풀을 연결
        
        return poolItem;
    }

    private void OnGetItem(PoolItem _item)
    {
        _item.gameObject.SetActive(true);
    }

    private void OnReleaseItem(PoolItem _item)
    {
        _item.gameObject.SetActive(false);
    }

    private void OnDestroyItem(PoolItem _item)
    {
        Destroy(_item.gameObject);
    }
}