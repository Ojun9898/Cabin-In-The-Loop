using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
    protected IState<T> currentState;
    
    protected virtual void Start()
    {
        Initialize();
    }
    
    protected abstract void Initialize();
    
    protected virtual void Update()
    {
        currentState?.Execute(GetEntity());
    }
    
    protected abstract T GetEntity();
    
    public void ChangeState(IState<T> newState)
    {
        if (currentState != null)
            currentState.Exit(GetEntity());
        currentState = newState;
        currentState.Enter(GetEntity());
    }
} 