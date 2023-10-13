using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyDisplayer : MonoBehaviour
{
    [SerializeField]
    private DifficultySettings settings;

    private void OnEnable()
    {
        
    }

    void Start()
    {
        
    }
    public void LoadLevel(Difficulty diff)
    {
        settings.m_chosenDiff = diff;
        SceneManager.LoadSceneAsync(settings.m_levelName);
    }
    public void Back()
    {
        gameObject.SetActive(false);
    }
}
