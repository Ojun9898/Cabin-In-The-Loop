using UnityEngine;

public class InsectChaseState : InsectBaseState
{
<<<<<<< HEAD
    private const float CHASE_RANGE = 20f;
=======
    private const float CHASE_RANGE = 6f;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
    private const float ATTACK_RANGE = 1.5f;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Chase;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        PlayAnimation("Walk");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        bool inRange = IsPlayerInRange(CHASE_RANGE);
        // 범위 안일 때만 직접 회전, 아닐 때는 NavMesh 회전
        if (navMeshAgent != null)
            navMeshAgent.updateRotation = !inRange;
        if (inRange)
            RotateTowards(playerTransform.position, 6f);
        
        MoveToPlayer();

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

        return EState.Chase;
    }
} 