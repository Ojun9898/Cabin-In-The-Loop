using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoAttackState : VendigoBaseState
{
    private const float ATTACK_DURATION = 1.5f;
    private const float ATTACK_RANGE = 4.5f;
    private const float CHASE_RANGE = 6f;
    private const float DAMAGE_FIELD_RADIUS = 2f;
    public const float DAMAGE_AMOUNT = 25f;  
    private bool hasAttacked = false;
    private VendigoAttackEvents attackEvents;
    
    
    
    protected override void SetStateKey()
    {
        stateKey = EState.Attack;
    }

    public override void EnterState()
    {
        base.EnterState();
        attackEvents = vendigo.GetComponentInChildren<VendigoAttackEvents>();
        attackEvents?.ResetThrow();
        StopMoving();
        PlayAnimation("Vendigo Attack");
        hasAttacked = false;
        
    }
    
    
    public override void UpdateState()
    {
        base.UpdateState();

        // 공격 도중에는 상태를 그대로 유지
        if (stateTimer < ATTACK_DURATION)
        {
            return;
        }

        // 공격이 끝난 이후 상태 전환 로직 실행
        HandlePostAttack();
    }
    
    private void CreateDamageField()
    {
        GameObject damageField = GameManager.Instance.GetDamageField();
        if (damageField != null)
        {
            damageField.transform.position = vendigo.transform.position;
            damageField.GetComponent<DamageField>().Initialize(
                vendigo.gameObject, 
                DAMAGE_AMOUNT, 
                DAMAGE_FIELD_RADIUS, 
                0.1f // 데미지 필드 지속 시간
            );
        }
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
