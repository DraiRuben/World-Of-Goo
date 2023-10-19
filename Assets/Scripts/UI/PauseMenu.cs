using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool m_pause;

    public void OpenPauseMenu()
    {
        m_pause = !m_pause;
        Time.timeScale = m_pause ? 0f : 1f;
        if (m_pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        GetComponent<Animator>().SetBool("ShowMenu", m_pause);
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
        m_pause = !m_pause;
        Time.timeScale = m_pause ? 0f : 1f;
        if (m_pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        GetComponent<Animator>().SetBool("ShowMenu", m_pause);

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
