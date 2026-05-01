using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private float MoveSpeed => moveSpeed;

    [SerializeField]
    private float sprintModifier;
    private float SprintModifier => sprintModifier;

    [SerializeField]
    private float playerGravity;
    private float PlayerGravity => playerGravity;

    [SerializeField]
    private float jumpHeight;
    private float JumpHeight => jumpHeight;

    [SerializeField]
    private int coyoteFrames;
    private int CoyoteFrames => coyoteFrames;

    [SerializeField]
    private float cameraSensitivity;
    private float CameraSensitivity => cameraSensitivity;

    [SerializeField]
    private CharacterController playerController;
    private CharacterController PlayerController => playerController;

    [SerializeField]
    private PlayerInput playerInputController;
    private PlayerInput PlayerInputController => playerInputController;

    [SerializeField]
    private Camera playerCamera;
    private Camera PlayerCamera => playerCamera;

    private float cameraPitch;
    private float verticalVelocity;

    private InputAction moveAction;
    private Vector2 moveInput;

    private InputAction lookAction;
    private Vector2 lookInput;

    private InputAction sprintAction;

    private InputAction jumpAction;
    private int coyoteFramesRemaining;


    private void OnEnable()
    {
        EnableActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnableActions()
    {
        var actions = PlayerInputController.actions;

        moveAction = actions["Move"];
        lookAction = actions["Look"];
        sprintAction = actions["Sprint"];
        jumpAction = actions["Jump"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        lookAction.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => lookInput = Vector2.zero;

        actions.Enable();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    private void DisableActions()
    {
        if (moveAction != null)
        {
            moveAction.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
            moveAction.canceled -= ctx => moveInput = Vector2.zero;
        }
        if (lookAction != null)
        {
            lookAction.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
            lookAction.canceled -= ctx => lookInput = Vector2.zero;
        }
    }

    private void Update()
    {
        Look();
        Movement();
    }

    private void FixedUpdate()
    {
        if (PlayerController.isGrounded)
        {
            coyoteFramesRemaining = CoyoteFrames;
        }
        else
        {
            if(coyoteFramesRemaining > 0)
                coyoteFramesRemaining -= 1;
        }
    }

    private void Movement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (PlayerController.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        move = move * MoveSpeed;

        if (sprintAction.IsPressed())
            move *= SprintModifier;

        if(jumpAction.WasPressedThisFrame() && coyoteFramesRemaining >= 1)
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * PlayerGravity);

        if (!PlayerController.isGrounded)
            verticalVelocity += PlayerGravity * Time.deltaTime;

        PlayerController.Move((move + (Vector3.up * verticalVelocity)) * Time.deltaTime);
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * lookInput.x * CameraSensitivity);

        cameraPitch -= lookInput.y * CameraSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        PlayerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }
}
