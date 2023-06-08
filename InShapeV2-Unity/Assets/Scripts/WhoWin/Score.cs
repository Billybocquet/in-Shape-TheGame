using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private GameObject score;
    [SerializeField] private TextMeshProUGUI scoreTextMeshProUGUI;

    private PauseMenu pauseMenu;

    public void Start()
    {
        pauseMenu = GetComponent<PauseMenu>();
    }

    public void WinnerIs(string winnerText)
    {
        pauseMenu.Pause(score);
        
        score.SetActive(true);
        scoreTextMeshProUGUI.text = winnerText;

    }
}
