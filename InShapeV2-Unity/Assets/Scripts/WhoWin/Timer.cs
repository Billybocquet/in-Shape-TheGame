using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField, Range(0, 480)] private int timer;

    [Header("UI Object")]
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Slider slider;

    [Header("Score")] 
    [SerializeField] private string scoreText;

    private Score score;

    [HideInInspector] public float t;
    
    private void Start()
    {
        t = timer;

    }

    private void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
        }
        else
        {
            t = 0;
        }
        
        DisplayTime(t);

        if (t <= 0)
        {
            Debug.Log("La partie est fini");

            score = GetComponent<Score>();
            
            score.WinnerIs(scoreText);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        float pourcent = t / timer;
        
        slider.value = pourcent;
        textMeshProUGUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
