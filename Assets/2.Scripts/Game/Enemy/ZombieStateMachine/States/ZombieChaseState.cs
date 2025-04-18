using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    private const float CHASE_RANGE = 8f;
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
        
        // 플레이어가 추적 범위를 벗어나면 Idle 상태로 전환
        if (!IsPlayerInRange(CHASE_RANGE))
        {
            stateTimer = float.MaxValue; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
            return;
        }
        
        // 플레이어가 공격 범위 안에 들어오면 공격 상태로 전환
        if (IsPlayerInRange(ATTACK_RANGE))
        {
            stateTimer = float.MaxValue; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
            return;
        }
        
        // 플레이어를 향해 이동
        MoveToPlayer();
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Chase;
        
        if (!IsPlayerInRange(CHASE_RANGE))
        {
            nextState = EState.Idle;
            return true;
        }
        
        if (IsPlayerInRange(ATTACK_RANGE))
        {
            nextState = EState.Attack;
            return true;
        }
        
        return false;
    }
} 