using UnityEngine;
using System.Collections.Generic;


public class ZombieStateMachine : StateMachine<Monster>
{
    [SerializeField] private List<ZombieBaseState> zombieStates;
    [SerializeField] private Transform playerTransform;
    
    public Transform PlayerTransform => playerTransform;
    
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
                Debug.LogError("[ZombieStateMachine] Start(): Player를 찾을 수 없습니다.");
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
            state.SetPlayerTransform(playerTransform);
        }
        
        // 상태 목록을 상태 머신에 등록
        states = new List<State<Monster>>(zombieStates);

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
        Monster zombie = GetComponent<Monster>();
        zombie.TakeDamage(damage);
        
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
    }
} 