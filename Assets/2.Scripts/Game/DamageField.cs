using UnityEngine;
using System.Collections;

public class DamageField : MonoBehaviour
{
<<<<<<< HEAD
    [Header("Owner / Spec")]
    private GameObject _owner;              // 이 필드를 생성한 주체
    private float _damage;                  // 줄 데미지
    private float _radius;                  // 기본 반지름(판정 기준)
    private float _duration;                // 지속 시간
=======
    private GameObject _owner;
    private float _damage;
    private float _radius;
    private float _duration;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)

    [Header("Hit Tuning")]
    [Tooltip("판정 반지름에 추가로 더해줄 여유 범위(유닛 발 사이즈, 네비 메쉬 오차 보정 등)")]
    [SerializeField] private float radiusInflation = 0.25f;

    [Tooltip("이펙트 스케일을 판정 반지름(지름)과 자동 동기화")]
    [SerializeField] private bool matchVisualToRadius = true;

    [Tooltip("주기적으로 데미지를 주는 간격(초)")]
    [SerializeField] private float tickInterval = 0.2f;

    [Tooltip("이 필드가 존재하는 동안 지속적으로 판정 수행")]
    [SerializeField] private bool continuousTick = true;

    [Header("Target Filter")]
    [Tooltip("피해를 줄 레이어만 판정 (Player, Monster 등 선택)")]
    [SerializeField] private LayerMask targetLayers = ~0; // 기본: 전부

    [Header("Vertical Tuning")]
    [Tooltip("지면에 납작한 원형 판정보다는 약간의 높이를 허용하고 싶을 때 사용 (캡슐 높이/2)")]
    [SerializeField] private float verticalHalfHeight = 0.4f;

    // 내부 상태
    private float _endTime;
    private Collider[] _buffer;             // NonAlloc용 버퍼
    private const int MaxHits = 64;         // 한 틱에서 최대 감지 수(상황에 맞게 조정)

    // --------- 외부에서 호출 ---------
    /// <summary>
    /// 데미지 필드 초기화
    /// </summary>
    public void Initialize(GameObject owner, float damage, float radius, float duration = 1f)
    {
<<<<<<< HEAD
        // 계층상 비활성 대비 (부모/자식 상태와 관계 없이 활성 계층 보장)
        if (!gameObject.activeInHierarchy)
        {
            // 가능한 한 루트/부모부터 켜야 하는 경우가 있으니,
            // 루트를 찾아서 SetActive(true) -> 자신도 SetActive(true)
            Transform t = transform;
            while (t.parent != null) t = t.parent;
            if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }
        
        _owner = owner;
        _damage = damage;
        _radius = radius;
        _duration = Mathf.Max(0f, duration);
=======
        this._owner = owner;
        this._damage = damage;
        this._radius = radius;
        this._duration = duration;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)

        _endTime = Time.time + _duration;

        // 버퍼 준비
        if (_buffer == null || _buffer.Length != MaxHits)
            _buffer = new Collider[MaxHits];

        // 비주얼 크기를 판정 반지름과 동기화(선택)
        if (matchVisualToRadius)
        {
            // 지름 = 반지름 * 2
            float diameter = Mathf.Max(0.01f, (_radius + radiusInflation) * 2f);
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(diameter, s.y, diameter); // Y 스케일은 파티클/메시 특성에 맞게 유지
        }

        // 즉시 1틱 가하고, 필요하면 코루틴으로 주기적 판정
        DealDamageOnce();

        if (continuousTick && tickInterval > 0f)
            StartCoroutine(TickRoutine());

        // 수명 종료 예약
        Invoke(nameof(Deactivate), _duration);
    }

    // --------- 판정 루프 ---------
    private IEnumerator TickRoutine()
    {
<<<<<<< HEAD
        var wait = new WaitForSeconds(tickInterval);
        while (Time.time < _endTime)
        {
            DealDamageOnce();
            yield return wait;
=======
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
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
        }
    }

    /// <summary>
    /// 한 번의 틱에서 범위 내 타겟에게 데미지를 가함
    /// </summary>
    private void DealDamageOnce()
    {
        // 수직 높이를 허용하는 캡슐 판정 (발 밑 오차 보정에 유리)
        Vector3 center = transform.position;
        float r = _radius + radiusInflation;

        Vector3 p1 = center + Vector3.up * verticalHalfHeight;
        Vector3 p2 = center - Vector3.up * verticalHalfHeight;

        // 레이어 필터 적용 + NonAlloc 사용
        int hitCount = Physics.OverlapCapsuleNonAlloc(p1, p2, r, _buffer, targetLayers, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hitCount; i++)
        {
            var col = _buffer[i];
            if (col == null) continue;

            GameObject target = col.gameObject;

            // 자기 자신 및 같은 팀/레이어 필터(원하는 규칙으로 추가 필터링)
            if (_owner != null)
            {
                // 같은 루트(소유자 자신) 무시
                if (target.transform.root == _owner.transform.root) 
                    continue;
            }

            // IDamageable이면 데미지 처리
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                int playerLayer = LayerMask.NameToLayer("Player");
                int monsterLayer = LayerMask.NameToLayer("Monster");

                if (target.layer == playerLayer)
                {
                    if (target.TryGetComponent<PlayerStateMachine>(out var psm))
                    {
                        if (psm.canHit)
                            damageable.TakeDamage(_damage);
                    }
                    // else: 플레이어가지만 PSM 없으면 스킵 (게임 규칙에 맞춰 조정 가능)
                }
                else if (target.layer == monsterLayer)
                {
                    damageable.TakeDamage(_damage);
                }
                else
                {
                    // 필요 시 기타 레이어에도 적용 가능
                    // damageable.TakeDamage(_damage);
                }
            }

            _buffer[i] = null; // 다음 틱을 위해 비움
        }
    }

    /// <summary>
    /// 필드를 비활성화하고 풀에 반환
    /// </summary>
    private void Deactivate()
    {
        // 코루틴/예약 정리
        StopAllCoroutines();
        CancelInvoke();

        // 오브젝트 풀로 반환
        GameManager.Instance.ReturnDamageField(gameObject);
    }

    // ---- 디버깅용 기즈모 ----
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        float r = (_radius > 0f ? _radius : 0.5f) + radiusInflation;
        Vector3 p1 = center + Vector3.up * verticalHalfHeight;
        Vector3 p2 = center - Vector3.up * verticalHalfHeight;
        // 캡슐 대략 표현: 상/하 구체 + 선
        Gizmos.DrawWireSphere(p1, r);
        Gizmos.DrawWireSphere(p2, r);
        Gizmos.DrawLine(p1 + Vector3.right * r, p2 + Vector3.right * r);
        Gizmos.DrawLine(p1 - Vector3.right * r, p2 - Vector3.right * r);
        Gizmos.DrawLine(p1 + Vector3.forward * r, p2 + Vector3.forward * r);
        Gizmos.DrawLine(p1 - Vector3.forward * r, p2 - Vector3.forward * r);
    }
}
