using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsEndLevel : MonoBehaviour
{
    public static StatsEndLevel instance;
    private Animator m_animator;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        m_animator.SetBool("Show", true);
        GetComponent<LevelStatsDisplayer>().SetLineText(
            Score.Instance.m_minScoreForWin, 
            Score.Instance.m_Score, 
            Score.Instance.m_totalSpawnedGoos, 
            Goo.s_moves, 
            Score.Instance.m_timeSinceLevelStart);
    }
    public void Next()
    {
        StartCoroutine(GoToNext());
    }
    public void Retry()
    {
        StartCoroutine(RetryLevel());
    }
    public void MainMenu()
    {
        StartCoroutine(GoToMainMenu());
    }
    private IEnumerator GoToMainMenu()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(1.2f);
        SceneManager.LoadSceneAsync("MainMenu");
    }
    private IEnumerator RetryLevel()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(1.2f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator GoToNext()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(1.2f);
        gameObject.SetActive(false);
        DifficultyDisplayer.instance.gameObject.SetActive(true);

    }

}
