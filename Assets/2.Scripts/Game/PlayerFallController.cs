using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerFallController : MonoBehaviour
{
    private CharacterController controller;
    private bool isFalling = false;
    private float verticalVelocity = 0f;
    private float gravity = 9.8f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isFalling)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            Vector3 fallMovement = new Vector3(0, verticalVelocity, 0);
            controller.Move(fallMovement * Time.deltaTime);
        }
    }

    public void Fall()
    {
        isFalling = true;
        verticalVelocity = 0f;
    }
}