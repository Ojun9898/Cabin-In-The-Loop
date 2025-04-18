using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPoolManager : SingletonMonoBehaviour<CustomPoolManager>
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    [SerializeField] private GameObject prefab;

    void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject obj = Instantiate(prefab, this.transform);
            SetPool(obj);
        }
    }

    public void SetPool(GameObject obj)
    {
        pool.Enqueue(obj);
        obj.SetActive(false);
    }

    public GameObject GetPool()
    {
        if (pool.Count <= 10)
            CreatePool();
        
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);

        return obj;
    }
}