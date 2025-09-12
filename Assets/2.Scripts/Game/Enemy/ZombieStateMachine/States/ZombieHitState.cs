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
<<<<<<< HEAD
<<<<<<< HEAD
        PlayAnimation("Damage");
=======
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        // Hit(피격) 사운드 재생 요청
        MonsterSFXManager.Instance.RequestPlay(
            EState.Hit,
            EMonsterType.Zombie,
            zombie.transform
        );
        PlayAnimation("Walk Back");
        TakeDamage(damageAmount); // 데미지 처리
<<<<<<< HEAD
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
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