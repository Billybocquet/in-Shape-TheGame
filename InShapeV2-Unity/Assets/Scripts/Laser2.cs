using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser2 : MonoBehaviour
{
    [Header("Transform")] 
    [SerializeField] private Transform startPoint;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRend;

    public void RenderLaser(bool activate, Rigidbody grabbed)
    {
        lineRend.enabled = activate;
        if (grabbed != null)
        {
            lineRend.SetPosition(0, startPoint.position);
            lineRend.SetPosition(1, grabbed.position);
        }
    }
}
