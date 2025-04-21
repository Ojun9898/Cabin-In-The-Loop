using UnityEngine;
using UnityEngine.Pool;

public class PoolItem : MonoBehaviour
{
    private IObjectPool<PoolItem> m_poolManager;
    
    public void SetPoolManager(IObjectPool<PoolItem> _pool)
    {
        m_poolManager = _pool;
    }
    
    public void OnSetPoolItem()
    {
        m_poolManager.Release(this);
    }
}