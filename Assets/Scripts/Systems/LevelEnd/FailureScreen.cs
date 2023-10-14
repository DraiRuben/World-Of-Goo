using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailureScreen : MonoBehaviour
{
    public static FailureScreen instance;
    private Animator m_animator;
    [SerializeField]
    private TextMeshProUGUI m_retryMessage;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
    }

    public void ShowFailScreen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        m_animator.SetBool("Open",true);
        m_retryMessage.text = $"You needed to save {Score.Instance.m_minScoreForWin - Score.Instance.m_Score} more Goos to complete this level.\r\nRetry ?";
    }
    public void Retry()
    {
        StartCoroutine(RetryLevel());
    }
    private IEnumerator RetryLevel()
    {
        m_animator.SetBool("Open", false);
        yield return new WaitForSecondsRealtime(1.5f);
        SceneChanger.instance.ReverseCurtainState();
        Time.timeScale = 1.0f;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
    public void MainMenu()
    {
        StartCoroutine(GoToMainMenu());
    }
    private IEnumerator GoToMainMenu()
    {
        m_animator.SetBool("Open", false);
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1.0f;
        SceneChanger.instance.ReverseCurtainState();
        SceneManager.LoadSceneAsync("MainMenu");
    }

}
