using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController2 : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool ShouldSprint => characterController.isGrounded;
    private bool ShouldJump => characterController.isGrounded;
    private bool ShouldCrouch => !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")] 
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool WillSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;

    [Header("Controls")] 
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInputActions playerInputActions;

    [Header("Movement Parameters")] 
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")] 
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")] 
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("HeadBob Parameters")] 
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parameters")] 
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Arnimator")]
    [SerializeField] private Animator robotAnimator;
    private Vector3 lastPosition;
    
    private Vector3 hitPointNormal;
    private bool IsSprinting;

    private GravityGun2 gravityGun2;
    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 1f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;
    
    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        gravityGun2 = GetComponent<GravityGun2>();
        
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += HandleJump;
        playerInputActions.Player.Crouch.performed += HandleCrouch;
        playerInputActions.Player.Zoom.performed += HandleZoom;
        playerInputActions.Player.Zoom.canceled += HandleZoom;
        playerInputActions.Player.Sprint.performed += HandleSprint;
        playerInputActions.Player.Sprint.canceled += HandleSprint;
    }
    
    void FixedUpdate()
    {
        if (CanMove)
        {
            float inputMoveX = playerInputActions.Player.Movement.ReadValue<Vector2>().y;
            float inputMoveY = playerInputActions.Player.Movement.ReadValue<Vector2>().x;
            
            if (!gravityGun2.IsEditing)
                HandleMovementInput(inputMoveX, inputMoveY);

            float inputLookX = playerInputActions.Player.Look.ReadValue<Vector2>().x;
            float inputLookY = playerInputActions.Player.Look.ReadValue<Vector2>().y;
            
            if (!gravityGun2.IsEditing)
                HandleMouseLook(inputLookX, inputLookY);
            
            if (canUseHeadBob)
                HandleHeadBob();

            ApplyFinalMovement();
        }
    }

    private void HandleMovementInput(float inputVectorX, float inputVectorY)
    {
        //Debug.Log("x : " + inputVectorX + "y : "+ inputVectorY);
        
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed :  walkSpeed) * inputVectorX, (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * inputVectorY);

        lastPosition = gameObject.transform.position;
        
        if (characterController.isGrounded && !IsSprinting && currentInput != Vector2.zero)
        {
            robotAnimator.SetTrigger("Walk");
        }
        else if (characterController.isGrounded && IsSprinting && currentInput != Vector2.zero)
        {
            robotAnimator.SetTrigger("Run");
        }
        else if (characterController.isGrounded && currentInput == Vector2.zero)
        {
            robotAnimator.SetTrigger("Idle");
        }
        else if (!characterController.isGrounded)
        {
            robotAnimator.SetTrigger("Jump");
        }

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (canSprint && ShouldSprint)
        {
            if (context.performed)
                IsSprinting = true;
            
            if (context.canceled)
                IsSprinting = false;
        }
    }

    private void HandleMouseLook(float inputVectorX, float inputVectorY)
    {
        //Debug.Log("x : " + inputVectorX + "y : "+ inputVectorY);
        
        rotationX -= inputVectorY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, inputVectorX * lookSpeedX, 0);
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        if (canJump && ShouldJump)
            if (context.performed)
            {
                //Debug.Log("Jump!" + context.phase);
                moveDirection.y = jumpForce;
            }
    }

    private void HandleCrouch(InputAction.CallbackContext context)
    {
        if (canCrouch && ShouldCrouch)
            if (context.performed)
            {
                //Debug.Log("Crouch!" + context.phase);
                StartCoroutine(CrouchStand());
            }
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom(InputAction.CallbackContext context)
    {
        if (canZoom)
        {
            if (context.performed)
            {
                //Debug.Log("Zoom!" + context.phase);
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }
            
                zoomRoutine = StartCoroutine(ToggleZoom(true));
            }

            if (context.canceled)
            {
                //Debug.Log("Zoom!" + context.phase);
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }
            
                zoomRoutine = StartCoroutine(ToggleZoom(false));
            }
        }
    }
    
    private void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (WillSlideOnSlopes && IsSliding)
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        //if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f));
            //yield break;
        
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch); 
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
