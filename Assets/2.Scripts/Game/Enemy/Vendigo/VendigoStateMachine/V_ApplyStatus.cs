using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class V_ApplyStatus : MonoBehaviour, IApplyStatus
{
    [Header("vfx 오브젝트")]
    [SerializeField] private B_ApplyEffet vfx; 
    
    // 버프 셋팅
    private float buffDuration = 10f; // 버트 유지 기간
    private float accelSpeed  = 2.5f;
    private float speedMax   = 5.0f; // 절대 상한
    private float fallbackBaseSpeed = 2.0f; // Agent.speed가 0이면 사용
    
    NavMeshAgent _agent;
    
    // 백업
    float _orignalSpeed = -1f;
    Coroutine _co;
    
    void Awake()
    {
        _agent  = GetComponentInParent<NavMeshAgent>(true);
    }

    
    public void ApplyBoostFor(float seconds)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(BoostRun(seconds > 0f ? seconds : buffDuration));
    }

    // 코루틴중에는 이동속도 증가
    IEnumerator BoostRun(float seconds)
    {
        if (_agent == null) yield break;

        // 매번 현재 속도를 백업(가급적 최신 평속을 복원하고 싶다면 이렇게)
        _orignalSpeed = _agent.speed;

        // 목표 속도 계산
        float baseSpeed   = (_agent.speed > 0f) ? _agent.speed : fallbackBaseSpeed;
        float targetSpeed = Mathf.Min(baseSpeed * accelSpeed, speedMax);

        // 보간 없이 즉시 속도 증가
        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.speed = targetSpeed;

        // 지속 시간 감소
        float left = (seconds > 0f) ? seconds : 0f;
        // left < 0 이 될때까지 매 프레임마다 반복
        while (left > 0f)
        {
            left -= Time.deltaTime;
            yield return null;
        }

        // 곧바로 원래 속도로 복구
        if (_agent.enabled)
            _agent.speed = _orignalSpeed;

        _co = null;
    }
}
