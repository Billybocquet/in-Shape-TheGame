using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;
    
    [Header("Parameters")]
    [SerializeField] private float maxHighlightDistance;
    
    [Header("LayerMask")]
    [SerializeField] private LayerMask layerMask;

    [Header("Outline")] 
    [SerializeField] private int width;
    [SerializeField] private Color color;

    private GravityGun2 gravityGun2;
    
    private Transform highlight;
    private Transform selection;
    private RaycastHit hit;

    private void Start()
    {
        gravityGun2 = GetComponent<GravityGun2>();
    }

    void Update()
    {
        if (!gravityGun2.grabbedRB)
        {
            Highligth();
        }
    }

    private void Highligth()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, maxHighlightDistance, layerMask)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = hit.transform;
            if (highlight.CompareTag("Shape") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = color;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = width;
                }
            }
            else
            {
                highlight = null;
            }
        }
    }
}
