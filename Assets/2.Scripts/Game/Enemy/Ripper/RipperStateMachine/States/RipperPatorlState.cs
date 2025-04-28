using UnityEngine;
using UnityEngine.AI;

public class RipperPatorlState : RipperBaseState
{
    private const float idleDuration = 1.5f;  // 도착 후 멈춰 있을 시간
    private float idleTimer = 0f;
    private bool isIdling = false;
    
    private const float WANDER_DURATION = 5f;
    private const float WANDER_RADIUS = 16f;
    private const float CHASE_RANGE = 6f;
    
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
            Debug.LogWarning("NavMeshAgent component not found on Monster. Adding it automatically.");
            navMeshAgent = owner.gameObject.AddComponent<NavMeshAgent>();
        
            // 기본 설정
            navMeshAgent.speed = 1.5f;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.angularSpeed = 120f;
            navMeshAgent.stoppingDistance = 0.1f;
        }
    }
    
    public override void EnterState()
    {
        base.EnterState();
        if (navMeshAgent != null)
            navMeshAgent.speed = defaultSpeed;

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
            stateTimer = 0f; // Wander 타이머 초기화
            lastWanderTime = Time.time; // 쿨다운 초기화
            isIdling = false; // 혹시 남아있던 플래그 초기화
        }
        
        PlayAnimation("Ripper Walk");
        SetNewWanderTarget();
        
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
    
        bool inRange = IsPlayerInRange(CHASE_RANGE);
        // 범위 안일 때만 직접 회전, 아닐 때는 NavMesh 회전
        if (navMeshAgent != null)
            navMeshAgent.updateRotation = !inRange;
        if (inRange)
        {
            RotateTowards(playerTransform.position, 6f);
            stateTimer = WANDER_DURATION; // 즉시 Chase로 전환
            return;
        }
        
        // 도착 즉시 Idle 상태로 진입
        if (!isIdling && navMeshAgent != null && navMeshAgent.remainingDistance < 0.5f)
        {
            isIdling = true;
            idleTimer = 0f;
            navMeshAgent.isStopped = true; // 멈추고
            PlayAnimation("Ripper Idle");  // Idle 애니 재생
            return;
        }
    
        // Idle 중에는 타이머만 돌리기
        if (isIdling)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                // Idle 끝나면 다시 Walk & 새로운 목적지
                isIdling = false;
                navMeshAgent.isStopped = false;
                PlayAnimation("Ripper Walk");
                SetNewWanderTarget();
                lastWanderTime = Time.time;
                stateTimer = 0f;
            }
            return;
        }
        
        // 평소 Wander 지속 시간 초과 시에도 새로운 목적지
        if (stateTimer >= WANDER_DURATION && Time.time - lastWanderTime >= wanderCooldownTime)
        {
            SetNewWanderTarget();
            lastWanderTime = Time.time;
            stateTimer = 0f;
        }
    }
    
    private void SetNewWanderTarget()
    {
        if (navMeshAgent == null) return;
    
        // XZ 평면에서만 랜덤 방향을 뽑도록 insideUnitCircle 사용
        Vector2 rand = Random.insideUnitCircle * WANDER_RADIUS;
        Vector3 candidate = ripper.transform.position + new Vector3(rand.x, 0f, rand.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, WANDER_RADIUS, NavMesh.AllAreas))
        {
            // 현재 위치와 너무 가까우면 재시도
            if (Vector3.Distance(ripper.transform.position, hit.position) < 1f)
            {
                // 1m 미만이면 버리고 재귀 또는 그냥 다음 프레임에 다시 호출
                return;
            }

            navMeshAgent.SetDestination(hit.position);
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
