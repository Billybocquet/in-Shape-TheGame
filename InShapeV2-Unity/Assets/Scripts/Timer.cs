using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField, Range(0, 120)] private int timer;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private float floatTime;
    private int intTime;

    private void Start()
    {
        floatTime = 0;
        intTime = 0;
    }

    private void Update()
    {
        floatTime += Time.deltaTime;

        if (intTime <= timer)
        {
            Debug.Log("La partie est fini");
        }
    }
}
