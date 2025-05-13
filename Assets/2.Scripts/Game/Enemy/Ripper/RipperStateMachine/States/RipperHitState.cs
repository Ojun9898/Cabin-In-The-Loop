using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipperHitState : RipperBaseState
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
        // Hit(피격) 사운드 재생 요청
        MonsterSFXManager.Instance.RequestPlay(
            EState.Hit,
            EMonsterType.Ripper,
            ripper.transform
        );
        PlayAnimation("Ripper Damage");
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
            if (ripper.IsDead())
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
