using UnityEngine;
using System.Collections.Generic;

public class InsectStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<InsectBaseState> insectStates;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject poisonThornPrefab; // PoisonThorn 프리팹
    [SerializeField] private Transform firePoint; // 발사 위치
    [SerializeField] private float projectileSpeed = 10f; // 발사체 속도
    
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
        
        // 상태 목록이 비어있으면 기본 상태들을 생성
        if (insectStates == null || insectStates.Count == 0)
        {
            insectStates = new List<InsectBaseState>
            {
                new InsectSpawnState(),
                new InsectIdleState(),
                new InsectPatorlState(),
                new InsectChaseState(),
                new InsectAttackState(),
                new InsectHitState(),
                new InsectDeathState()
            };
        }
        
        // 각 상태를 초기화
        foreach (var state in insectStates)
        {
            // 1) 플레이어 Transform 전달
            state.SetPlayerTransform(playerTransform);
            
            // 2) 공격 상태는 추가 초기화
            if (state is InsectAttackState attackState)
                attackState.InitializeAttack(poisonThornPrefab, firePoint, projectileSpeed);
            
            // 3) 기존 Initialize 호출
            state.Initialize(monster);
        }
        
        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(insectStates);

        base.Initialize();
    }
    
    public void OnHit(int damage)
    {
        Monster monster = GetComponent<Monster>();
        monster.TakeDamage(damage);
        
        if (!monster.IsDead())
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
        
    }
} 