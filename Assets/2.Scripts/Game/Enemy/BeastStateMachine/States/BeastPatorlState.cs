using UnityEngine;
using UnityEngine.AI;

public class BeastPatorlState : BeastBaseState
{
    private const float WANDER_DURATION = 5f;
<<<<<<< HEAD
<<<<<<< HEAD
    private const float WANDER_RADIUS = 18f;
=======
    private const float WANDER_RADIUS = 8f;
    private const float CHASE_RANGE = 6f;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
    private const float WANDER_RADIUS = 8f;
    private const float CHASE_RANGE = 6f;
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    
    private Vector3 wanderTarget;
    private NavMeshAgent navMeshAgent;
    private float wanderCooldownTime = 3f; // 목표 변경 최소 간격
    private float lastWanderTime = 0f;     // 마지막으로 목표 변경한 시간
    
    protected override void SetStateKey()
    {
        stateKey = EState.Wander;
    }
    
    public override void Initialize(Monster owner)
    {
        base.Initialize(owner);
        
        // NavMeshAgent 컴포넌트 가져오기
        navMeshAgent = owner.gameObject.GetComponent<NavMeshAgent>();
        
        // NavMeshAgent가 없는 경우 추가
        if (navMeshAgent == null)
        {
<<<<<<< HEAD
<<<<<<< HEAD
=======
            Debug.LogWarning("NavMeshAgent component not found on Monster. Adding it automatically.");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
            Debug.LogWarning("NavMeshAgent component not found on Monster. Adding it automatically.");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
            navMeshAgent = owner.gameObject.AddComponent<NavMeshAgent>();
            
            // 기본 설정
            navMeshAgent.speed = 1.5f;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.angularSpeed = 120f;
            navMeshAgent.stoppingDistance = 0.1f;
            navMeshAgent.updateRotation = true;
            navMeshAgent.updatePosition = true;
        }
    }
    
    public override void EnterState()
    {
        base.EnterState();
        PlayAnimation("Walk");
        SetNewWanderTarget();
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        
        // 플레이어가 감지 범위 안에 들어오면 추적 상태로 전환
        if (IsPlayerInChaseRange())
        {
            stateTimer = WANDER_DURATION; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
            return;
        }
        
        // 목표 지점에 도달했거나 일정 시간이 지났으면 새로운 목표 지점 설정
        if (navMeshAgent != null && !navMeshAgent.pathPending && navMeshAgent.isActiveAndEnabled)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || stateTimer >= WANDER_DURATION)
            {
                if (Time.time - lastWanderTime >= wanderCooldownTime)
                {
                    SetNewWanderTarget();
                    lastWanderTime = Time.time;
                    stateTimer = 0f;
                }
            }
        }
    }
    
    private void SetNewWanderTarget()
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled) return;
    
        Vector3 randomDirection = Random.insideUnitSphere * WANDER_RADIUS;
        randomDirection += beast.transform.position; // 현재 위치를 기준으로 랜덤 방향
    
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, WANDER_RADIUS, NavMesh.AllAreas))
        {
            wanderTarget = hit.position; // 유효한 위치를 타겟으로 설정
            navMeshAgent.isStopped = false; // 이동 활성화
            navMeshAgent.SetDestination(wanderTarget); // 타겟으로 이동 시작
<<<<<<< HEAD
<<<<<<< HEAD
            
        }
        else
        {
            
=======
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
            Debug.Log($"New Wander Target Set: {wanderTarget}"); // 디버그 출력
        }
        else
        {
            Debug.LogWarning("Failed to find a valid position for wandering on the NavMesh.");
<<<<<<< HEAD
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Wander;
        
        if (IsPlayerInChaseRange())
        {
            nextState = EState.Chase;
            return true;
        }

        if (IsPlayerInHowlRange())
        {
            nextState = EState.Rattack;
        }
        
        return false;
    }
} 