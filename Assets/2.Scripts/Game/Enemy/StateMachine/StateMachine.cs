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
            Debug.LogWarning("States list is null. Initializing empty list.");
        }
        
        if (states.Count == 0)
        {
            Debug.LogWarning("States list is empty. No states will be initialized.");
            return;
        }
        
        // 상태 사전 초기화
        stateDictionary.Clear();
        
        foreach (var state in states)
        {
            if (state == null)
            {
                Debug.LogError("Null state found in states list. Skipping.");
                continue;
            }
            
            state.Initialize(owner);
            
            // 중복 키 체크
            if (stateDictionary.ContainsKey(state.StateKey))
            {
                Debug.LogError($"Duplicate state key found: {state.StateKey}. Skipping duplicate state.");
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
        if (currentState != null)
        {
            currentState.ExitState();
        }
        
        if (stateDictionary.TryGetValue(nextState, out State<T> newState))
        {
            Debug.Log($"Changing state from {currentState?.StateKey} to {newState.StateKey}");
            currentState = newState;
            currentState.EnterState();
        }
        else
        {
            Debug.LogError($"State {nextState} does not exist in the state machine");
        }
    }
} 