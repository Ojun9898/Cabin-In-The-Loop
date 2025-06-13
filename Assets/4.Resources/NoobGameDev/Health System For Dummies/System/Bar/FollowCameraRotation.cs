using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    [SerializeField] Transform target;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        // 카메라 회전값 가져오기
        Vector3 camEuler = target.rotation.eulerAngles;

        // Y축만 180도 뒤로 돌림
        camEuler.y += 180f;

        // 수정된 회전 적용
        transform.rotation = Quaternion.Euler(camEuler);
    }
}