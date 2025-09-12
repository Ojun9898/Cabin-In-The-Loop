using UnityEngine;
using System.Collections.Generic;

public class BeastStateMachine : StateMachine<Monster>
{
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField] private List<BeastBaseState> beastStates;
=======
    private List<BeastBaseState> beastStates;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
    [SerializeField] private List<BeastBaseState> beastStates;
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    [SerializeField] private Transform playerTransform;
    
    // 하울링 관련 필드
    [SerializeField] private Howling howling;
    [SerializeField] private float howlRange = 8f;
    [SerializeField] private int maxHealth = 100;
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField] private ParticleSystem howlParticle; // 하울링 파티클 시스템
    private int currentHealth;
=======
    private int currentHealth;
    private float howlTimer = 0f;
    private bool hasUsedEmergencyHowl = false;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
    [SerializeField] private ParticleSystem howlParticle; // 하울링 파티클 시스템
    private int currentHealth;
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    private bool isHowling = false;
    
    public Transform PlayerTransform => playerTransform;
    
    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
    }
    
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    protected override void Initialize()
    {
        // Monster 컴포넌트 가져오기
        Monster monster = GetComponent<Monster>();
        if (monster == null)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            
=======
            Debug.LogWarning("Monster component not found on BeastStateMachine. Adding it automatically.");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
            Debug.LogWarning("Monster component not found on BeastStateMachine. Adding it automatically.");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
            monster = gameObject.AddComponent<Monster>();
        }
        
        // 플레이어 참조 설정
        if (playerTransform != null)
        {
            monster.SetPlayer(playerTransform);
        }
        else
        {
<<<<<<< HEAD
<<<<<<< HEAD
            
=======
            Debug.LogWarning("Player Transform not set in BeastStateMachine. Please assign it in the inspector.");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
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
<<<<<<< HEAD
=======
            Debug.LogWarning("Player Transform not set in BeastStateMachine. Please assign it in the inspector.");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
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
<<<<<<< HEAD
<<<<<<< HEAD
=======
        howlTimer = 0f;
        hasUsedEmergencyHowl = false;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        isHowling = false;

        base.Initialize();
    }
<<<<<<< HEAD
<<<<<<< HEAD
=======

    private void Update()
    {
        // 하울링 중이 아닐 때만 쿨다운 타이머 증가
        if (!isHowling)
        {
            howlTimer += Time.deltaTime;
        }
    }
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    
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

<<<<<<< HEAD
<<<<<<< HEAD
=======
    public bool HasUsedEmergencyHowl()
    {
        return hasUsedEmergencyHowl;
    }

    public void SetUsedEmergencyHowl(bool used)
    {
        hasUsedEmergencyHowl = used;
    }

>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    public bool IsDead()
    {
        return currentHealth <= 0;
    }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    public void StartHowling()
    {
        isHowling = true;
        if (howlParticle != null)
        {
            howlParticle.gameObject.SetActive(true);
            howlParticle.Play();
<<<<<<< HEAD
=======
    public bool CanUseHowl()
    {
        const float HOWL_COOLDOWN = 15f;
        return (!hasUsedEmergencyHowl && GetHealthPercentage() <= 0.5f) || howlTimer >= HOWL_COOLDOWN;
    }

    public void StartHowling()
    {
        isHowling = true;
        if (GetHealthPercentage() <= 0.5f)
        {
            hasUsedEmergencyHowl = true;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        }
    }

    public void EndHowling()
    {
        isHowling = false;
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
        if (howlParticle != null)
        {
            howlParticle.Stop();
            howlParticle.gameObject.SetActive(false);
        }
<<<<<<< HEAD
=======
        howlTimer = 0f;
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    }
    
    // 디버깅을 위한 현재 상태 로그 출력
    public override void ChangeState(EState nextState)
    {
        EState previousState = currentState?.StateKey ?? EState.Idle;
        base.ChangeState(nextState);
<<<<<<< HEAD
<<<<<<< HEAD
        
=======
        Debug.Log($"Beast state changed: {previousState} -> {nextState}");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
        Debug.Log($"Beast state changed: {previousState} -> {nextState}");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    }
} 