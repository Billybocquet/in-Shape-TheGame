using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfWin : MonoBehaviour
{
    [Header("Checking Time")] 
    [SerializeField, Range(0, 10)] private float checkingTime; 
    
    private Shape shape;
    private float t;

    private void Start()
    {
        t = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Shape")
        {
            shape = other.gameObject.GetComponent<Shape>();

            t += Time.deltaTime; 
            
            if (shape.onTower && t >= checkingTime)
            {
                Debug.Log("Tower reach " + gameObject.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        t = 0;
    }
}
