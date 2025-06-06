using UnityEngine;
using System.Collections.Generic;

public class BeastStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<BeastBaseState> beastStates;
    [SerializeField] private Transform playerTransform;
    
    // 하울링 관련 필드
    [SerializeField] private Howling howling;
    [SerializeField] private float howlRange = 8f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private ParticleSystem howlParticle; // 하울링 파티클 시스템
    private int currentHealth;
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
            
            monster = gameObject.AddComponent<Monster>();
        }
        
        // 플레이어 참조 설정
        if (playerTransform != null)
        {
            monster.SetPlayer(playerTransform);
        }
        else
        {
            
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