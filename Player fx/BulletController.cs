using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Tooltip("총알의 이동 속도")]
    public float moveSpeed = 20f; // 총알이 날아가는 속도

    [Tooltip("총알이 자동으로 파괴될 시간 (초)")]
    public float destroyTime = 3f; // 총알이 너무 멀리 날아가지 않도록 자동으로 제거될 시간

    // Rigidbody를 사용할 경우
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 참조 가져오기

        // 일정 시간 후 총알 파괴 (메모리 누수 방지)
        Destroy(gameObject, destroyTime);
    }

    void Start()
    {
        // Rigidbody에 앞으로 나아가는 힘을 가합니다.
        // transform.forward는 총알 오브젝트의 Z축(앞 방향)을 나타냅니다.
        if (rb != null)
        {
            rb.velocity = transform.forward * moveSpeed;
        }
        else
        {
            Debug.LogWarning("BulletController: Rigidbody가 없습니다! 총알이 움직이지 않을 수 있습니다.");
        }
    }

    /*
    // Rigidbody를 사용하지 않고 Transform을 직접 이동시킬 경우 (Start() 대신 Update() 사용)
    void Update()
    {
        // 매 프레임마다 총알을 앞으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
    */

    // 총알이 다른 오브젝트와 충돌했을 때 (Is Trigger가 true일 경우)
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트에 따라 다른 동작 수행 (예: 데미지 주기)
        // Debug.Log("총알이 충돌했습니다: " + other.name);

        // 총알이 충돌하면 바로 파괴
        Destroy(gameObject);
    }
}