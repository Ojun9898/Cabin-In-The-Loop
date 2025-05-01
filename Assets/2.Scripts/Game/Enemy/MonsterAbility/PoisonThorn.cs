using UnityEngine;

public class PoisonThorn : MonoBehaviour
{
    private float damageAmount;
    private float lifeTime = 5f; // 발사체 수명

    public void Initialize(float damage)
    {
        damageAmount = damage;
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동으로 파괴
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지 적용
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // 충돌 후 발사체 파괴
            Destroy(gameObject);
        }
    }
}