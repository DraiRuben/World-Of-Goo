using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool Pause;

    public void OpenPauseMenu()
    {
        Pause = !Pause;
        Time.timeScale = Pause ? 0f : 1f;
        if (Pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        GetComponent<Animator>().SetBool("ShowMenu", Pause);
    }
    public void PauseResume(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OpenPauseMenu();
        }
    }
    public void PauseResumeNoCallback()
    {
        Pause = !Pause;
        Time.timeScale = Pause ? 0f : 1f;
        if (Pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        GetComponent<Animator>().SetBool("ShowMenu", Pause);

    }
    private IEnumerator DelayedDeactivation()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);

    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneChanger.instance.ChangeSceneWithName(SceneManager.GetActiveScene().name);
    }
}
