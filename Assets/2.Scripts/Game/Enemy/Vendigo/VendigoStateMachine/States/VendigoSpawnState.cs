using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoSpawnState : VendigoBaseState
{
    private const float SPAWN_DURATION = 1.5f;
    
    protected override void SetStateKey()
    {
        stateKey = EState.Spawn;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        StopMoving();
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
    }
    
    public override bool IsStateEnd(out EState nextState)
    {
        nextState = EState.Spawn;

        if (stateTimer >= SPAWN_DURATION)
        {
            nextState = EState.Idle;
            return true;
        }

        return false;
    }
}
