using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
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

    [SerializeField]
    private AudioClip m_woodLow;
    [SerializeField]
    private AudioClip m_woodMedium;
    [SerializeField]
    private AudioClip m_woodHigh;

    private AudioSource m_audioSource;
    private Animator m_animator;
    [SerializeField]
    private bool m_disableOnLoad = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        if (m_disableOnLoad)
            gameObject.SetActive(false);


    }
    private void OnEnable()
    {
        m_animator.SetBool("Show", true);
        m_levelName.text = m_settings.m_levelName;
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
        StartCoroutine(PlaySounds());
    }

    public void LoadLevel(EnumWorkaround diff)
    {
        m_settings.m_chosenDiff = diff.Difficulty;
        StartCoroutine(ChooseLevel());
    }

    public void Back()
    {
        StartCoroutine(GoBack());
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
    private IEnumerator GoBack()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(0.667f);
        gameObject.SetActive(false);

    }
    private IEnumerator ChooseLevel()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(0.667f);
        SceneChanger.instance.ChangeSceneWithName(m_settings.m_levelName);
    }
    private IEnumerator PlaySounds()
    {
        yield return new WaitForSecondsRealtime(1.20f);
        m_audioSource.clip = m_woodHigh;
        m_audioSource.Play();
        yield return new WaitForSecondsRealtime(0.35f);
        m_audioSource.clip = m_woodMedium;
        m_audioSource.Play();
        yield return new WaitForSecondsRealtime(0.35f);
        m_audioSource.clip = m_woodLow;
        m_audioSource.Play();
    }
}
