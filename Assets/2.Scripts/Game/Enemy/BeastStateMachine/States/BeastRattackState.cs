using UnityEngine;

public class BeastRattackState : BeastBaseState
{
    private const float HOWL_DURATION = 2f;
    private const float PARTICLE_START_TIME = 0.5f; // 애니메이션 시작 후 파티클이 나오는 시간
    private bool hasStartedParticle = false;

    protected override void SetStateKey()
    {
        stateKey = EState.Rattack;
    }

    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
        PlayAnimation("Shout");
        hasStartedParticle = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // 애니메이션 중간에 파티클 시작
        if (!hasStartedParticle && stateTimer >= PARTICLE_START_TIME)
        {
            StartHowling();
            hasStartedParticle = true;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        EndHowling();
    }

    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Rattack;

        if (stateTimer >= HOWL_DURATION)
        {
            // 다음 상태 결정
            if (IsPlayerInAttackRange())
            {
                nextState = EState.Attack;
            }
            else if (IsPlayerInChaseRange())
            {
                nextState = EState.Chase;
            }
            else
            {
                nextState = EState.Wander;
            }
            return true;
        }

        return false;
    }
}