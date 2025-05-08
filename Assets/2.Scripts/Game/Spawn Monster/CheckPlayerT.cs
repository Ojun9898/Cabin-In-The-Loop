using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpawnManager_PlayerTransformCheck
{
    public static class CheckPlayerT 
    {
        public static void AssignTransform(this GameObject go, Transform player)
        {
            if (go.TryGetComponent<Monster>(out var m)) 
                m.SetPlayer(player);
            
            if (go.TryGetComponent<ZombieStateMachine>(out var a)) 
                a.SetPlayerTransform(player);
            
            if (go.TryGetComponent<InsectStateMachine>(out var b)) 
                b.SetPlayerTransform(player);

            if (go.TryGetComponent<VendigoStateMachine>(out var c)) 
                c.SetPlayerTransform(player);

            if (go.TryGetComponent<RipperStateMachine>(out var d)) 
                d.SetPlayerTransform(player);
            
            if (go.TryGetComponent<BeastStateMachine>(out var e)) 
                e.SetPlayerTransform(player);
        }
    }

}
