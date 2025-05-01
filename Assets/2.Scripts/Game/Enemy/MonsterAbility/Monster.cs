using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [Header("기본 정보")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    
    [Header("감지 범위")]
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    
    [Header("참조")]
    public Transform player;
    
    private MonsterHealth health;
    private MonsterMovement movement;
    private MonsterCombat combat;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    private static readonly int AttackIndexParam = Animator.StringToHash("AttackIndex");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    
    private void Awake()
    {
        InitializeComponents();
        SubscribeToEvents();
    }
    
    private void InitializeComponents()
    {
        health = new MonsterHealth(maxHealth);
        
        // NavMeshAgent 컴포넌트 가져오기 또는 추가
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogWarning("NavMeshAgent component not found on Monster. Adding it automatically.");
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            
            // 기본 설정
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.angularSpeed = 120f;
            navMeshAgent.stoppingDistance = 0.1f;
        }
        
        movement = new MonsterMovement(navMeshAgent, moveSpeed);
        combat = new MonsterCombat(attackDamage, attackRange);
        animator = GetComponent<Animator>();
        
        // 플레이어 참조 확인
        if (player == null)
        {
            Debug.LogWarning("Player reference not set. Please assign a player in the inspector.");
        }
    }
    
    // 플레이어 참조 설정 메서드
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
    
    private void SubscribeToEvents()
    {
        health.OnHealthChanged += HandleHealthChanged;
        health.OnDeath += HandleDeath;
    }
    
    private void HandleHealthChanged(int newHealth)
    {
        // 체력 변경 시 처리
    }
    
    private void HandleDeath()
    {
        // 사망 시 처리
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
        if (player == null) return;

      NavMeshHit navHit;
      if (NavMesh.SamplePosition(player.position, out navHit, 1.0f, NavMesh.AllAreas))
      {
          movement.MoveToTarget(navHit.position);
          Debug.Log($"Moving to sampled position near player: {navHit.position}");
      }
      else
      {
          Debug.LogWarning("Player position is not on NavMesh. Cannot move.");
      }
    }
    
    public void SetAttackAnimation(int index)
    {
        if (animator != null)
        {
            animator.SetInteger(AttackIndexParam, index);
            animator.SetTrigger(AttackTrigger);
        }
        else
        {
            Debug.LogWarning("Animator component not found on Monster.");
        }
    }
    
    public void StopMoving()
    {
        movement.Stop();
    }
    
    public void PlayAnimation(string animationName)
    {
        animator?.CrossFade(animationName, 0.1f, 0);
    }
    
    public bool IsDead()
    {
        return health.CurrentHealth <= 0;
    }
    
    public void OnHit(int damage)
    {
        health.TakeDamage(damage);
    }
} 