using UnityEngine;

public class DamageField : MonoBehaviour
{
    private GameObject owner;
    private float damage;
    private float radius;
    private float duration;

    /// <summary>
    /// 필드 초기화 시 owner를 함께 넘깁니다.
    /// owner는 이 필드를 생성(발동)한 주체(GameObject)입니다.
    /// </summary>
    public void Initialize(GameObject owner, float damage, float radius, float duration = 1f)
    {
        this.owner = owner;
        this.damage = damage;
        this.radius = radius;
        this.duration = duration;

        Invoke(nameof(Deactivate), duration);
        DealDamage();
    }

    private void DealDamage()
    {
        // 플레이어 자신의 콜라이더는 제외하도록 필터링
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) 
                continue;

            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void Deactivate()
    {
        // gameObject.SetActive(false);
        GameManager.Instance.ReturnDamageField(gameObject);
    }
}