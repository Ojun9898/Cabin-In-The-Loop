using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class B_ApplyEffet : MonoBehaviour
{
    [Header("VFX Root (켜고/끄는 오브젝트)")]
    public GameObject vfxRoot;

    [Header("재트리거 정책")]
    public bool extendDurationOnRetrigger = true;
    
    [Header("VFX 재생속도 제어 (VisualEffect.playRate)")]
    [Tooltip("켜질 때 전체 시뮬레이션 속도(오버레이 Rate)를 몇 배로 재생할지")]
    public bool setPlayRateOnActivate = true;
    [Tooltip("1=기본속도, 3=3배속, 10=10배속")]
    public float playRateOnActivate = 3f;
    [Tooltip("끝나면 원래 playRate로 복구할지")]
    public bool restorePlayRateOnEnd = true;
    
    // ★ 추가: Run이 시작될 때 루트의 ApplyStatus를 자동 적용할지
    [Header("상태 버프(2-1 효과) 동시 적용")]
    [SerializeField] private bool applyStatusOnRun = true;

    Coroutine _co;
    float _remain = 0f;
    
    struct VfxPlayRateBackup { public VisualEffect vfx; public float original; }
    List<VfxPlayRateBackup> _playRateBackups = new List<VfxPlayRateBackup>();
    VisualEffect[] _vfxs;

    void Awake()
    {
        if (vfxRoot) vfxRoot.SetActive(false);
        _vfxs = (vfxRoot ? vfxRoot : gameObject).GetComponentsInChildren<VisualEffect>(true);
    }

    public void ActivateFor(float durationSeconds)
    {
        if (durationSeconds <= 0f) return;

        if (_co == null) _co = StartCoroutine(Run(durationSeconds));
        else _remain = extendDurationOnRetrigger ? Mathf.Max(_remain, durationSeconds) : durationSeconds;
    }

    IEnumerator Run(float firstDuration)
    {
        _remain = firstDuration;

        // 1) VFX 오브젝트 켜기
        if (vfxRoot) vfxRoot.SetActive(true);
        
        // 2) Play Rate(시뮬레이션 속도) 백업/설정
        _playRateBackups.Clear();
        if (setPlayRateOnActivate && _vfxs != null)
        {
            foreach (var v in _vfxs)
            {
                if (!v) continue;
                _playRateBackups.Add(new VfxPlayRateBackup { vfx = v, original = v.playRate });
                v.playRate = playRateOnActivate;   // ★ 전체 재생 속도 배율
            }
        }
        
        // ★★★ 여기! 상태 버프(2-1 효과) 동시 적용
        if (applyStatusOnRun)
        {
            ApplyStatusOnRoot(_remain); // 루트의 Z/R/V/I 중 하나만 적용
        }
        
        // 유지 시간
        while (_remain > 0f)
        {
            _remain -= Time.deltaTime;
            yield return null;
        }
        
        // 3) Play Rate 복구
        if (restorePlayRateOnEnd && _playRateBackups.Count > 0)
        {
            foreach (var b in _playRateBackups)
                if (b.vfx) b.vfx.playRate = b.original;
        }

        // 4) VFX 끄기
        if (vfxRoot) vfxRoot.SetActive(false);
        _co = null;
    }
    
    void ApplyStatusOnRoot(float seconds)
    {
        var applier = GetComponentInParent<IApplyStatus>();
        if (applier != null) applier.ApplyBoostFor(seconds);
    }
}
