using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_RANGE = 1.5f;
    private bool hasAttacked = false;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Attack;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Attack");
        hasAttacked = false;
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        
        // 공격 범위를 벗어나면 Chase 상태로 전환
        if (!IsPlayerInRange(ATTACK_RANGE))
        {
            stateTimer = float.MaxValue; // 즉시 상태 전환을 위해 타이머를 최대값으로 설정
            return;
        }
        
        // 공격 애니메이션의 중간 지점에서 데미지를 적용
        if (!hasAttacked && stateTimer >= ATTACK_DURATION * 0.5f)
        {
            // 여기서 데미지를 적용하는 로직을 추가
            // 예: zombie.DealDamage();
            hasAttacked = true;
        }
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Attack;
        
        if (!IsPlayerInRange(ATTACK_RANGE))
        {
            nextState = EState.Chase;
            return true;
        }
        
        if (stateTimer >= ATTACK_DURATION)
        {
            nextState = EState.Chase;
            return true;
        }
        
        return false;
    }
} 