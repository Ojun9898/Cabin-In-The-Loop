using UnityEngine;

public class DamageField : MonoBehaviour
{
    private GameObject _owner;              // 이 필드를 생성한 주체
    private float _damage;                  // 줄 데미지 양
    private float _radius;                  // 공격 범위 반지름
    private float _duration;                // 필드 지속 시간

    private float _elapsedTime;             // 누적 시간 (지속시간 초과 시 비활성화)

    /// <summary>
    /// 데미지 필드 초기화 함수
    /// </summary>
    public void Initialize(GameObject owner, float damage, float radius, float duration = 1f)
    {
        _owner = owner;
        _damage = damage;
        _radius = radius;
        _duration = duration;


        _elapsedTime = 0f;

        Invoke(nameof(Deactivate), duration);    // 지정된 시간이 지나면 자동으로 비활성화
        DealDamage();       // 일정 간격마다 데미지를 주는 코루틴 시작
    }

    /// <summary>
    /// 범위 내 타겟에게 데미지를 가함 (중복 방지 포함)
    /// </summary>
    private void DealDamage()
    {
        // 반지름 내의 모든 콜라이더 감지
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);

        foreach (var hit in hits)
        {
            // 자기 자신 혹은 같은 팀은 무시 (ex. 플레이어가 생성한 경우 플레이어는 무시)
            if (hit.gameObject.layer == _owner.layer)
                continue;

            GameObject target = hit.gameObject;

            // 각 타겟이 쿨다운 검사 및 데미지 적용을 직접 처리
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (target.layer == LayerMask.NameToLayer("Player"))
                {
                    target.TryGetComponent<PlayerStateMachine>(out var playerStateMachine);
                    if (playerStateMachine.canHit)
                    {
                        damageable.TakeDamage(_damage);
                    }
                }

                else if(target.layer == LayerMask.NameToLayer("Monster"))
                {
                    damageable.TakeDamage(_damage);
                }
            }
        }
    }

    /// <summary>
    /// 필드를 비활성화하고 풀에 반환
    /// </summary>
    private void Deactivate()
    {
        GameManager.Instance.ReturnDamageField(gameObject);
    }
}
