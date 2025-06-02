using UnityEngine;
using System.Collections.Generic;

public abstract class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<State<T>> states;
    protected Dictionary<EState, State<T>> stateDictionary = new Dictionary<EState, State<T>>();
    protected State<T> currentState;
    protected T owner;

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        owner = GetComponent<T>();

        if (states == null)
        {
            states = new List<State<T>>();

        }

        if (states.Count == 0)
        {

            return;
        }

        // 상태 사전 초기화
        stateDictionary.Clear();

        foreach (var state in states)
        {
            if (state == null)
            {

                continue;
            }

            state.Initialize(owner);

            // 중복 키 체크
            if (stateDictionary.ContainsKey(state.StateKey))
            {

                continue;
            }

            stateDictionary.Add(state.StateKey, state);
        }

        if (states.Count > 0)
        {
            currentState = states[0];
            currentState.EnterState();
        }
    }

    public void ResetStateMachine()
    {
        Initialize();
    }

protected virtual void Update()
    {
        if (currentState == null) return;
        
        currentState.UpdateState();
        
        EState nextState;
        if (currentState.IsStateEnd(out nextState))
        {
            ChangeState(nextState);
        }
    }
    
    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }
    
    public virtual void ChangeState(EState nextState)
    {
        // 현재 상태가 Death라면(사망 상태) 다른 상태로 절대 전환하지 않음
        if (currentState != null && currentState.StateKey == EState.Death)
            return;
        
        if (currentState != null)
        {
            currentState.ExitState();
        }
        
        if (stateDictionary.TryGetValue(nextState, out State<T> newState))
        {
            
            currentState = newState;
            currentState.EnterState();
        }
        else
        {
            
        }
    }
} 