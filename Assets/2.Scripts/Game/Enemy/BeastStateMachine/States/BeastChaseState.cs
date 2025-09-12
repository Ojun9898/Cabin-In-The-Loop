using UnityEngine;

<<<<<<< HEAD
<<<<<<< HEAD

public class BeastChaseState : BeastBaseState
{
    private const float CHASE_RANGE = 18f;
    private const float ATTACK_RANGE = 1.5f;
    
    // 추격시에는 이동속도가 더 빨라짐
    private const float RUN_SPEED = 3.1f;
    
=======
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
public class BeastChaseState : BeastBaseState
{
    private const float CHASE_RANGE = 6f;
    private const float ATTACK_RANGE = 1.5f;
    
<<<<<<< HEAD
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    protected override void SetStateKey()
    {
        stateKey = EState.Chase;
    }
    
<<<<<<< HEAD
<<<<<<< HEAD
    public override void ExitState()
    {
        base.ExitState();
        if (navMeshAgent != null)
            defaultSpeed = navMeshAgent.speed;
    }

    public override void EnterState()
    {
        base.EnterState();
        // Run 상태 진입 시 속도 변경
        if (navMeshAgent != null)
            navMeshAgent.speed = RUN_SPEED;

        PlayAnimation("Chase");
=======
    public override void EnterState()
    {
        base.EnterState();
        PlayAnimation("Walk");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
    public override void EnterState()
    {
        base.EnterState();
        PlayAnimation("Chase");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    }

    public override void UpdateState()
    {
        base.UpdateState();
<<<<<<< HEAD
<<<<<<< HEAD
        MoveToPlayer();
        
=======
        
        MoveToPlayer();

>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
        
        MoveToPlayer();

>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        // 상태 전환 처리
        EState nextState = CheckStateTransitions();
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue;
        }
    }

    public override bool IsStateEnd(out EState nextState)
    {
        nextState = CheckStateTransitions();
        return nextState != stateKey;
    }
    
    private EState CheckStateTransitions()
    {
        if (!IsPlayerInChaseRange()) 
        {
            return EState.Wander;
        }

        if (IsPlayerInAttackRange())
        {
            return EState.Attack;
        }

<<<<<<< HEAD
<<<<<<< HEAD
        // 하울링 사용 가능하고 플레이어가 하울링 범위 안에 있으면 Rattack 상태로 전환
        if (BeastRattackState.CanUseHowl() && IsPlayerInHowlRange())
=======
        if (IsPlayerInHowlRange())
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
        // 하울링 사용 가능하고 플레이어가 하울링 범위 안에 있으면 Rattack 상태로 전환
        if (BeastRattackState.CanUseHowl() && IsPlayerInHowlRange())
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        {
            return EState.Rattack;
        }

        return EState.Chase;
    }
} 