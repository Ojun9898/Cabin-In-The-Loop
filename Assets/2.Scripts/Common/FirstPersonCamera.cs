using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("싱글톤 참조")]
    // GameManager에서 관리하는 플레이어와 카메라 트랜스폼을 가져옵니다.
    private Transform playerBody;        // 플레이어 몸체(요 회전을 담당)
    private Transform cameraTransform;   // 카메라 트랜스폼

    [Header("민감도")]
    public float mouseSensitivity = 3.0f; // 마우스 민감도

    [Header("카메라 위치 설정")]
    public float cameraHeight = 1.8f;    // 카메라가 플레이어 바닥에서 떨어진 높이

    [Header("회전 제한")]
    public float minPitch = -60f;       // 피치(상하) 최소 각도
    public float maxPitch = 60f;        // 피치(상하) 최대 각도

    private float yaw = 0f;             // 요(좌우) 회전값
    private float pitch = 0f;           // 피치(상하) 회전값

    void Awake()
    {
        // GameManager 싱글톤에서 Transform을 할당
        playerBody = GameManager.Instance.playerTransform;       // GameManager의 playerTransform
        cameraTransform = GameManager.Instance.cameraTransform;  // GameManager의 cameraTransform

        // 카메라를 플레이어 몸체 자식으로 설정하면 자연스럽게 따라갑니다.
        cameraTransform.SetParent(playerBody);
        // 로컬 위치를 머리 높이에 맞게 초기화
        cameraTransform.localPosition = new Vector3(0f, cameraHeight, 0f);
    }

    void Start()
    {
        // 커서 고정 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 초기 회전 값 가져오기
        Vector3 camAngles = cameraTransform.localEulerAngles;
        pitch = camAngles.x;               // 카메라의 X축 회전값
        yaw = playerBody.eulerAngles.y;    // 플레이어 몸체의 Y축 회전값
    }

    void Update()
    {
        // 마우스 입력으로 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;      // 좌우 회전 누적
        pitch -= mouseY;    // 상하 회전 누적
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // 피치 값 제한

        // 카메라 로컬 X축 회전(피치 적용)
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        // 플레이어 몸체 Y축 회전(요 적용)
        playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    /// <summary>
    /// 지면(수평면) 상의 카메라 전방 벡터 반환
    /// </summary>
    public Vector3 GetCameraForwardOnPlane()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    /// <summary>
    /// 플레이어 요(좌우) 회전값을 Quaternion으로 반환
    /// </summary>
    public Quaternion GetYawRotation()
    {
        return Quaternion.Euler(0f, yaw, 0f);
    }
}
