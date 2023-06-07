using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [Header("UI Object")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            textMeshProUGUI.text = (progress * 100).ToString() + " %";
            
            //Debug.Log(operation.progress);

            yield return null;
        }
    }
    
    public void QuitGame()
    {
        //Debug.Log("Quit!");
        
        Application.Quit();
    }
}
