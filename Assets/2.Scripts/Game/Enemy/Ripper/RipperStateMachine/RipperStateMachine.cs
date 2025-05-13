using System.Collections.Generic;
using UnityEngine;

public class RipperStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<RipperBaseState> ripperStates;
    [SerializeField] private Transform playerTransform;
    
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
        if (ripperStates == null || ripperStates.Count == 0)
        {
            ripperStates = new List<RipperBaseState>
            {
                new RipperSpawnState(),
                new RipperIdleState(),
                new RipperPatorlState(),
                new RipperChaseState(),
                new RipperAttackState(),
                new RipperHitState(),
                new RipperDeathState()
            };
        }
        
        // 각 상태 초기화 & 플레이어 Transform 주입
        foreach (var state in ripperStates)
        {
            state.Initialize(monster);
            state.SetPlayerTransform(playerTransform);
        }

        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(ripperStates);

        base.Initialize();
    }
    
    private void OnEnable()
    {
        // states가 준비되지 않았다면 패스
        if (states == null || states.Count == 0) 
            return;

        ChangeState(EState.Idle);
    }

    
    public void OnHit(int damage)
    {
        Monster ripper = GetComponent<Monster>();
        ripper.TakeDamage(damage);
    
        if (!ripper.IsDead())
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
