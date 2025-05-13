using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendigoStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<VendigoBaseState> vendigoStates;
    [SerializeField] private Transform playerTransform;
    
    public Transform PlayerTransform => playerTransform;
    
    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
    }
    
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
                Debug.LogError("[VendigoStateMachine] Start(): Player를 찾을 수 없습니다.");
                return;
            }
        }

        // 초기화 호출
        Initialize();
    }
    
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리)
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
        if (vendigoStates == null || vendigoStates.Count == 0)
        {
            vendigoStates = new List<VendigoBaseState>
            {
                new VendigoSpawnState(),
                new VendigoIdleState(),
                new VendigoPatorlState(),
                new VendigoChaseState(),
                new VendigoAttackState(),
                new VendigoHitState(),
                new VendigoDeathState()
            };
        }
        
        // 각 상태 초기화 & 플레이어 Transform 주입
        foreach (var state in vendigoStates)
        {
            state.Initialize(monster);
            state.SetPlayerTransform(playerTransform);
        }

        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(vendigoStates);

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
    }
    
    
}
