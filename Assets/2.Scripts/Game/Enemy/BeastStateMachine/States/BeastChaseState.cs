using UnityEngine;


public class BeastChaseState : BeastBaseState
{
    private const float ATTACK_RANGE = 1.5f;
    
    // 추격시에는 이동속도가 더 빨라짐
    private const float RUN_SPEED = 3.1f;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Chase;
    }
    
    public override void ExitState()
    {
        base.ExitState();
        if (navMeshAgent != null)
            defaultSpeed = navMeshAgent.speed;
    }

    public override void EnterState()
    {
        base.EnterState();
        // Run 상태 진입 시 속도 변경
        if (navMeshAgent != null)
            navMeshAgent.speed = RUN_SPEED;

        PlayAnimation("Chase");
        // Chase 사운드 재생 요청
        MonsterSFXManager.Instance.RequestPlay(
            EState.Chase,
            EMonsterType.Beast,
            beast.transform
        );
    }

    public override void UpdateState()
    {
        base.UpdateState();
        // Chase 사운드 반복 재생
        MonsterSFXManager.Instance.RequestPlay(
            EState.Chase,
            EMonsterType.Beast,
            beast.transform
        );
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

        // 하울링 사용 가능하고 플레이어가 하울링 범위 안에 있으면 Rattack 상태로 전환
        if (BeastRattackState.CanUseHowl() && IsPlayerInHowlRange())
        {
            return EState.Rattack;
        }

        return EState.Chase;
    }
} 