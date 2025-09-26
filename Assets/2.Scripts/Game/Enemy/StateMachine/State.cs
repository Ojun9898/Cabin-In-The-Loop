using UnityEngine;

public enum EState
{
    Spawn,
    Idle,
    Move,
    Attack,
    Death,
    Hit,
    Wander,
    Chase,
    Rattack
}

public abstract class State<T> where T : MonoBehaviour
{
    [SerializeField] protected EState stateKey;
    public EState StateKey => stateKey;
    
    public abstract void Initialize(T owner);
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract bool IsStateEnd(out EState nextState);
} 