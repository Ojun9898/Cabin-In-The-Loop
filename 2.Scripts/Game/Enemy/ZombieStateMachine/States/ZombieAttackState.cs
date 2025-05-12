using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_RANGE = 2f;
    private const float CHASE_RANGE = 6f;
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

        // 공격 도중에는 상태를 그대로 유지
        if (stateTimer < ATTACK_DURATION)
        {
            // 공격이 중간에 처리되지 않았다면 데미지 적용 (모션 중간 시점)
            if (!hasAttacked && stateTimer >= ATTACK_DURATION * 0.5f)
            {
                // 실제 데미지 로직
                // 예: zombie.DealDamage();
                hasAttacked = true;
                Debug.Log("Attack executed: Damage inflicted to player!");
            }
            return;
        }

        // 공격이 끝난 이후 상태 전환 로직 실행
        HandlePostAttack();
    }
    
    // 공격 상태가 종료되었는지 확인
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Attack;

        // 공격 모션이 끝나지 않았으면 상태 종료 금지
        if (stateTimer < ATTACK_DURATION)
        {
            return false;
        }

        HandlePostAttack(out nextState);
        return true;
    }

    // 공격 후 상태 전환 처리
    private void HandlePostAttack(out EState nextState)
    {
        nextState = EState.Idle;

        if (IsPlayerInRange(ATTACK_RANGE))
        {
            nextState = EState.Attack;
        }
        else if (IsPlayerInRange(CHASE_RANGE))
        {
            nextState = EState.Chase;
        }
    }

    private void HandlePostAttack()
    {
        EState nextState;
        HandlePostAttack(out nextState);
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue;
        }
    }
} 