using UnityEngine;

public class DamageFieldEffect : MonoBehaviour
{
    public GameObject hitEffectPrefab; // 피격 이펙트 프리팹을 Inspector에서 할당
    public float effectDuration = 1f; // 이펙트 지속 시간

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트에 Enemy 태그가 있는지 확인 (태그 설정은 Unity Inspector에서 합니다.)
        if (other.CompareTag("Enemy"))
        {
            // 피격 이펙트 프리팹이 할당되어 있다면
            if (hitEffectPrefab != null)
            {
                // 충돌 지점에 이펙트 생성
                GameObject hitEffect = Instantiate(hitEffectPrefab, other.ClosestPoint(transform.position), Quaternion.identity);

                // 생성된 이펙트가 자동으로 제거되도록 설정 (선택 사항)
                Destroy(hitEffect, effectDuration);
            }
            else
            {
                Debug.LogWarning("피격 이펙트 프리팹이 할당되지 않았습니다!");
            }
        }
    }
}