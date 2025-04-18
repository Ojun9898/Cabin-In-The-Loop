using UnityEngine;

public interface ITurretState
{
    public TurretAI turret { get; set; }
    
    void Enter();
    void Stay();
    void Exit();
}