using UnityEngine;

public class ZombieHitState : ZombieBaseState
{
    private const float HIT_DURATION = 0.5f;
    private int damageAmount = 10; // 데미지 양 설정
    
    protected override void SetStateKey()
    {
        stateKey = EState.Hit;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Walk Back");
        TakeDamage(damageAmount); // 데미지 처리
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Hit;
        
        if (stateTimer >= HIT_DURATION)
        {
            if (zombie.IsDead())
            {
                nextState = EState.Death;
            }
            else
            {
                nextState = EState.Chase;
            }
            return true;
        }
        
        return false;
    }
} 