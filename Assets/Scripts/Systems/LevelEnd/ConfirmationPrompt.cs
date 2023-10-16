using System.Collections;
using TMPro;
using UnityEngine;

public class ConfirmationPrompt : MonoBehaviour
{
    private Animator m_animator;
    //always avoiding direct references if possible
    public static ConfirmationPrompt instance;

    [SerializeField]
    private TextMeshProUGUI m_promptMessage;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
    }

    public void OpenPrompt()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        m_promptMessage.text = $"You still need to save {Score.Instance.m_minScoreForWin - Score.Instance.m_Score} Goos if you want to complete this level.\r\nAre you sure ?";
        Time.timeScale = 0f;
        m_animator.SetBool("Open", true);
    }
    private void ClosePrompt()
    {
        m_animator.SetBool("Open", false);
    }
    public void Yes()
    {
        StartCoroutine(Fail());
    }
    private IEnumerator Fail()
    {
        ClosePrompt();
        yield return new WaitForSecondsRealtime(1f);
        SceneChanger.instance.ReverseCurtainState();
        yield return new WaitForSecondsRealtime(1.3f);
        FailureScreen.instance.ShowFailScreen();

    }
    public void No()
    {
        Time.timeScale = 1.0f;
        ClosePrompt();
    }
}
