using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoHitState : VendigoBaseState
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
        PlayAnimation("Vendigo Damage");
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
            if (vendigo.IsDead())
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
