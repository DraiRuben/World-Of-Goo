using System.Collections;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator m_animator;
    public bool m_isInWaitScreen = false;
    public static NextLevel instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
    }
    void Start()
    {
        Score.Instance.scoreChanged.AddListener(Appear);
    }

    public void Appear()
    {
        m_animator.SetBool("Appear", true);
    }
    public void TryDisappear()
    {
        if (Score.Instance.CanGoToNextLevel())
        {
            StartCoroutine(GoToNextLevel());
            Time.timeScale = 0f;
            m_isInWaitScreen = true;

        }
        else
        {
            //may call this function multiple times during prompt if the player re clicks on the button, though this shouldn't cause any issues
            ConfirmationPrompt.instance.OpenPrompt();
        }
    }
    private IEnumerator GoToNextLevel()
    {
        m_animator.SetBool("Appear", false);
        yield return new WaitForSecondsRealtime(0.40f);
        SceneChanger.instance.ReverseCurtainState();
        yield return new WaitForSecondsRealtime(1.3f);
        Score.Instance.SaveScore();
        StatsEndLevel.instance.gameObject.SetActive(true);

    }
}
