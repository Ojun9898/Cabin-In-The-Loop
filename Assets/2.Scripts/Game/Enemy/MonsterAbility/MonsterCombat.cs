using UnityEngine;

public class MonsterCombat
{
    private float attackDamage;
    private float attackRange;
    private float attackCooldown;
    private float lastAttackTime;
    
    public MonsterCombat(float attackDamage, float attackRange)
    {
        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
        this.attackCooldown = 1.5f; // 기본 공격 쿨다운
        this.lastAttackTime = -attackCooldown; // 처음에는 바로 공격 가능하도록 설정
    }
    
    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
    
    public void Attack()
    {
        if (CanAttack())
        {
            lastAttackTime = Time.time;
            // 실제 데미지 적용은 AttackState에서 처리
        }
    }
    
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    
    public float GetAttackRange()
    {
        return attackRange;
    }
    
    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }
} 