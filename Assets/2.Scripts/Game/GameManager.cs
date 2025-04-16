using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform playerTransform;
    public Transform CameraTransform;

    protected override void Awake()
    {
        base.Awake();

        // Player 태그를 가진 오브젝트 찾아서 Transform 바인딩
        PlayerStateMachine playerSM = FindObjectOfType<PlayerStateMachine>();
        if (playerSM != null)
        {
            playerTransform = playerSM.transform;
        }
    }
}
