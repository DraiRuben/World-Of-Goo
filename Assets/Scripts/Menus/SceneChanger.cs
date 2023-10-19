using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;

    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_Open;
    [SerializeField]
    private AudioClip m_Close;

    public static SceneChanger instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_Open;
    }
    private void Start()
    {
        m_audioSource.Play();
    }
    public void ChangeSceneWithIndex(int sceneIndex)
    {
        StartCoroutine(LoadLevelAsyncInt((sceneIndex)));
    }
    public void NewGame(string sceneName)
    {
        StartCoroutine(LoadLevelAsyncString((sceneName)));
        float volume = 1f;
        if (PlayerPrefs.HasKey("MainVolume"))
        {
            volume = PlayerPrefs.GetFloat("MainVolume");
        }
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("MainVolume", volume);

    }
    public void ChangeSceneWithName(string sceneName)
    {
        StartCoroutine(LoadLevelAsyncString((sceneName)));
    }
    private IEnumerator LoadLevelAsyncInt(int sceneIndex)
    {
        ReverseCurtainState();
        SaveLastPlayedLevel();
        m_audioSource.clip = m_Close;
        m_audioSource.Play();
        yield return new WaitForSecondsRealtime(1.3f);
        SceneManager.LoadSceneAsync(sceneIndex);

    }
    private IEnumerator LoadLevelAsyncString(string sceneName)
    {
        ReverseCurtainState();
        SaveLastPlayedLevel();
        m_audioSource.clip = m_Close;
        if(!SceneManager.GetActiveScene().name.Contains("Level"))
            m_audioSource.Play();

        yield return new WaitForSecondsRealtime(1.3f);
        SceneManager.LoadSceneAsync(sceneName);
    }
    public void ReverseCurtainState()
    {
        gameObject.SetActive(true);
        m_Animator.SetTrigger("Start");
        if (!SceneManager.GetActiveScene().name.Contains("Level"))
            m_audioSource.Play();

    }
    public void Exit()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        SaveLastPlayedLevel();
    }
    private void SaveLastPlayedLevel()
    {
        //Save current level to playerprefs
        if (SceneManager.GetActiveScene().name.Contains("Level"))
            PlayerPrefs.SetInt("LastPlayedLevel", SceneManager.GetActiveScene().buildIndex);
    }
}
