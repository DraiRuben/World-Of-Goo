using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static Score Instance;
    public int m_score = 0;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        m_score = 0;
        DontDestroyOnLoad(gameObject);
    }
    public void SaveScore()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            string KeyName = SceneManager.GetActiveScene().name + " Score";
            if (PlayerPrefs.HasKey(KeyName))
            {
                if(PlayerPrefs.GetInt(KeyName)<m_score)
                {
                    PlayerPrefs.SetInt(KeyName, m_score);
                }
            }
            else
            {
                PlayerPrefs.SetInt(KeyName , m_score);
            }
        }
    }
}
