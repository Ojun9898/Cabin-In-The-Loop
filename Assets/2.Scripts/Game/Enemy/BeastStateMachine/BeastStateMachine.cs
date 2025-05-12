using UnityEngine;
using System.Collections.Generic;

public class BeastStateMachine : StateMachine<Monster>
{
    private List<BeastBaseState> beastStates;
    [SerializeField] private Transform playerTransform;
    
    // 하울링 관련 필드
    [SerializeField] private Howling howling;
    [SerializeField] private float howlRange = 8f;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private float howlTimer = 0f;
    private bool hasUsedEmergencyHowl = false;
    private bool isHowling = false;
    
    public Transform PlayerTransform => playerTransform;
    
    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
    }
    
    protected override void Initialize()
    {
        // Monster 컴포넌트 가져오기
        Monster monster = GetComponent<Monster>();
        if (monster == null)
        {
            Debug.LogWarning("Monster component not found on BeastStateMachine. Adding it automatically.");
            monster = gameObject.AddComponent<Monster>();
        }
        
        // 플레이어 참조 설정
        if (playerTransform != null)
        {
            monster.SetPlayer(playerTransform);
        }
        else
        {
            Debug.LogWarning("Player Transform not set in BeastStateMachine. Please assign it in the inspector.");
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
        howlTimer = 0f;
        hasUsedEmergencyHowl = false;
        isHowling = false;

        base.Initialize();
    }

    private void Update()
    {
        // 하울링 중이 아닐 때만 쿨다운 타이머 증가
        if (!isHowling)
        {
            howlTimer += Time.deltaTime;
        }
    }
    
    public void OnHit(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Monster monster = GetComponent<Monster>();
        monster.OnHit(damage);
        
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

    public bool HasUsedEmergencyHowl()
    {
        return hasUsedEmergencyHowl;
    }

    public void SetUsedEmergencyHowl(bool used)
    {
        hasUsedEmergencyHowl = used;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

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
        }
    }

    public void EndHowling()
    {
        isHowling = false;
        howlTimer = 0f;
    }
    
    // 디버깅을 위한 현재 상태 로그 출력
    public override void ChangeState(EState nextState)
    {
        EState previousState = currentState?.StateKey ?? EState.Idle;
        base.ChangeState(nextState);
        Debug.Log($"Beast state changed: {previousState} -> {nextState}");
    }
} 