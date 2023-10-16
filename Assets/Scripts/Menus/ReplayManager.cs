using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    [SerializeField]
    private DifficultyDisplayer m_difficultyDisplayer;
    [Serializable]
    struct ReplayButton
    {
        public Button m_button;
        public TextMeshProUGUI m_Text;
        public DifficultySettings m_difficultySettings;
    }
    [SerializeField]
    private List<ReplayButton> buttonList;

    private void Start()
    {
        //Sets the text of all buttons for the level Replay
        int HighestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel");
        for (int i = 1; i <= SceneManager.sceneCountInBuildSettings - 2; i++)
        {
            buttonList[i - 1].m_Text.text = "Level " + i;
            if (HighestUnlockedLevel <= i - 1)
            {
                buttonList[i - 1].m_button.interactable = false;
                buttonList[i - 1].m_Text.text += ": Locked";
            }
            else
            {
                //for some reason it give the ref to i as an index, instead of the value, so it fucks everything and I need to do a local copy of it, that somehow works
                int temp = i - 1;
                buttonList[i - 1].m_button.onClick.AddListener(()
                    => OpenDifficultyOptions(buttonList[temp].m_difficultySettings));
            }
        }
    }
    private void OpenDifficultyOptions(DifficultySettings settings)
    {
        m_difficultyDisplayer.gameObject.SetActive(true);

        m_difficultyDisplayer.m_settings = settings;
    }
}
