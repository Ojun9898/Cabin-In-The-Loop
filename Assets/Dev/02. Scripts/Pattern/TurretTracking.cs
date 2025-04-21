using UnityEngine;

public class TurretTracking : MonoBehaviour, ITurretState
{
    public TurretAI turret { get; set; }
    
    private float timer;
    
    void Start()
    {
        turret = this.GetComponent<TurretAI>();
    }
    
    public void Enter()
    {
        
    }

    public void Stay()
    {
        if (turret.targets.Count > 0)
        {
            LookAtTarget();
            ShootCooldown();
        }
        else
            turret.ChangeToPatrol();
    }

    public void Exit()
    {
        
    }
    
    private void LookAtTarget()
    {
        var targetDir = (turret.currentTarget.position - this.transform.position).normalized;
        turret.headTf.rotation = Quaternion.Slerp(turret.headTf.rotation, Quaternion.LookRotation(targetDir), 0.1f);
    }

    private void ShootCooldown()
    {
        timer += Time.deltaTime;
        if (timer >= turret.shootCooldown)
        {
            timer = 0f;
            turret.ChangeToAttack();
        }
    }
}