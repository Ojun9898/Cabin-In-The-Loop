using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class B_ApplyEffet : MonoBehaviour
{
    [Header("분노VFX")]
    public GameObject rageVFX;
    // 코루틴 실행중에 코루틴신호가 들어왔을떄 시간연장
    public bool extendDuration = true;
    
    [Header("VFX 재생속도 제어 (VisualEffect.playRate)")]
    [Tooltip("1=기본속도, 3=3배속, 10=10배속")]
    public float playRateOnActivate = 3f;
    
    Coroutine _co;
    float _remainTime = 0f;
    
    // 원래 vfx의 속도를 저장할 백업 컨테이너
    struct VfxPlayRateBackup { public VisualEffect vfx; public float original; }
    List<VfxPlayRateBackup> _playRateBackups = new List<VfxPlayRateBackup>();
    
    VisualEffect[] _vfxs;

    void Awake()
    {
        if (rageVFX) rageVFX.SetActive(false);
        _vfxs = (rageVFX ? rageVFX : gameObject).GetComponentsInChildren<VisualEffect>(true);
    }

    public void ActivateFor(float durationSeconds)
    {
        if (durationSeconds <= 0f) return;

        if (_co == null) _co = StartCoroutine(RageVFX(durationSeconds));
        // 코루틴 도중에 또 코루틴 신호가 들어왔다면 시간을 연장(_remainTime 이 3초면 10초로 연장)
        else _remainTime = extendDuration ? Mathf.Max(_remainTime, durationSeconds) : durationSeconds;
    }

    IEnumerator RageVFX(float firstDuration)
    {
        _remainTime = firstDuration;

        // 1) VFX 오브젝트 켜기
        if (rageVFX) rageVFX.SetActive(true);
        
        // 2) Play Rate(시뮬레이션 속도) 백업/설정
        _playRateBackups.Clear();
        if (_vfxs != null)
        {
            // 아직 1개의 VFX를 사용하고 있지만, 향후 확장성과 GC를 유발하지 않기 때문에 foreach를 사용 
            foreach (var v in _vfxs)
            {
                if (!v) continue;
                // 원래 playRate 값을 백업 컨테이너에 저장
                _playRateBackups.Add(new VfxPlayRateBackup { vfx = v, original = v.playRate });
                // VisualEffect의 시뮬레이션 시간(Delta Time)에 playRateOnActivate 값을 곱함
                v.playRate = playRateOnActivate;   
            }
        }
        
        // 버프효과(이동속도 상승) 동시적용
        // 루트의 Z/R/V/I 중 하나만 적용
        ApplyStatusOnRoot(_remainTime); 
        
        // 유지 시간
        while (_remainTime > 0f)
        {
            _remainTime -= Time.deltaTime;
            yield return null;
        }
        
        // 3) Play Rate 복구
        if (_playRateBackups.Count > 0)
        {
            foreach (var b in _playRateBackups)
                if (b.vfx) b.vfx.playRate = b.original;
        }

        // 4) VFX 끄기
        if (rageVFX) rageVFX.SetActive(false);
        _co = null;
    }
    
    void ApplyStatusOnRoot(float seconds)
    {
        var applier = GetComponent<IApplyStatus>();
        if (applier != null) applier.ApplyBoostFor(seconds);
    }
}
