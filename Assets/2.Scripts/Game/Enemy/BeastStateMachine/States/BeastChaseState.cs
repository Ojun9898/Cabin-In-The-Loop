using UnityEngine;

public class BeastChaseState : BeastBaseState
{
    private const float CHASE_RANGE = 18f;
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

        if (IsPlayerInHowlRange())
        {
            return EState.Rattack;
        }

        return EState.Chase;
    }
} 