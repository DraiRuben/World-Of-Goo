using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool m_pause;
    private Animator m_animator;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    public void OpenPauseMenu()
    {
        m_pause = !m_pause;
        Time.timeScale = m_pause ? 0f : 1f;
        if (m_pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        m_animator.SetBool("ShowMenu", m_pause);
    }
    //for ESC press
    public void PauseResume(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OpenPauseMenu();
        }
    }
    //used by on screen button in the UI
    public void PauseResumeNoCallback()
    {
        m_pause = !m_pause;
        Time.timeScale = m_pause ? 0f : 1f;
        if (m_pause)
            gameObject.SetActive(true);
        else
            StartCoroutine(DelayedDeactivation());

        m_animator.SetBool("ShowMenu", m_pause);

    }
    //so that it can play its animation
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
