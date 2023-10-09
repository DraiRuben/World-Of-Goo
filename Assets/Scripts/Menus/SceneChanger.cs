using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
    public static void ChangeSceneWithIndex(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
    public void Exit()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        //Save current level to playerprefs
        if (SceneManager.GetActiveScene().name.Contains("Level")) 
            PlayerPrefs.SetInt("LastPlayedLevel",SceneManager.GetActiveScene().buildIndex);
    }
}
