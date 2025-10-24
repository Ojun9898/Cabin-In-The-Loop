using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class BeastStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<BeastBaseState> beastStates;
    [SerializeField] private Transform playerTransform;
    
    // 하울링 관련 필드
    [SerializeField] private Howling howling;
    private float howlRange = 180f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private ParticleSystem howlParticle; // 하울링 파티클 시스템
    [SerializeField] private BeastHowlEffect howlEffect; // 하울링시 사용할 특수효과
    
    // 하울링의 효과를 받을 범위설정
    [SerializeField] private float howlEffectRadius = 8f;  // 둥근 원(구) 반경
    [SerializeField] private LayerMask enemyMask;       // 범위를 그리는 레이어
  
    private int currentHealth;
    private bool isHowling = false;
    
    public Transform PlayerTransform => playerTransform;
    
    // 하울링시 적용할 데미지 증가코드
    public float CurrentDamageMultiplier =>
        (howlEffect != null) ? howlEffect.CurrentDamageMultiplier : 1f;
    
    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
    }
    
    private void Start()
    {
        // 플레이어 바인딩을 Start에서 처리
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("[BeastStateMachine] Start(): Player를 찾을 수 없습니다.");
                return;
            }
        }

        // 초기화 호출
        Initialize();
    }
    protected override void Initialize()
    {
        // Monster 컴포넌트 가져오기
        Monster monster = GetComponent<Monster>();
        if (monster == null)
        {
            monster = gameObject.AddComponent<Monster>();
        }
        
        // 플레이어 참조 설정
        if (playerTransform != null)
        {
            monster.SetPlayer(playerTransform);
        }
        
        // 파티클 시스템 가져오기
        if (howlParticle == null)
        {
            howlParticle = GetComponentInChildren<ParticleSystem>();
            if (howlParticle != null)
            {
                // 파티클이 루프하지 않도록 설정
                var main = howlParticle.main;
                main.loop = false;
            }
        }
        
        // 상태 목록이 비어있으면 기본 상태들을 생성
        if (beastStates == null || beastStates.Count == 0)
        {
            beastStates = new List<BeastBaseState>
            {
                new BeastSpawnState(),
                new BeastIdleState(),
                new BeastPatorlState(),
                new BeastChaseState(),
                new BeastAttackState(),
                new BeastHitState(),
                new BeastDeathState(),
                new BeastRattackState()
            };
        }
        
        // 각 상태를 초기화
        foreach (var state in beastStates)
        {
            state.Initialize(monster);
        }
        
        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(beastStates);

        // 초기화
        currentHealth = maxHealth;
        isHowling = false;

        base.Initialize();
    }
    
    // 하울링을 하기 위한 헬퍼
    // Initialize()에서 beastStates에 상태들을 생성/등록하므로 여기서 찾을 수 있음
    public TState GetState<TState>() where TState : BeastBaseState
    {
        // states 에는 (Spawn/Idle/Patrol/Chase/Attack/Hit/Death/Rattack) 상태를 담고 있음
        foreach (var s in states)              
        { 
            // 만약, machine.GetState<BeastRattackState>()가 호출되면
            // BeastRattackState 을 t로 캐스팅해서 리턴
            if (s is TState t) return t;
        }
        return null;
    }
    
    public void OnHit(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Monster monster = GetComponent<Monster>();
        monster.TakeDamage(damage);
        
        if (!IsDead())
        {
            ChangeState(EState.Hit);
        }
        else
        {
            ChangeState(EState.Death);
        }
    }
    
    

    // 하울링 관련 메서드
    public float GetHowlRange()
    {
        return howlRange;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void StartHowling()
    {
        isHowling = true;
        if (howlParticle != null)
        {
            howlParticle.gameObject.SetActive(true);
            howlParticle.Play();
        }
        
        // 버스트 트리거 (이후 5초 유지 + 페이드아웃까지 독립 동작)
        howlEffect?.TriggerHowlBurst();
        
        // 하울링 효과 10초
        ApplyHowlingEffect(10f);
    }
    
    void ApplyHowlingEffect(float durationSeconds)
    {
        if (durationSeconds <= 0f) return;
        // 3D: 구(OverlapSphere)
        // enemyMask : LayerMask 타입으로 적용할 레이어 설정
        // QueryTriggerInteraction.Collide : Trigger 콜라이더도 탐색에 포함
        var sphere = Physics.OverlapSphere(transform.position, howlEffectRadius, enemyMask, QueryTriggerInteraction.Ignore);
        // 자기 자신 및 null 방어
        foreach (var s in sphere)
        {
            // null 이거나, 비스트 자신의 콜라이더를 잡았으면 제외(자기 버프를 중복으로 걸지 않으려는 의도)
            if (!s || s.gameObject == gameObject) continue;

            // ??(널 병합 연산자) 기준으로 왼쪽이 null 이면 오른쪽 사용, 아니면 왼쪽 사용
            var effect = s.GetComponent<B_ApplyEffet>() ?? s.GetComponentInParent<B_ApplyEffet>();
            if (effect != null)
                effect.ActivateFor(durationSeconds);
        }
    }

    // 에디터에서 반경을 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, howlEffectRadius);
    }

    public void EndHowling()
    {
        isHowling = false;
        if (howlParticle != null)
        {
            howlParticle.Stop();
            howlParticle.gameObject.SetActive(false);
        }
        
    }
    
    // 디버깅을 위한 현재 상태 로그 출력
    public override void ChangeState(EState nextState)
    {
        EState previousState = currentState?.StateKey ?? EState.Idle;
        base.ChangeState(nextState);
        
    }
} 