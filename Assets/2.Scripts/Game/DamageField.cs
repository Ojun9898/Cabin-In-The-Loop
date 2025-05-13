using UnityEngine;

public class DamageField : MonoBehaviour
{
    private GameObject _owner;
    private float _damage;
    private float _radius;
    private float _duration;

    /// <summary>
    /// 필드 초기화 시 owner를 함께 넘깁니다.
    /// owner는 이 필드를 생성(발동)한 주체(GameObject)입니다.
    /// </summary>
    public void Initialize(GameObject owner, float damage, float radius, float duration = 1f)
    {
        this._owner = owner;
        this._damage = damage;
        this._radius = radius;
        this._duration = duration;

        Invoke(nameof(Deactivate), duration);
        DealDamage();
    }

    private void DealDamage()
    {
        // 플레이어 자신의 콜라이더는 제외하도록 필터링
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == _owner.layer) 
                continue;

            if (hit.gameObject.layer != _owner.layer && hit.TryGetComponent<Monster>(out var target0))
            {
                Debug.Log("Monster HIT!");
                target0.TakeDamage(_damage);
            }
            
            if (hit.gameObject.layer != _owner.layer && hit.TryGetComponent<PlayerStatus>(out var target1))
            {
                target1.TakeDamage(_damage);
            }
        }
    }

    private void Deactivate()
    {
        // gameObject.SetActive(false);
        GameManager.Instance.ReturnDamageField(gameObject);
    }
}