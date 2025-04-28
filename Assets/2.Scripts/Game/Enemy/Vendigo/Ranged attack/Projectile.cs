using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]


public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private float damage;
    // 일정시간뒤에 파괴
    private const float LIFETIME = 4.5f;

    // 방향 계산부터 인스턴스 생성, 초기화까지 전부 처리
    public static void Throw
    (
        GameObject prefab,
        Transform muzzlePoint,
        Vector3 targetPosition,
        float speed,
        float damage,
        float upwardSpeed
    )
    {
        // 1) 방향 계산
        Vector3 start = muzzlePoint.position;
        Vector3 dir = targetPosition - start;
        dir.y = 0f;
        dir.Normalize();

        // 2) 생성 & 초기화
        var go = Object.Instantiate(prefab, start, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        proj.Initialize(dir, speed, damage, upwardSpeed);
    }
    
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    
    public void Initialize(Vector3 direction, float speed, float damage, float upwardSpeed)
    {
        this.damage = damage;
        // 발사속도 :수평 속도 + 수직 속도 
        Vector3 launchVel = (direction * speed) + (Vector3.up * upwardSpeed);
        rb.velocity = Vector3.zero; // 기존 속도 초기화
        rb.AddForce(launchVel * rb.mass, ForceMode.Impulse);
        Destroy(gameObject, LIFETIME);
    }
    
    
    // 향후 플레이어 오브젝트에 따라서 Collider 또는 Trigger 로 수정
    void OnCollisionEnter(Collision collision)
    {
        // “Player” 태그를 가진 객체와 부딪히면
        if (collision.gameObject.CompareTag("Player"))
        {
            var hp = collision.gameObject.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(damage);
            Destroy(gameObject);
        }
       
    }
}
