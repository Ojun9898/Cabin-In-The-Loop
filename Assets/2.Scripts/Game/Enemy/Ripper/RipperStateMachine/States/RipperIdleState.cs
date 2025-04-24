using UnityEngine;

public class RipperIdleState : RipperBaseState
{
    private const float IDLE_DURATION = 3f;
    private const float CHASE_RANGE = 8f;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Idle;
    }

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Ripper Idle");
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
    
        // 플레이어가 감지 범위 안에 들어오면 추적 상태로 전환
        if (IsPlayerInRange(CHASE_RANGE))
        {
            stateTimer = IDLE_DURATION; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Idle;
    
        if (stateTimer >= IDLE_DURATION)
        {
            if (IsPlayerInRange(CHASE_RANGE))
            {
                nextState = EState.Chase;
                return true;
            }
            else
            {
                nextState = EState.Wander;
                return true;
            }
        }
    
        return false;
    }
}
