using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipperDeathState : RipperBaseState
{
    private const float DEATH_DURATION = 2f;
    private bool hasStartedDeathAnimation = false;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Death;
    }
    
    

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        // Death(사망) 사운드 재생 요청
        MonsterSFXManager.Instance.RequestPlay(
            EState.Death,
            EMonsterType.Ripper,
            ripper.transform
        );
        PlayAnimation("Ripper Death");
        hasStartedDeathAnimation = false;
        
        ripper.HandleDeath();
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
     
        // 사망 애니메이션이 끝나면 오브젝트를 비활성화
        if (!hasStartedDeathAnimation && stateTimer >= DEATH_DURATION)
        {
            hasStartedDeathAnimation = true;
            ripper.gameObject.SetActive(false);
        }
    }
 
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Death;
        return false; // Death 상태는 종료되지 않음
    }
}
