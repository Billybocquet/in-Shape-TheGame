using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using Debug = UnityEngine.Debug;

public class FirstPersonController3 : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool ShouldSprint => characterController.isGrounded;
    private bool ShouldJump => characterController.isGrounded;
    private bool ShouldCrouch => !duringCrouchAnimation && characterController.isGrounded;

    private FMOD.Studio.EventInstance jump;

    [Header("Functional Options")] 
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool WillSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;

    [Header("Controls")] 
    [SerializeField] private PlayerInput playerInput;

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

    [Header("Walk Sound")] 
    [SerializeField] private FootstepManager footstepManager;
    [SerializeField] private float footstepSpeed;
    [SerializeField] private float timerr;

    [Header("Arnimator")]
    [SerializeField] private Animator robotAnimator;
    private Vector3 lastPosition;

    private float inputMoveX;
    private float inputMoveY;

    private float inputLookX;
    private float inputLookY;
    
    private Vector3 hitPointNormal;
    private bool IsSprinting;

    private GravityGun3 gravityGun3;

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
        gravityGun3 = GetComponent<GravityGun3>();
        
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void FixedUpdate()
    {
        if (CanMove)
        {
            if (!gravityGun3.IsEditing)
                HandleMovementInput(inputMoveX, inputMoveY);

            if (!gravityGun3.IsEditing)
                HandleMouseLook(inputLookX, inputLookY);
            
            if (canUseHeadBob)
                HandleHeadBob();

            ApplyFinalMovement();
            
            AnimationPlayer();
            
            
        }
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        inputMoveX = context.ReadValue<Vector2>().y;
        inputMoveY = context.ReadValue<Vector2>().x;
    }

    public void LookInput(InputAction.CallbackContext context)
    {
        inputLookX = context.ReadValue<Vector2>().x;
        inputLookY = context.ReadValue<Vector2>().y;
    }
    
    private void HandleMovementInput(float inputVectorX, float inputVectorY)
    {
        //Debug.Log("x : " + inputVectorX + "y : "+ inputVectorY);
        
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed :  walkSpeed) * inputVectorX, (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * inputVectorY);

        lastPosition = gameObject.transform.position;

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    public void HandleSprint(InputAction.CallbackContext context)
    {
        if (canSprint && ShouldSprint)
        {
            if (context.performed)
                IsSprinting = true;
            
            if (context.canceled)
                IsSprinting = false;
        }
    }

    public void HandleMouseLook(float inputVectorX, float inputVectorY)
    {
        //Debug.Log("x : " + inputVectorX + "y : "+ inputVectorY);
        
        rotationX -= inputVectorY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, inputVectorX * lookSpeedX, 0);
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (canJump && ShouldJump)
            if (context.performed)
            {
                //Debug.Log("Jump!" + context.phase);
                moveDirection.y = jumpForce;
                if (characterController.isGrounded)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Players/Jump");
                }
            }
    }

    public void HandleCrouch(InputAction.CallbackContext context)
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

    public void HandleZoom(InputAction.CallbackContext context)
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
        
        if (timerr > footstepSpeed&&characterController.velocity.magnitude >= 0.05)
        {
            footstepManager.PlayFootstep();
            timerr = 0.0f;
        }

        timerr += Time.deltaTime;
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

    private void AnimationPlayer()
    {
        if (characterController.isGrounded)
        {
            if (currentInput != Vector2.zero)
            {
                if (!IsSprinting)
                {
                    Debug.Log("Walk");
                    robotAnimator.SetBool("Jump", false);
                    robotAnimator.SetBool("Walk", true);
                    robotAnimator.SetBool("Run", false);
                    robotAnimator.SetBool("Idle", false);
                }
                else
                {
                    Debug.Log("Run");
                    robotAnimator.SetBool("Jump", false);
                    robotAnimator.SetBool("Walk", false);
                    robotAnimator.SetBool("Run", true);
                    robotAnimator.SetBool("Idle", false);
                }
            }
            else
            {
                Debug.Log("Idle");
                robotAnimator.SetBool("Jump", false);
                robotAnimator.SetBool("Walk", false);
                robotAnimator.SetBool("Run", false);
                robotAnimator.SetBool("Idle", true);
            }
            
        }
        else
        {
            Debug.Log("Jump");
            robotAnimator.SetBool("Jump", true);
            robotAnimator.SetBool("Walk", false);
            robotAnimator.SetBool("Run", false);
            robotAnimator.SetBool("Idle", false);
        }
    }
}
