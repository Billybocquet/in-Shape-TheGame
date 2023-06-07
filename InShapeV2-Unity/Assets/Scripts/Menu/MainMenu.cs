using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(int sceneIndex)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneIndex);
    }

    public void QuitGame()
    {
        //Debug.Log("Quit!");
        
        Application.Quit();
    }
}
