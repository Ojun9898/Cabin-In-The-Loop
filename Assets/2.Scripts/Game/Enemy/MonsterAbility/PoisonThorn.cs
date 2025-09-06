using UnityEngine;

public class PoisonThorn : MonoBehaviour
{
    private float damageAmount;
    private bool _returned;  // 중복 반환 방지
    private float lifeTime = 5f; // 발사체 수명

    public void Initialize(float damage)
    {
        damageAmount = damage;
        _returned = false;   // 풀에서 다시 꺼내면 리셋
    }
    
    private void OnDisable()
    {
        // 풀로 돌아가 비활성화되었다가 재사용될 때를 대비해 초기화
        _returned = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_returned) return;

        // 플레이어 전용 판정
        var proxy = other.GetComponentInParent<PlayerStatusProxy>();
        if (proxy != null)
        {
            proxy.TakeDamage(damageAmount);
            ReturnToPool();
            return;
        }

        var player = other.GetComponentInParent<PlayerStatus>();
        if (player != null)
        {
            player.TakeDamage(damageAmount);
            ReturnToPool();
            return;
        }
    }
    
    private void ReturnToPool()
    {
        _returned = true;
        ProjectilePoolManager.Instance.ReturnToPool(gameObject);
    }
}