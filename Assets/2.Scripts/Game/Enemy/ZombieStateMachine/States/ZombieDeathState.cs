using UnityEngine;

public class ZombieDeathState : ZombieBaseState
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
            EMonsterType.Zombie,
            zombie.transform
        );
        PlayAnimation("Death");
        hasStartedDeathAnimation = false;
        
        // Monster의 HandleDeath 호출
        zombie.HandleDeath();
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        
        // 사망 애니메이션이 끝나면 오브젝트를 비활성화
        if (!hasStartedDeathAnimation && stateTimer >= DEATH_DURATION)
        {
            hasStartedDeathAnimation = true;
            zombie.gameObject.SetActive(false);
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Death;
        return false; // Death 상태는 종료되지 않음
    }
} 