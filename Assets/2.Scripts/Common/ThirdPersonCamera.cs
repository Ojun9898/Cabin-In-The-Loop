using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // 따라갈 대상
    public float mouseSensitivity = 3.0f;
    public float scrollSensitivity = 2.0f;

    public float distance = 6.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float height = 3.0f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw = 0f;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 마우스 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 마우스 휠 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        distance -= scroll;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 위치 계산
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = target.position + rotation * new Vector3(0, height, -distance);

        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10f);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public Vector3 GetCameraForwardOnPlane()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Quaternion GetYawRotation()
    {
        return Quaternion.Euler(0f, yaw, 0f);
    }
}