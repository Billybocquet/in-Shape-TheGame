using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun2 : MonoBehaviour
{
    public bool CanGravityGun { get; private set; } = true;

    [Header("Fonctionnal Parameters")] 
    [SerializeField] private bool CanThrow;
    [SerializeField] private bool CanMove;
    [SerializeField] private bool CanRotate;
    
    [Header("Controls")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInputActions playerInputActions;
    
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Animator")] 
    [SerializeField] private Animator gravityGunAnimator;

    [Header("Parameters")]
    [SerializeField] private float maxGrabDistance;
    [SerializeField] private float throwForce;
    [SerializeField] private float lerpSpeed;

    [Header("Transform Parameter")] 
    [SerializeField] private Vector3 xRotation;
    [SerializeField] private Vector3 yRotation;
    [SerializeField] private Vector3 zRotation;
    [SerializeField, Range(0, 100)] private float speedRotation;
    [SerializeField] private float maxForward;
    [SerializeField] private float minForward;
    [SerializeField, Range(0, 100)] private float speedForward;

    [Header("LayerMask")]
    [SerializeField] private LayerMask layerMask;

    [Header("Holder")]
    [SerializeField] private Transform objectHolder;
    [SerializeField] private Vector3 holderOrigin;


    private Rigidbody grabbedRB;
    private Transform grabbedTransform;

    private float forwardMove;
    [HideInInspector] public bool IsEditing;
    
    void Awake()
    {
        IsEditing = false;
        
        objectHolder.localPosition = holderOrigin;
        forwardMove = holderOrigin.z;
        playerInput = GetComponent<PlayerInput>();
        
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        
        playerInputActions.Player.Grab.performed += Grab;
        playerInputActions.Player.Grab.canceled += Grab;
        playerInputActions.Player.Throw.performed += ThrowGrabbed;
        playerInputActions.Player.EditMode.performed += Editing;
    }
    
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxGrabDistance, Color.green);
        
        if (grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));
            
            gravityGunAnimator.SetTrigger("Use");
        }
        
        if (CanGravityGun)
        {
            if (CanRotate)
            {
                float rotateAxisX = playerInputActions.Player.Rotate.ReadValue<Vector2>().x;
                float rotateAxisY = playerInputActions.Player.Rotate.ReadValue<Vector2>().y;
                float rotateAxisZ = playerInputActions.Player.Rotate2.ReadValue<Vector2>().x;
                RotateGrabbed(rotateAxisX, rotateAxisY, rotateAxisZ);
            }


            if (CanMove)
            {
                float AxisFB = playerInputActions.Player.ForwardBack.ReadValue<float>();
                MoveGrabbed(AxisFB);
            }
        }
    }

    private void Grab(InputAction.CallbackContext context)
    {
        

        if (context.performed)
        {
            //Debug.Log("Grab! " + context.phase);

            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out hit, maxGrabDistance, layerMask))
            {
                grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                grabbedTransform = hit.collider.gameObject.GetComponent<Transform>();
                    
                if (grabbedRB)
                {
                    grabbedRB.isKinematic = true;
                }
            }
        }
        
        if (grabbedRB && context.canceled)
        {
            //Debug.Log("Drop! " + context.phase);
            
            gravityGunAnimator.SetTrigger("No");
                
            grabbedRB.isKinematic = false;
            IsEditing = false;
                
            objectHolder.localPosition = holderOrigin;
                
            grabbedTransform = null;
            grabbedRB = null;
        }
    }
    
    private void RotateGrabbed(float axisX, float axisY, float axisZ)
    {
        //Debug.Log("x : " + axisX + "y : " + axisY + "z : " + axisZ);
        
        if (grabbedRB && IsEditing)
        {
            grabbedTransform.Rotate(axisX * xRotation * speedRotation * Time.deltaTime); 
            
            grabbedTransform.Rotate(axisY * yRotation * speedRotation * Time.deltaTime); 
            
            grabbedTransform.Rotate(axisZ * zRotation * speedRotation * Time.deltaTime);
        }
    }

    private void Editing(InputAction.CallbackContext context)
    {
        if (grabbedRB && context.performed)
        {
            //Debug.Log("EditMode! " + context.phase);
            IsEditing = !IsEditing;
        }
    }

    private void MoveGrabbed(float axis)
    {
        //Debug.Log(axis);
        
        if (grabbedRB && !IsEditing)
        {
            forwardMove += axis * speedForward * Time.deltaTime;
            forwardMove = Mathf.Clamp(forwardMove, minForward, maxForward);

            objectHolder.localPosition = new Vector3(0, 0, forwardMove);
        }
    }

    private void ThrowGrabbed(InputAction.CallbackContext context)
    {
        if (CanThrow && grabbedRB)
        {
            if (context.performed)
            {
                //Debug.Log("Trow! " + context.phase);
                
                grabbedRB.isKinematic = false;
                grabbedRB.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);
                
                objectHolder.localPosition = holderOrigin;
                
                IsEditing = false;
                
                grabbedTransform = null;
                grabbedRB = null;
            }
        }
    }
}