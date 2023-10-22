using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private JsonDataService m_saver = new();

    private void Start()
    {
        //Sets the text of all buttons for the level Replay
        string path = Application.persistentDataPath + "/UnlockedLevels.json";
        int HighestUnlockedLevel = 2;
        if (File.Exists(path))
        {
            HighestUnlockedLevel = m_saver.LoadData<UnlockedLevels>("UnlockedLevels").m_easy.Last();
        }
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 2; i++)
        {
            if (i+2>HighestUnlockedLevel)
            {
                buttonList[i].m_button.interactable = false;

                buttonList[i].m_Text.text = buttonList[i].m_difficultySettings.m_levelName + " Locked";
            }
            else
            {
                buttonList[i].m_Text.text = buttonList[i].m_difficultySettings.m_levelName;
                int iBis = i;
                buttonList[i].m_button.onClick.AddListener(()
                    => OpenDifficultyOptions(buttonList[iBis].m_difficultySettings));
            }
        }
    }
    public void OpenDifficultyOptions(DifficultySettings settings)
    {
        if (!m_difficultyDisplayer.gameObject.activeSelf)
        {
            m_difficultyDisplayer.m_settings = settings;
            m_difficultyDisplayer.gameObject.SetActive(true);
        }


    }
}
