using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyDisplayer : MonoBehaviour
{
    public static DifficultyDisplayer instance;
    public DifficultySettings m_settings;
    [SerializeField]
    private TextMeshProUGUI m_levelName;
    [SerializeField]
    private TextMeshProUGUI m_easy;
    [SerializeField]
    private TextMeshProUGUI m_medium;
    [SerializeField]
    private TextMeshProUGUI m_hard;

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
        m_levelName.text = m_settings.m_levelName + " Difficulty";
        string keyname = m_settings.m_levelName + "_Easy_Score";
        if (PlayerPrefs.HasKey(keyname))
        {
            m_easy.text = "Score:" + PlayerPrefs.GetInt(keyname).ToString();
        }
        else
        {
            m_easy.text = "Locked";
            m_easy.transform.parent.GetComponent<Button>().interactable = false;
        }
        keyname = m_settings.m_levelName + "_Medium_Score";
        if (PlayerPrefs.HasKey(keyname))
        {
            m_medium.text = "Score:" + PlayerPrefs.GetInt(keyname).ToString();
        }
        else
        {
            m_medium.text = "Locked";
            m_medium.transform.parent.GetComponent<Button>().interactable = false;
        }
        keyname = m_settings.m_levelName + "_Hard_Score";
        if (PlayerPrefs.HasKey(keyname))
        {
            m_hard.text = "Score:" + PlayerPrefs.GetInt(keyname).ToString();
        }
        else
        {
            m_hard.text = "Locked";
            m_hard.transform.parent.GetComponent<Button>().interactable = false;
        }
    }

    public void LoadLevel(EnumWorkaround diff)
    {
        m_settings.m_chosenDiff = diff.Difficulty;
        StartCoroutine(ChooseLevel());
    }
    private IEnumerator ChooseLevel()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(0.40f);
        SceneChanger.instance.ChangeSceneWithName(m_settings.m_levelName);
    }
    public void Back()
    {
        gameObject.SetActive(false);
    }
    public void MainMenu()
    {
        StartCoroutine(GoToMainMenu());
    }
    private IEnumerator GoToMainMenu()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(2.27f);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
