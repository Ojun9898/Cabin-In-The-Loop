using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class R_ApplyStatus : MonoBehaviour, IApplyStatus
{
    [Header("Optional: 이 VFX를 함께 켤 경우 연결")]
    [SerializeField] private B_ApplyEffet vfx;   // 동시에 켜고 싶다면 할당해두기
    
    [Header("Buff Settings")]
    [SerializeField] private float buffDuration = 10f; // 요청: 10초
    private float runSpeedMul  = 2.5f;
    private float runSpeedCap   = 5.0f; // ← 절대 상한(원하면 4.5~5.0)
    private float speedBlendTime = 0.35f; // ← 이 시간 동안 부드럽게 보간
    private float fallbackBaseSpeed = 2.4f;
    
    NavMeshAgent _agent;
    // 백업
    float _origSpeed = -1f;
    Coroutine _co;
    
    void Awake()
    {
        _agent  = GetComponentInParent<NavMeshAgent>(true);
    }
    
    public void ApplyBoostFor(float seconds)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(Co_Boost(seconds > 0f ? seconds : buffDuration));
    }

    IEnumerator Co_Boost(float seconds)
    {
        if (_agent != null && _origSpeed < 0f) _origSpeed = _agent.speed;

        float baseline = (_agent != null && _agent.speed > 0f) ? _agent.speed : fallbackBaseSpeed;
        float target = Mathf.Min(baseline * runSpeedMul, runSpeedCap);

        float left = seconds;
        float k = (speedBlendTime > 0f) ? 1f / speedBlendTime : 999f;

        while (left > 0f)
        {
            left -= Time.deltaTime;
            if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
            {
                float t = 1f - Mathf.Exp(-k * Time.deltaTime);
                _agent.speed = Mathf.Lerp(_agent.speed, target, t);
                if (_agent.speed > runSpeedCap) _agent.speed = runSpeedCap;
            }
            yield return null;
        }

        if (_agent != null && _origSpeed >= 0f) _agent.speed = _origSpeed;
        _co = null;
    }
}
