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
    private int fixedFramesForSlide;
    private int FixedFramesForSlide => fixedFramesForSlide;

    [SerializeField]
    private float slideMomentum;
    private float SlideMomentum => slideMomentum;

    [SerializeField]
    private float momentumFallOff;
    private float MomentumFallOff => momentumFallOff;

    [SerializeField]
    private float playerGravity;
    private float PlayerGravity => playerGravity;

    [SerializeField]
    private float crashVelocity;
    private float CrashVelocity => crashVelocity;

    [SerializeField]
    private int framesForBigJump;
    private int FramesForBigJump => framesForBigJump;

    [SerializeField]
    private float jumpHeight;
    private float JumpHeight => jumpHeight;

    [SerializeField]
    private float bigJumpHeight;
    private float BigJumpHeight => bigJumpHeight;

    [SerializeField]
    private int coyoteFrames;
    private int CoyoteFrames => coyoteFrames;

    [SerializeField]
    private int framesBeforeCrash;
    private int FramesBeforeCrash => framesBeforeCrash;

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

    Vector3 targetMove = Vector3.zero;

    private InputAction crouchAction;
    private Vector3 crouchVector = new Vector3(1, 0.5f, 1);
    private int framesForSlideRemaining;
    bool isSliding;

    bool isCrouched;
    bool isSprinting;

    private float momentum = 1f;
    private float momentumMinimum = 1f;

    int remainingFramesForBigJump;
    int framesInTheAir;


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
        crouchAction = actions["Crouch"];

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
        if (momentum > momentumMinimum)
            momentum -= MomentumFallOff * Time.deltaTime;
        else
            momentum = 1f;


        isCrouched = crouchAction.IsInProgress() && coyoteFramesRemaining > 0;
        isSprinting = sprintAction.IsInProgress() && !isCrouched && PlayerController.isGrounded;

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

        if (isSprinting)
        {
            if (framesForSlideRemaining > 0)
            {
                framesForSlideRemaining -= 1;
            }
        }
        else
            framesForSlideRemaining = FixedFramesForSlide;

        if(remainingFramesForBigJump > 0)
        {
            remainingFramesForBigJump -= 1;
        }

        if (!PlayerController.isGrounded)
        {
            framesInTheAir += 1;
        }
        else
            framesInTheAir = 0;
    }

    private void Movement()
    {
        if ((isSliding && !crouchAction.IsInProgress()) || momentum <= 0.5f)
        {
            momentumMinimum = 1f;
            isSliding = false;
        }

        if(!isSliding && framesInTheAir < 1)
        {
            momentum = momentumMinimum;
        }

        if (!isSliding)
            targetMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        targetMove.Normalize();


        if (PlayerController.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        targetMove *= MoveSpeed;

        if (isSprinting)
            targetMove *= SprintModifier;

        if (jumpAction.WasPressedThisFrame() && coyoteFramesRemaining >= 1)
        {
            verticalVelocity = remainingFramesForBigJump > 0 ? BigJumpHeight : JumpHeight;
            isCrouched = false;
            coyoteFramesRemaining = 0;
            framesInTheAir = 1;
        }

        if (isCrouched && framesInTheAir == 0)
        {
            transform.localScale = crouchVector;
            verticalVelocity = CrashVelocity;

            if (!isSliding)
            {
                if (framesForSlideRemaining == 0)
                {
                    isSliding = true;
                    momentum = SlideMomentum;
                    momentumMinimum = 0f;
                }
                else
                {
                    targetMove *= 0.5f;
                }
            }
        }
        else
            transform.localScale = Vector3.one;

        if (!PlayerController.isGrounded)
        {
            verticalVelocity += PlayerGravity * Time.deltaTime;
            if (crouchAction.IsInProgress() && framesInTheAir > FramesBeforeCrash)
            {
                verticalVelocity = CrashVelocity;
                remainingFramesForBigJump = FramesForBigJump;
            }
        }

        PlayerController.Move(((targetMove * momentum) + (Vector3.up * verticalVelocity)) * Time.deltaTime);
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * lookInput.x * CameraSensitivity);

        cameraPitch -= lookInput.y * CameraSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        PlayerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }
}
