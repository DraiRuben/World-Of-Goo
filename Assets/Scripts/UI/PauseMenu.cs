using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool Pause;
    public void PauseResume()
    {
        Pause = !Pause;
        Time.timeScale = Pause?0f:1f;
        gameObject.SetActive(Pause);
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
