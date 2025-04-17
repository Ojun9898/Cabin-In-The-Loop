using UnityEngine;

public interface IState<T> where T : MonoBehaviour
{
    void Enter(T entity);
    void Execute(T entity);
    void Exit(T entity);
} 