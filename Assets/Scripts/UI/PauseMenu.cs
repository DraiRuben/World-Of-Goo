using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool Pause;
    public void PauseResume()
    {
        Pause = !Pause;
        Time.timeScale = Pause?0f:1f;
        gameObject.SetActive(Pause);
    }
}
