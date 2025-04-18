using UnityEngine;
using UnityEngine.AI;

public class ZombieWanderState : ZombieBaseState
{
    private const float WANDER_DURATION = 5f;
    private const float WANDER_RADIUS = 10f;
    private const float CHASE_RANGE = 8f;
    
    private Vector3 wanderTarget;
    private NavMeshAgent navMeshAgent;
    
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
            Debug.LogWarning("NavMeshAgent component not found on Monster. Adding it automatically.");
            navMeshAgent = owner.gameObject.AddComponent<NavMeshAgent>();
            
            // 기본 설정
            navMeshAgent.speed = 1.5f; // 기본 이동 속도
            navMeshAgent.acceleration = 8f; // 기본 가속도
            navMeshAgent.angularSpeed = 120f; // 기본 회전 속도
            navMeshAgent.stoppingDistance = 0.1f; // 기본 정지 거리
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
        if (IsPlayerInRange(CHASE_RANGE))
        {
            stateTimer = WANDER_DURATION; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
            return;
        }
        
        // 목표 지점에 도달했거나 일정 시간이 지났으면 새로운 목표 지점 설정
        if (navMeshAgent != null && (navMeshAgent.remainingDistance < 0.5f || stateTimer >= WANDER_DURATION))
        {
            SetNewWanderTarget();
            stateTimer = 0f;
        }
    }
    
    private void SetNewWanderTarget()
    {
        if (navMeshAgent == null) return;
        
        // 현재 위치에서 랜덤한 방향으로 이동
        Vector3 randomDirection = Random.insideUnitSphere * WANDER_RADIUS;
        randomDirection += zombie.transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, WANDER_RADIUS, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            navMeshAgent.SetDestination(wanderTarget);
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Wander;
        
        if (IsPlayerInRange(CHASE_RANGE))
        {
            nextState = EState.Chase;
            return true;
        }
        
        return false;
    }
} 