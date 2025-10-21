using UnityEngine;

public class InsectChaseState : InsectBaseState
{
    private const float CHASE_RANGE = 48f;
    private const float ATTACK_RANGE = 1.5f;
    
    // 추격시에는 이동속도가 더 빨라짐
    private float runSpeed = 2.2f; // ★ 변경: const → 필드(기본값 동일)
    public float BaseRunSpeed => runSpeed; 
    
    protected override void SetStateKey()
    {
        stateKey = EState.Chase;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        // Run 상태 진입 시 속도 변경
        if (navMeshAgent != null) 
            navMeshAgent.speed = runSpeed;
        PlayAnimation("Walk");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        bool inRange = IsPlayerInRange(CHASE_RANGE);
        // 범위 안일 때만 직접 회전, 아닐 때는 NavMesh 회전
        if (navMeshAgent != null)
            navMeshAgent.updateRotation = !inRange;
        if (inRange)
            RotateTowards(playerTransform.position, 6f);
        
        MoveToPlayer();

        // 상태 전환 처리
        EState nextState = CheckStateTransitions();
        if (nextState != stateKey)
        {
            stateTimer = float.MaxValue;
        }
    }

    public override bool IsStateEnd(out EState nextState)
    {
        nextState = CheckStateTransitions();
        return nextState != stateKey;
    }
    
    private EState CheckStateTransitions()
    {
        if (!IsPlayerInChaseRange()) 
        {
            return EState.Wander;
        }

        if (IsPlayerInAttackRange())
        {
            return EState.Attack;
        }

        return EState.Chase;
    }
} 