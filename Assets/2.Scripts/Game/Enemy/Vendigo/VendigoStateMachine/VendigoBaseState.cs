using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class VendigoBaseState : State<Monster>
{
    protected Monster vendigo;
    protected float stateTimer;

    protected NavMeshAgent navMeshAgent;
    protected float defaultSpeed;
    
    protected Transform playerTransform;

    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
    } 
    
    public override void Initialize(Monster owner)
    {
        vendigo = owner;
        stateTimer = 0f;
        SetStateKey();
    
        // NavMeshAgent 가져와서 자동 회전 끄기
        navMeshAgent = owner.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        { 
            defaultSpeed = navMeshAgent.speed;
            navMeshAgent.updateRotation = false;
        }
    }
    
    // 플레이어의 위치를 바라보며 부드럽게 Y축 회전
    protected void RotateTowards(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 dir = targetPosition - vendigo.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
    
        Quaternion want = Quaternion.LookRotation(dir);
        vendigo.transform.rotation = Quaternion.Slerp(vendigo.transform.rotation, want, rotationSpeed * Time.deltaTime);
    }
    
    // 하위 클래스에서 구현하여 stateKey를 설정
    protected abstract void SetStateKey();
    
    public override void EnterState()
    {
        stateTimer = 0f;
    }

    public override void ExitState()
    {
        // 기본 구현은 비어있음
    }
    
    public override void UpdateState()
    {
        stateTimer += Time.deltaTime;
    }

    public override void FixedUpdateState()
    {
        // 물리 기반 업데이트가 필요한 경우 하위 클래스에서 구현
    }
    
    protected bool IsPlayerInRange(float range)
    {
        return vendigo.IsPlayerInRange(range);
    }

    protected void MoveToPlayer()
    {
        vendigo.MoveToPlayer();
    }

    protected void StopMoving()
    {
        vendigo.StopMoving();
    }
    
    protected void PlayAnimation(string animationName)
    {
        vendigo.PlayAnimation(animationName);
    }

    protected void TakeDamage(int damage)
    {
        vendigo.TakeDamage(damage);
    }
    
}
