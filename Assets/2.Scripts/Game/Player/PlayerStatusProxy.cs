using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStatusProxy : MonoBehaviour, IDamageable
{
    // PlayerStatus 싱글톤 캐시 (없으면 필요 시 자동 확보)
    private PlayerStatus _status;

    /// <summary>
    /// 전역 PlayerStatus 접근자. 캐시되어 있지 않으면 Ensure로 보장 후 캐싱.
    /// </summary>
    public PlayerStatus Status
    {
        get
        {
            if (_status == null)
                _status = PlayerStatus.Ensure();
            return _status;
        }
    }

    [Header("자동 캐시")]
    [Tooltip("Awake에서 PlayerStatus를 미리 확보합니다.")]
    [SerializeField] private bool cacheOnAwake = true;

    private void Awake()
    {
        if (cacheOnAwake)
            _status = PlayerStatus.Ensure();
    }

    // ===== IDamageable 위임 =====
    public void TakeDamage(float amount)
    {
        // 널 가드 (이론상 Ensure가 항상 반환하지만, 예외 상황 대비)
        var s = Status;
        if (s != null) s.TakeDamage(amount);
    }

    // ===== 선택: 경험치/스탯 위임(픽업/트리거에서 GetComponent로 접근하는 경우 대비) =====
    public void GainXp(float amount)
    {
        var s = Status;
        if (s != null) s.GainXp(amount);
    }

    public float GetStat(StatusType type)
    {
        var s = Status;
        return s != null ? s.GetTotalStat(type) : 0f;
    }

    // IDamageable에 다른 시그니처(예: TryTakeDamage)가 있다면 동일하게 위임 메서드 추가:
    // public void TryTakeDamage(DamageField src, float damage, float cooldown) => Status?.TryTakeDamage(src, damage, cooldown);
}