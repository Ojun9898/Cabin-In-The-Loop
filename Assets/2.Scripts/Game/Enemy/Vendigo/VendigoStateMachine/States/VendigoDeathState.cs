using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoDeathState : VendigoBaseState
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
        MonsterSFXManager.Instance.RequestPlay(
            EState.Death,
            EMonsterType.Vendigo,
            vendigo.transform
        );
        PlayAnimation("Vendigo Death");
        hasStartedDeathAnimation = false;
        
        vendigo.HandleDeath();
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
 
        // 사망 애니메이션이 끝나면 오브젝트를 비활성화
        if (!hasStartedDeathAnimation && stateTimer >= DEATH_DURATION)
        {
            hasStartedDeathAnimation = true;
            vendigo.gameObject.SetActive(false);
        }
    }
 
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Death;
        return false; // Death 상태는 종료되지 않음
    }
}
