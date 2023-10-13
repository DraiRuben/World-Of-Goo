using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    [Serializable]
    struct ReplayButton
    {
        public Button m_button;
        public TextMeshProUGUI m_Text;
    }
    [SerializeField]
    private List<ReplayButton> buttonList;

    private void Start()
    {
        //Sets the text of all buttons for the level Replay
        int HighestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel");
        for (int i =1;i<=SceneManager.sceneCountInBuildSettings-2;i++)
        {
            buttonList[i - 1].m_Text.text = "Level " + i;
            if (HighestUnlockedLevel <= i-1)
            {
                buttonList[i - 1].m_button.interactable = false;
                buttonList[i - 1].m_Text.text += ": Locked";
            }
        }
    }
}
