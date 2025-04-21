using UnityEngine;

public class TurretPatrol : MonoBehaviour, ITurretState
{
    public TurretAI turret { get; set; }
    
    private float turnSpeed = 1f;
    private float theta;

    void Start()
    {
        turret = this.GetComponent<TurretAI>();
    }
    
    public void Enter()
    {
        
    }

    public void Stay()
    {
        RotationTurret();
    }

    public void Exit()
    {
        
    }
    
    private void RotationTurret()
    {
        theta += Time.deltaTime * turnSpeed;
        turret.headTf.localRotation = Quaternion.Euler(Vector3.up * 60f * Mathf.Sin(theta));
    }
}