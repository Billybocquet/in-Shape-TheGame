using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInputActions playerInputActions;

    [Header("Layer Mask")] 
    [SerializeField] private LayerMask layerMask;

    [Header("Range")]
    [SerializeField] private float interactRange;
    [SerializeField] private float interactRangeUI;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        
        playerInputActions.Player.Interact.performed += Interaction;
    }


    private void Interaction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, layerMask);
            foreach (Collider collider in colliderArray)
            {
                //Debug.Log(collider);

                if (collider.TryGetComponent(out ShapeSpawner shapeSpawner))
                {
                    shapeSpawner.Spawn();
                }
            }
        }
    }

    public ShapeSpawner GetSpawnerInteractable()
    {
        List<ShapeSpawner> shapeSpawnersList = new List<ShapeSpawner>();
        
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRangeUI, layerMask);
        foreach (Collider collider in colliderArray)
        {
            //Debug.Log(collider);

            if (collider.TryGetComponent(out ShapeSpawner shapeSpawner))
            {
                shapeSpawnersList.Add(shapeSpawner);
            }
        }

        ShapeSpawner closestShapeSpawner = null;
        foreach (ShapeSpawner shapeSpawner in shapeSpawnersList)
        {
            if (closestShapeSpawner == null)
                closestShapeSpawner = shapeSpawner;
            else
            {
                if (Vector3.Distance(transform.position, shapeSpawner.transform.position) <
                    Vector3.Distance(transform.position, closestShapeSpawner.transform.position))
                {
                    closestShapeSpawner = shapeSpawner;
                }
            }
        }
        
        return closestShapeSpawner;
    }
}
