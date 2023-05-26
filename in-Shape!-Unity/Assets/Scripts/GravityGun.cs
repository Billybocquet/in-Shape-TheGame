using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    public bool CanGravityGun { get; private set; } = true;

    [Header("Fonctionnal Parameters")] 
    [SerializeField] private bool CanThrow;
    [SerializeField] private bool CanMove;
    [SerializeField] private bool CanRotate;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    
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

    [Header("Controls")]
    [SerializeField] private KeyCode leftClick;
    [SerializeField] private KeyCode rightClick;
    [SerializeField] private KeyCode grabButton;
    [SerializeField] private KeyCode throwButton;
    [SerializeField] private KeyCode xRotateButton;
    [SerializeField] private KeyCode yRotateButton;
    [SerializeField] private KeyCode zRotateButton;
    [SerializeField] private KeyCode forwardButton;

    [Header("Holder")]
    [SerializeField] private Transform objectHolder;
    [SerializeField] private Vector3 holderOrigin;


    private Rigidbody grabbedRB;
    private Transform grabbedTransform;
    public bool isEdit;

    private float forwardMove;

    // Start is called before the first frame update
    void Start()
    {
        objectHolder.localPosition = holderOrigin;
        forwardMove = holderOrigin.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanGravityGun)
        {
            /*Grab();*/
            
            /*if (CanRotate)
                RotateGrabbed();*/
            
            if(CanMove)
                MoveGrabbed();
            
            /*if(CanThrow)
                ThrowGrabbed();*/

        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (grabbedRB)
        {
            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * lerpSpeed));
        }
        
        if (CanGravityGun)
        {
            if (context.performed)
            {
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

                if (Physics.Raycast(ray, out hit, maxGrabDistance))
                {
                    grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                    grabbedTransform = hit.collider.gameObject.GetComponent<Transform>();
                    
                    if (grabbedRB)
                    {
                        grabbedRB.isKinematic = true;
                    }
                }
            }
            
            if (context.canceled && grabbedRB)
            {
                grabbedRB.isKinematic = false;
                
                objectHolder.localPosition = holderOrigin;
                
                grabbedTransform = null;
                grabbedRB = null;
            }
        }
    }
    
    public void RotateGrabbed(InputAction.CallbackContext context)
    {
        float rotateAmountX = context.ReadValue<Vector2>().x;
        float rotateAmountY = context.ReadValue<Vector2>().y;
        
        if (grabbedRB && isEdit)
        {
            grabbedTransform.Rotate(rotateAmountX * xRotation * speedRotation * Time.deltaTime); 
            grabbedTransform.Rotate(rotateAmountY * yRotation * speedRotation * Time.deltaTime); 
            
            /*if (Input.GetKey(xRotateButton))
            {
                if (Input.GetKey(leftClick))
                    grabbedTransform.Rotate(xRotation * speedRotation * Time.deltaTime); 
                
                if (Input.GetKey(rightClick))
                    grabbedTransform.Rotate(-xRotation * speedRotation * Time.deltaTime);
            }
            
            if (Input.GetKey(yRotateButton))
            {
                if (Input.GetKey(leftClick))
                    grabbedTransform.Rotate(yRotation * speedRotation * Time.deltaTime); 
                
                if (Input.GetKey(rightClick))
                    grabbedTransform.Rotate(-yRotation * speedRotation * Time.deltaTime); 
            }
            
            if (Input.GetKey(zRotateButton))
            {
                if (Input.GetKey(leftClick))
                    grabbedTransform.Rotate(zRotation * speedRotation * Time.deltaTime); 
                
                if (Input.GetKey(rightClick))
                    grabbedTransform.Rotate(-zRotation * speedRotation * Time.deltaTime);
            }*/
        }
    }

    private void MoveGrabbed()
    {
        if (grabbedRB & Input.GetKey(forwardButton))
        {
            if (Input.GetKey(leftClick))
            {
                forwardMove += speedForward * Time.deltaTime;
                forwardMove = Mathf.Clamp(forwardMove, minForward, maxForward);

                objectHolder.localPosition = new Vector3(0, 0, forwardMove);
            }

            if (Input.GetKey(rightClick))
            {
                forwardMove -= speedForward * Time.deltaTime;
                forwardMove = Mathf.Clamp(forwardMove, minForward, maxForward);
                
                objectHolder.localPosition = new Vector3(0, 0, forwardMove);
            }
        }
    }

    public void ThrowGrabbed(InputAction.CallbackContext context)
    {
        if (grabbedRB && CanThrow)
        {
            if (context.performed)
            {
                grabbedRB.isKinematic = false;
                grabbedRB.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);
                
                objectHolder.localPosition = holderOrigin;
                
                grabbedTransform = null;
                grabbedRB = null;
            }
        }
    }

    public void Editing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isEdit = !isEdit;
        }
    }
}
