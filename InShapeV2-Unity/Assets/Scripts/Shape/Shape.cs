using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [Header("Interact")] 
    [SerializeField] private string interactText;

    [HideInInspector] public bool onTower;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Destroy")
           Destroy(gameObject);

        if (collision.gameObject.tag == "Shape")
        {
            onTower = true;
        }
        else
        {
            onTower = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        onTower = false;
    }

    public string GetInteractText()
    {
        return interactText;
    }
}
