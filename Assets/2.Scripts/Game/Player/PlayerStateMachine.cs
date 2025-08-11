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
    
    private PlayerStatus _status;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _weaponController = GetComponent<WeaponController>();
        animator = GetComponent<Animator>();
        
        // ★ ADD: PlayerStatus 보장 + 이동속도 동기화
        _status = PlayerStatus.Ensure();              // 메인씬에 없어도 자동 생성/로드
        RefreshMoveSpeedFromStatus();                 // JSON에서 Speed 읽어와 적용

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
    
    // ★ ADD: JSON(PlayerStatus)에서 Speed 읽어와 currentMoveSpeed 갱신
    public void RefreshMoveSpeedFromStatus()
    {
        if (_status != null)
        {
            currentMoveSpeed = _status.GetTotalStat(StatusType.Speed);
        }
        else
        {
            currentMoveSpeed = moveSpeed; // 백업값
        }
    }

    // ★ ADD: 외부에서 현재 이동속도 읽을 때 쓰기 편한 프로퍼티(옵션)
    public float MoveSpeed => currentMoveSpeed;

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