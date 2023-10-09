using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    
    void Start()
    {
        if (PlayerPrefs.GetInt("LastPlayedLevel") == 0)
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(
                ()=>SceneChanger.ChangeSceneWithIndex(PlayerPrefs.GetInt("LastPlayedLevel")));
        }
    }
    
}
