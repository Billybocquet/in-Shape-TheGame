using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Controls")] 
    [SerializeField] private PlayerInput playerInput;

    [Header("UI Object")] 
    [SerializeField] private GameObject pauseMenuUI;
    
    private static bool GameIsPaused;

    public void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void SetPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameIsPaused)
            {
                Resume(pauseMenuUI);
            }
            else
            {
                Pause(pauseMenuUI);
            }
        }
    }
    public void Resume(GameObject uiGameObject)
    {
        uiGameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
        
    public void Pause(GameObject uiGameObject)
    {
        uiGameObject.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu(int sceneIndex)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneIndex);
    }

    public void QuitGame()
    {
        //Debug.Log("Quit!");
        
        Application.Quit();
    }
}
