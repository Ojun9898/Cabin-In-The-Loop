using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(WeaponController))]
public class PlayerStateMachine : MonoBehaviour
{
    public IPlayerState CurrentState;
    public IPlayerState PreviousState;


    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterController controller;

    public float moveSpeed = 2f;
    public float jumpSpeed = 7f;
    public float rotationSpeed = 10f;
    public float gravity = -9.8f;
    public Vector3 velocity;
    public bool canHit = true;
    
    public float currentMoveSpeed;

    private WeaponController _weaponController;
    
    [HideInInspector] public bool attackPressed;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool runPressed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _weaponController = GetComponent<WeaponController>();
        animator = GetComponent<Animator>();
        currentMoveSpeed = moveSpeed;

        ChangeState(new PlayerIdleState());
    }

    void Update()
    {
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -1f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _weaponController.EquipWeapon(WeaponType.Axe);
            WeaponSaveSystem.SaveWeapon(WeaponType.Axe);
        }
        
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _weaponController.EquipWeapon(WeaponType.CrowBar);
            WeaponSaveSystem.SaveWeapon(WeaponType.CrowBar);
        }
        
        controller.Move(velocity * Time.deltaTime);

        if (CurrentState != null)
            CurrentState.Execute(this);
        
        
    }

    public void ChangeState(IPlayerState newState)
    {
        if (CurrentState != null)
            CurrentState.Exit(this);

        PreviousState = CurrentState;
        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.Enter(this);
    }
    
    public Vector3 GetCameraRelativeMoveDirection()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 camForward = GameManager.Instance.cameraTransform.forward;
        Vector3 camRight = GameManager.Instance.cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        return camForward * inputDir.z + camRight * inputDir.x;
    }
    
    public void RotateTowardsCameraDirection()
    {
        Vector3 targetDir = GetCameraRelativeMoveDirection();

        if (targetDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnRun(InputAction.CallbackContext context)
    {
        runPressed = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            attackPressed = true;
    }
}