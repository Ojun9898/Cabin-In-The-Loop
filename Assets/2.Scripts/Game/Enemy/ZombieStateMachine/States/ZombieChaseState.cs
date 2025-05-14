using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : ZombieBaseState
{
    private const float CHASE_RANGE     = 20f;
    private const float ATTACK_RANGE    = 1.5f;
    private const float REPATH_INTERVAL = 0.5f;  // 재탐색 간격

    private Transform   playerTransform;
    private NavMeshAgent navMeshAgent;
    private float       lastRepathTime;

    protected override void SetStateKey() => stateKey = EState.Chase;

    public override void Initialize(Monster owner)
    {
        base.Initialize(owner);

        // NavMeshAgent 캐싱
        navMeshAgent = owner.GetComponent<NavMeshAgent>()
                       ?? owner.gameObject.AddComponent<NavMeshAgent>();
        // 플레이어 Transform 캐싱
        var sm = owner.GetComponent<ZombieStateMachine>();
        if (sm != null)
            playerTransform = sm.PlayerTransform;

        // Chase 상태용 설정
        navMeshAgent.autoRepath             = true;
        navMeshAgent.autoTraverseOffMeshLink = true;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    public override void EnterState()
    {
        base.EnterState();
        PlayAnimation("Walk");
        lastRepathTime = 0f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        MoveToPlayer();

        // 상태 전환 판단
        EState next = CheckStateTransitions();
        if (next != stateKey)
            stateTimer = float.MaxValue;
    }

    public override bool IsStateEnd(out EState nextState)
    {
        nextState = CheckStateTransitions();
        return nextState != stateKey;
    }

    private EState CheckStateTransitions()
    {
        if (!IsPlayerInRange(CHASE_RANGE))
            return EState.Wander;
        if (IsPlayerInRange(ATTACK_RANGE))
            return EState.Attack;
        return EState.Chase;
    }

    private void MoveToPlayer()
    {
        if (playerTransform == null || navMeshAgent == null)
            return;

        // 일정 간격 이상 경과했을 때만 재탐색
        if (Time.time - lastRepathTime < REPATH_INTERVAL)
            return;

        // 경로가 부분적이거나 오래된 경우 초기화
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial || navMeshAgent.isPathStale)
        {
            navMeshAgent.ResetPath();
        }

        // 플레이어 위치로 이동 명령
        navMeshAgent.SetDestination(playerTransform.position);
        lastRepathTime = Time.time;
    }
} 