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

            if (go.TryGetComponent<VendigoStateMachine>(out var v)) 
                v.SetPlayerTransform(player);

            if (go.TryGetComponent<RipperStateMachine>(out var r)) 
                r.SetPlayerTransform(player);
        }
    }

}
