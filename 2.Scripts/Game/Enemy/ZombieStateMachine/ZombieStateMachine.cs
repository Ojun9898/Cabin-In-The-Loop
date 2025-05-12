using UnityEngine;
using System.Collections.Generic;


public class ZombieStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<ZombieBaseState> zombieStates;
    [SerializeField] private Transform playerTransform;
    
    protected override void Initialize()
    {
        // Monster 컴포넌트 가져오기
        Monster monster = GetComponent<Monster>();
        if (monster == null)
        {
            Debug.LogWarning("Monster component not found on ZombieStateMachine. Adding it automatically.");
            monster = gameObject.AddComponent<Monster>();
        }
        
        // 플레이어 참조 설정
        if (playerTransform != null)
        {
            monster.SetPlayer(playerTransform);
        }
        else
        {
            Debug.LogWarning("Player Transform not set in ZombieStateMachine. Please assign it in the inspector.");
        }
        
        // 상태 목록이 비어있으면 기본 상태들을 생성
        if (zombieStates == null || zombieStates.Count == 0)
        {
            zombieStates = new List<ZombieBaseState>
            {
                new ZombieSpawnState(),
                new ZombieIdleState(),
                new ZombiePatorlState(),
                new ZombieChaseState(),
                new ZombieAttackState(),
                new ZombieHitState(),
                new ZombieDeathState()
            };
        }
        
        // 각 상태를 초기화
        foreach (var state in zombieStates)
        {
            state.Initialize(monster);
        }
        
        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(zombieStates);

        base.Initialize();
    }
    
    public void OnHit(int damage)
    {
        Monster zombie = GetComponent<Monster>();
        zombie.OnHit(damage);
        
        if (!zombie.IsDead())
        {
            ChangeState(EState.Hit);
        }
        else
        {
            ChangeState(EState.Death);
        }
    }
    
    // 디버깅을 위한 현재 상태 로그 출력
    public override void ChangeState(EState nextState)
    {
        EState previousState = currentState?.StateKey ?? EState.Idle;
        base.ChangeState(nextState);
        Debug.Log($"Zombie state changed: {previousState} -> {nextState}");
    }
} 