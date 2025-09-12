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
                Debug.LogError("[InsectStateMachine] Start(): Player를 찾을 수 없습니다.");
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
            Debug.LogWarning("Monster component not found on InsectStateMachine. Adding it automatically.");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
            Debug.LogWarning("Monster component not found on InsectStateMachine. Adding it automatically.");
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
            Debug.LogWarning("Player Transform not set in InsectStateMachine. Please assign it in the inspector.");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
            Debug.LogWarning("Player Transform not set in InsectStateMachine. Please assign it in the inspector.");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
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
<<<<<<< HEAD
<<<<<<< HEAD
        
=======
        Debug.Log($"Insect state changed: {previousState} -> {nextState}");
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
=======
        Debug.Log($"Insect state changed: {previousState} -> {nextState}");
>>>>>>> 4301af75291249a954534f393c172d24ac73c9c8
    }
} 