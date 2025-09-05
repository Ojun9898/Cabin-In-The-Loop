using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("총 발사 설정")]
    [Tooltip("총알이 발사될 지점의 Transform")]
    public Transform firePoint; 

    [Tooltip("발사할 총알 프리팹 (선택 사항)")]
    public GameObject bulletPrefab; 

    [Tooltip("총 발사음 오디오 클립 (선택 사항)")]
    public AudioClip shootSound;
    private AudioSource audioSource; // 오디오 재생을 위한 컴포넌트

    [Header("이펙트 설정")]
    [Tooltip("총구 화염(Muzzle Flash) 파티클 시스템")]
    public ParticleSystem muzzleFlashParticle;

    // 총 발사 쿨타임 (다음 발사까지의 지연 시간)
    [Tooltip("총 발사 쿨타임 (초)")]
    public float fireRate = 0.5f; 
    private float nextFireTime = 0f; // 다음 발사 가능 시간

    void Awake()
    {
        // AudioSource 컴포넌트가 없으면 추가 (사운드 재생용)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 시작 시 총구 화염 파티클 비활성화 (선택 사항, Play On Awake가 false인 경우)
        if (muzzleFlashParticle != null)
        {
            muzzleFlashParticle.Stop(); // 파티클을 멈추고 초기 상태로
            muzzleFlashParticle.Clear(); // 남아있는 파티클 제거
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 발사 (또는 Input.GetButton("Fire1") 등)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // 다음 발사 가능 시간 업데이트
        }
    }

    /// <summary>
    /// 총을 발사하는 함수. 외부에서 호출될 수도 있습니다 (예: 애니메이션 이벤트, AI)
    /// </summary>
    public void Shoot()
    {
        // 1. 총구 화염 파티클 재생
        if (muzzleFlashParticle != null)
        {
            muzzleFlashParticle.Play(); // 파티클 시스템 재생
        }

        // 2. 총 발사음 재생
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound); // 한 번만 재생되는 사운드
        }

        // 3. 총알 생성 (선택 사항: 총알 프리팹이 할당된 경우)
        if (bulletPrefab != null && firePoint != null)
        {
            // firePoint 위치와 회전으로 총알 생성
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // 추가적으로 총알에 힘을 가하거나 데미지 로직을 추가할 수 있습니다.
        }

        Debug.Log("총 발사! (VFX 및 사운드 재생)");
    }
}