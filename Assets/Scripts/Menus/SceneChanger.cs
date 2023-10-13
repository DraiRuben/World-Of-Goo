using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;
    public static SceneChanger instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void ChangeSceneWithIndex(int sceneIndex)
    {
        StartCoroutine(LoadLevelAsyncInt((sceneIndex)));
    }
    public void ChangeSceneWithName(string sceneName)
    {
        StartCoroutine(LoadLevelAsyncString((sceneName)));
    }
    private IEnumerator LoadLevelAsyncInt(int sceneIndex)
    {
        gameObject.SetActive(true);
        yield return null;
        m_Animator.SetTrigger("Start");
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadSceneAsync(sceneIndex);

    }
    private IEnumerator LoadLevelAsyncString(string sceneName)
    {
        gameObject.SetActive(true);
        yield return null;
        m_Animator.SetTrigger("Start");
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadSceneAsync(sceneName);
    }
    public void Exit()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        //Save current level to playerprefs
        if (SceneManager.GetActiveScene().name.Contains("Level"))
            PlayerPrefs.SetInt("LastPlayedLevel", SceneManager.GetActiveScene().buildIndex);
    }
}
