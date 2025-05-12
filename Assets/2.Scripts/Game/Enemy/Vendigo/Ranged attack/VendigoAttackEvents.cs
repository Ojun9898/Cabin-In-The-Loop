using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoAttackEvents : MonoBehaviour
{
    [Header("투사물 세팅")]
    public GameObject projectilePrefab;
    public Transform  muzzlePoint;
    private float      projectileSpeed = 11.5f;
    private float      upwardSpeed     = 2.75f;
    
    bool hasThrown = false;
    
    public void Throw()
    {
        // 애니메이션 중에 한번만 던지게 실행
        if (hasThrown) return;
        hasThrown = true;
        
        // VendigoStateMachine 컴포넌트에서 playerTransform 프로퍼티 꺼내기
        var pt = GetComponent<VendigoStateMachine>();
        Vector3 targetPos = pt.PlayerTransform.position;
        
        Projectile.Throw
        (
            projectilePrefab,
            muzzlePoint,
            targetPos,
            projectileSpeed,
            VendigoAttackState.DAMAGE_AMOUNT,
            upwardSpeed
        );
    }
    
    public void ResetThrow()
    {
        hasThrown = false;
    }
}
