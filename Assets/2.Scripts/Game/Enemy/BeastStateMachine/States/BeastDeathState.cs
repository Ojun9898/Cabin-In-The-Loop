using UnityEngine;

public class BeastDeathState : BeastBaseState
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
        PlayAnimation("Death");
        hasStartedDeathAnimation = false;
<<<<<<< HEAD
        
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        
        // 사망 애니메이션이 끝나면 오브젝트를 비활성화
        if (!hasStartedDeathAnimation && stateTimer >= DEATH_DURATION)
        {
            hasStartedDeathAnimation = true;
            beast.gameObject.SetActive(false);
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Death;
        return false; // Death 상태는 종료되지 않음
    }
} 