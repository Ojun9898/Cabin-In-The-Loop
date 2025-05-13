using UnityEngine;

public class PoisonThorn : MonoBehaviour
{
    private float damageAmount;
<<<<<<< HEAD
    private bool _returned;  // 중복 반환 방지
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
    private float lifeTime = 5f; // 발사체 수명

    public void Initialize(float damage)
    {
        damageAmount = damage;
<<<<<<< HEAD
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
=======
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동으로 파괴
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지 적용
            PlayerStatus playerHealth = other.GetComponent<PlayerStatus>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // 충돌 후 발사체 파괴
            Destroy(gameObject);
        }
    }
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
}