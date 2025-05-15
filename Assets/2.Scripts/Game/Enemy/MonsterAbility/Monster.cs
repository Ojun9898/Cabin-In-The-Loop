using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [HideInInspector]
    public bool isMonsterSpawned;
    
    [Header("기본 정보")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    
    [Header("감지 범위")]
    public float chaseRange = 20f;
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
    
    private void OnEnable()
    {
        // 풀에서 꺼내 활성화될 때 true
        isMonsterSpawned = true;
    }
    
    private void OnDisable()
    {
        // 풀로 돌아가거나 비활성화될 때 false
        isMonsterSpawned = false;
    }
    
    private void InitializeComponents()
    {
        health = new MonsterHealth(maxHealth);
        
        // NavMeshAgent 컴포넌트 가져오기 또는 추가
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
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
    
    private void HandleHealthChanged(float newHealth)
    {
        // 체력 변경 시 처리
        Debug.Log($"[Monster] 체력 변경됨: {newHealth}");
    }
    
    public void HandleDeath()
    {
        // 사망 시 처리
        animator.Play("Death");
        // 경험치 지급
        if (player != null && player.TryGetComponent<PlayerStatus>(out var playerStatus))
        {
            Debug.Log("경험치 지급 됨");
            playerStatus.GainXp(20f);
        }
        StartCoroutine(waitForDeath());
        gameObject.SetActive(false);
       
    }

    IEnumerator waitForDeath()
    {
        yield return new WaitForSeconds(2f);
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
          
      }
      else
      {
          
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
    
    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }
} 