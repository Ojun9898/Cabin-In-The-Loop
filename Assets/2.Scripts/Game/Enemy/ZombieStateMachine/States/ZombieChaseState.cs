using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    private const float CHASE_RANGE = 6f;
    private const float ATTACK_RANGE = 1.5f;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Chase;
    }
    
    public override void ExitState()
    {
        base.ExitState();
        // Chase 상태 벗어나면 즉시 해당 몬스터 모든 SFX 중단
        MonsterSFXManager.Instance.StopAllAudio(
            zombie.transform.GetInstanceID()
        );
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
        if (!IsPlayerInRange(CHASE_RANGE))
        {
            return EState.Wander;
        }

        if (IsPlayerInRange(ATTACK_RANGE))
        {
            return EState.Attack;
        }

        return EState.Chase;
    }
} 