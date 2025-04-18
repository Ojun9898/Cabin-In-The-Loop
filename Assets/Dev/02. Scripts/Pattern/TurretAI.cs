using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public ITurretState currentState;
    
    private ITurretState patrolState;
    private ITurretState trackingState;
    private ITurretState attackState;

    public GameObject bulletPrefab;
    public ParticleSystem ps;
    public Transform shootTf;
    
    public List<Transform> targets = new List<Transform>();
    public Transform headTf;

    public Transform currentTarget;
    
    public float shootCooldown = 1f;
    
    void Start()
    {
        patrolState = this.gameObject.AddComponent<TurretPatrol>();
        trackingState = this.gameObject.AddComponent<TurretTracking>();
        attackState = this.gameObject.AddComponent<TurretAttack>();
        
        ChangeToPatrol();
    }

    void Update()
    {
        currentState?.Stay();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MONSTER"))
        {
            targets.Add(other.transform);

            SetTarget();
            
            ChangeToTracking();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MONSTER"))
        {
            SetTarget(other.transform);
        }
    }

    public void SetTarget(Transform prevTarget = null)
    {
        if (prevTarget != null)
            targets.Remove(prevTarget);

        if (targets.Count > 0)
            currentTarget = targets[0];
        else
            currentTarget = null;
    }

    public void ChangeToPatrol() => OnChageState(patrolState);
    public void ChangeToTracking() => OnChageState(trackingState);
    public void ChangeToAttack() => OnChageState(attackState);
    
    public void OnChageState(ITurretState newState)
    {
        currentState?.Exit();
        
        currentState = newState;
        
        currentState?.Enter();
    }
}