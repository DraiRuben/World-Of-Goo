using System.Collections;
using System.IO;
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

    private JsonDataService m_saver = new();
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
    //don't know how to do one function that can take either an int or a string, don't care enough to lose time on this
    public void ChangeSceneWithIndex(int sceneIndex)
    {
        StartCoroutine(LoadLevelAsyncInt((sceneIndex)));
    }
    public void ChangeSceneWithName(string sceneName)
    {
        StartCoroutine(LoadLevelAsyncString((sceneName)));
    }
    //resets everything saved except the main volume, TODO: prompt asking the player if he wants to reset
    public void NewGame(string sceneName)
    {
        StartCoroutine(LoadLevelAsyncString((sceneName)));
        string path = Application.persistentDataPath + "/Level ";

        //deletes every score & unlocked levels from memory
        for (int i = 0; i < SceneManager.sceneCount - 2; i++)
        {
            if (!File.Exists(path + i+".json"))
                File.Delete(path + i);
        }
        path = Application.persistentDataPath + "/UnlockedLevels.json";
        if (!File.Exists(path))
            File.Delete(path);

        UnlockedLevels unlocked = new();
        unlocked.m_easy.Add(2);
        m_saver.SaveData("UnlockedLevels", unlocked);

    }

    private IEnumerator LoadLevelAsyncInt(int sceneIndex)
    {
        ReverseCurtainState();
        SaveLastPlayedLevel();
        m_audioSource.clip = m_Close;
        if (!SceneManager.GetActiveScene().name.Contains("Level"))
            m_audioSource.Play();
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1.3f);
        SceneManager.LoadSceneAsync(sceneIndex);

    }
    private IEnumerator LoadLevelAsyncString(string sceneName)
    {
        ReverseCurtainState();
        SaveLastPlayedLevel();
        m_audioSource.clip = m_Close;
        Time.timeScale = 1f;
        if (!SceneManager.GetActiveScene().name.Contains("Level"))
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
