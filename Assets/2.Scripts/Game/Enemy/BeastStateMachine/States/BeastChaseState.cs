using UnityEngine;

public class BeastChaseState : BeastBaseState
{
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
        SetPlayerPosition();
        
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

        // 인스턴스 상태를 꺼내서 CanUseHowl() 호출
        var machine = beast != null ? beast.GetComponent<BeastStateMachine>() : null;   
        var rattackState = machine != null ? machine.GetState<BeastRattackState>() : null;

        if (rattackState != null && rattackState.CanUseHowl() && IsPlayerInHowlRange())
            return EState.Rattack;
        
        return EState.Chase;
    }
} 