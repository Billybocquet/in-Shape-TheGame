using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun3 : MonoBehaviour
{
    public bool CanGravityGun { get; private set; } = true;

    [Header("Fonctionnal Parameters")] 
    [SerializeField] private bool CanThrow;
    [SerializeField] private bool CanMove;
    [SerializeField] private bool CanRotate;
    
    [Header("Controls")]
    [SerializeField] private PlayerInput playerInput;

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

    [Header("Emitter")] 
    [SerializeField] private GameObject gravgunHover;
    [SerializeField] private GameObject gravgunShoot;

    [Header("Script")] 
    [SerializeField] private Laser2 laser2;

    [HideInInspector] public Rigidbody grabbedRB;
    private Transform grabbedTransform;
    private bool lineActivate;

    private float forwardMove;
    [HideInInspector] public bool IsEditing;
    
    private float rotateAxisX;
    private float rotateAxisY;
    private float rotateAxisZ;
    private float AxisFB;
    
    void Awake()
    {
        IsEditing = false;

        objectHolder.localPosition = holderOrigin;
        forwardMove = holderOrigin.z;
        playerInput = GetComponent<PlayerInput>();
        laser2 = GetComponent<Laser2>();
    }
    
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxGrabDistance, Color.green);
        
        if (grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position,
                Time.deltaTime * lerpSpeed));

            lineActivate = true;
            laser2.RenderLaser(lineActivate, grabbedRB);
        }
        else
        {
            lineActivate = false;
            laser2.RenderLaser(lineActivate, grabbedRB);
        }
        
        if (CanGravityGun)
        {
            if (CanRotate && IsEditing)
            {
                RotateGrabbed(rotateAxisX, rotateAxisY, rotateAxisZ);
            }


            if (CanMove)
            {
                MoveGrabbed(AxisFB);
            }
        }
        
        AnimationGun();
    }

    public void RotateInput1(InputAction.CallbackContext context)
    {
        rotateAxisX = context.ReadValue<Vector2>().x;
        rotateAxisY = context.ReadValue<Vector2>().y;
    }

    public void RotateInput2(InputAction.CallbackContext context)
    {
        rotateAxisZ = context.ReadValue<Vector2>().x;
    }

    public void AxisInput(InputAction.CallbackContext context)
    {
        AxisFB = context.ReadValue<float>();
    }

    public void Grab(InputAction.CallbackContext context)
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

            grabbedRB.isKinematic = false;
            IsEditing = false;
                
            forwardMove = holderOrigin.z;
            objectHolder.localPosition = new Vector3(0, 0, forwardMove);
                
            grabbedTransform = null;
            grabbedRB = null;
        }
    }
    
    private void RotateGrabbed(float axisX, float axisY, float axisZ)
    {
        //Debug.Log("x : " + axisX + "y : " + axisY + "z : " + axisZ);
        
        if (grabbedRB)
        {
            grabbedTransform.Rotate(axisX * xRotation * speedRotation * Time.deltaTime); 
            
            grabbedTransform.Rotate(axisY * yRotation * speedRotation * Time.deltaTime); 
            
            grabbedTransform.Rotate(axisZ * zRotation * speedRotation * Time.deltaTime);
        }
    }

    public void Editing(InputAction.CallbackContext context)
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

    public void ThrowGrabbed(InputAction.CallbackContext context)
    {
        if (CanThrow && grabbedRB)
        {
            if (context.performed)
            {
                //Debug.Log("Trow! " + context.phase);
                
                grabbedRB.isKinematic = false;
                grabbedRB.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);

                forwardMove = holderOrigin.z;
                objectHolder.localPosition = new Vector3(0, 0, forwardMove);
                
                IsEditing = false;
                
                grabbedTransform = null;
                grabbedRB = null;
            }
        }
    }

    private void AnimationGun()
    {
        if (grabbedRB)
        {
            gravityGunAnimator.SetTrigger("Use");
        }
        else if (!grabbedRB)
        {
            gravityGunAnimator.SetTrigger("No");
        }
    }

    public void SoundGun(InputAction.CallbackContext context)
    {
        if (grabbedRB)
        {
            gravgunHover.gameObject.SetActive(true);
        }
        else if (!grabbedRB)
        {
            gravgunHover.gameObject.SetActive(false);
        }

        if (context.performed)
        {
            gravgunShoot.gameObject.SetActive(true);
        }
        else if (!context.performed)
        {
            gravgunShoot.gameObject.SetActive(false);
        }
    }
}