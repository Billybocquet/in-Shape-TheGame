using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [Header("Interact")] 
    [SerializeField] private string interactText;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Destroy")
           Destroy(gameObject);
    }
    
    public string GetInteractText()
    {
        return interactText;
    }
}
