using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("기본 정보")]
    public int maxHealth = 100;
    public int health;
    public float moveSpeed = 1.5f;
    public float attackDamage = 10f;
    
    [Header("감지 범위")]
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    
    [Header("참조")]
    public Transform player;
    
    [HideInInspector] public Animator animator;
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    
    protected virtual void Awake()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }
    
    public virtual void OnHit(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }
    
    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
    }
    
    public bool IsPlayerInAttackRange()
    {
        return IsPlayerInRange(attackRange);
    }
    
    public bool IsPlayerInChaseRange()
    {
        return IsPlayerInRange(chaseRange);
    }
    
    public void MoveToPlayer()
    {
        if (agent != null && player != null)
        {
            agent.SetDestination(player.position);
        }
    }
    
    public void StopMoving()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
    
    public void ResumeMoving()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    
    public void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.CrossFade(animationName, 0.1f, 0);
        }
    }
    
    public bool IsDead()
    {
        return health <= 0;
    }
} 