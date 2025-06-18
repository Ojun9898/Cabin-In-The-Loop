using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour, IDamageable
{
    [HideInInspector]
    public bool isMonsterSpawned;
    
    [Header("기본 정보")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private PlayerStatus playerStatus;
    
    [Header("감지 범위")]
    public float chaseRange = 20f;
    public float attackRange = 1.5f;
    
    [Header("참조")]
    public Transform player;
    
    private MonsterHealth health;
    private int defaultMaxHealth;  // 원본 maxHealth 보관용
    private MonsterMovement movement;
    private MonsterCombat combat;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private StateMachine<Monster> stateMachine;
    
    private bool isDead = false;    // 사망 여부 플래그
    private bool xpGiven = false;   // 경험치 지급 여부 플래그
    
    
    private static readonly int AttackIndexParam = Animator.StringToHash("AttackIndex");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    
    private bool canTakeDamage = true;
    private const float HIT_CoolDown = 1.25f;
    
    private void Awake()
    {
        defaultMaxHealth = maxHealth;
        InitializeHealth(maxHealth);

        stateMachine = GetComponent<StateMachine<Monster>>();
        InitializeComponents();
        SubscribeToEvents();

        if (playerStatus == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerStatus = playerObj.GetComponent<PlayerStatus>();

            if (playerStatus == null)
                Debug.LogError("[Monster] Awake(): PlayerStatus를 찾을 수 없습니다.");
        }
    }
    
    private void OnEnable()
    {
        isDead   = false;
        xpGiven  = false;
        // 풀에서 꺼내 활성화될 때 true
        isMonsterSpawned = true;
        canTakeDamage = true;
    }
    
    private void OnDisable()
    {
        // 풀로 돌아가거나 비활성화될 때 false
        isMonsterSpawned = false;
        canTakeDamage = true;
    }
    
    // health를 새로 생성하는 공통 메서드
    private void InitializeHealth(int hp)
    {
        health = new MonsterHealth(hp);
        health.OnHealthChanged += HandleHealthChanged;
        health.OnDeath         += HandleDeath;
    }
    
    public void ResetHealth(int? overrideMax = null)
    {
        int hp = overrideMax ?? defaultMaxHealth;
        maxHealth = hp;              // Inspector 상에도 반영
        InitializeHealth(hp);        // MonsterHealth 재생성
    }

    public void ResetState(Vector3 spawnPos, Transform playerTransform, int? overrideMaxHealth = null)
    {
        // 1) 기본 플래그
        isMonsterSpawned = true;
        isDead  = false;
        xpGiven = false;
        
        // 2) 플레이어 참조
        player = playerTransform;
        
        // 3) 체력 초기화
        ResetHealth(overrideMaxHealth ?? defaultMaxHealth);
        
        // 4) NavMeshAgent
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled   = true;
            navMeshAgent.Warp(spawnPos);
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.isStopped = false;
            }
        }
        
        // 5) Collider
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
        
        // 6) Animator
        if (animator != null)
        {
            animator.Rebind();  
        }
        
        // 7) Movement·Combat (선택적 재생성)
        movement = new MonsterMovement(navMeshAgent, moveSpeed);
        combat   = new MonsterCombat(attackDamage, attackRange);
        
        // 8) StateMachine
        stateMachine.ResetStateMachine();
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
        // 이미 사망 처리했으면 무시
        if (isDead) return;
        isDead = true;
        
        // 한 번만 xp 지급
        AwardXp();
        
        // 이동·충돌 완전 중지
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.isStopped = true;   // 이제 정상 대입 가능
        }

        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;             // 정상 대입
        }
        
        // 체력 0 시 바로 Death 상태로 전환
        stateMachine?.ChangeState(EState.Death);
    }
    
    
    // 몬스터 사망지 경험치를 한번만 지급
    public void AwardXp()
    {
        Debug.Log($"[AwardXp] playerStatus={playerStatus}, xpGiven(before)={xpGiven}");
        if (playerStatus == null)
        {
            Debug.LogError("[AwardXp] playerStatus가 할당되지 않았습니다!");
            return;
        }
        if (!xpGiven)
        {
            xpGiven = true;
            playerStatus.GainXp(20f);
            Debug.Log("[AwardXp] XP granted!");
        }
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
        if (isDead) return;
        if (player == null) return;

      NavMeshHit navHit;
      if (NavMesh.SamplePosition(player.position, out navHit, 1.0f, NavMesh.AllAreas))
      {
          movement.MoveToTarget(navHit.position);
      }
    }
    
    public void SetAttackAnimation(int index)
    {
        if (isDead) return;
        if (animator != null)
        {
            animator.SetInteger(AttackIndexParam, index);
            animator.SetTrigger(AttackTrigger);
        }
    }
    
    public void StopMoving()
    {
        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
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
        if (isDead) return;
        // 무적 중이면 damage를 0으로 만들고 로그만 남김
        if (!canTakeDamage)
        {
            Debug.Log("[Monster] 무적상태: 데미지 0 적용");
            health.TakeDamage(0f);
            return;
        }
        // 평상시 피격 처리
        health.TakeDamage(damage);
        stateMachine.ChangeState(EState.Hit);
        StartCoroutine(DamageCooldown());
    }
    
    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        // 무적시간 1초
        yield return new WaitForSeconds(HIT_CoolDown);
        canTakeDamage = true;
    }
} 