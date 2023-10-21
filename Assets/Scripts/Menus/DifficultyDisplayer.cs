using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Toggle m_easyButton;
    [SerializeField]
    private Toggle m_mediumButton;
    [SerializeField]
    private Toggle m_hardButton;

    [SerializeField]
    private TextMeshProUGUI m_levelInfo;

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

    private ColorBlock m_pressedButtonColors;
    private ColorBlock m_unpressedButtonColors;
    private JsonDataService m_saver = new();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        if (m_disableOnLoad)
            gameObject.SetActive(false);
        m_unpressedButtonColors= m_easyButton.colors;
        m_pressedButtonColors= m_easyButton.colors;
        m_pressedButtonColors.normalColor = new Color(190f/255,190f/255, 190f/ 255);
        m_easyButton.colors = m_pressedButtonColors;

    }
    private void OnEnable()
    {
        m_settings.m_chosenDiff = Difficulty.Easy;
        EventSystem.current.SetSelectedGameObject(m_easyButton.gameObject);
        m_animator.SetBool("Show", true);
        m_levelName.text = m_settings.m_levelName;
        string path = Application.persistentDataPath + "/UnlockedLevels.json";
        int levelIndex = int.Parse(m_settings.m_levelName.Split(' ')[1]) + 2;
        if (File.Exists(path))
        {
            UnlockedLevels Unlocked = m_saver.LoadData<UnlockedLevels>("UnlockedLevels");
            if(Unlocked.m_easy.Contains(levelIndex))
            {
                m_easyButton.interactable = true;
            }
            if(Unlocked.m_medium.Contains(levelIndex))
            {
                m_mediumButton.interactable = true;
            }
            if(Unlocked.m_hard.Contains(levelIndex))
            {
                m_hardButton.interactable= true;
            }
        }
        UpdateLevelInfoText();
        StartCoroutine(PlaySounds());
    }
    private void UpdateLevelInfoText()
    {
        int levelIndex = int.Parse(m_settings.m_levelName.Split(' ')[1]) + 2;
        int totalSpawnAmount = SpawnManagerHolder.instance.manager.SpawnDictionary[levelIndex].GetTotalGooAmount();
        int goal = (int)(totalSpawnAmount * DifficultyManagerHolder.instance.manager.DifficultyDictionary[levelIndex].GetActiveMultiplier());
        m_levelInfo.text = $"Total that will spawn: {totalSpawnAmount}" +
            $"\r\nGoal: {goal}";
    }
    public void ChangeDifficulty(Toggle clickedButton)
    {
        if(clickedButton == m_easyButton)
        {
            m_mediumButton.isOn = false;
            m_mediumButton.colors = m_unpressedButtonColors;
            m_hardButton.isOn = false;
            m_hardButton.colors = m_unpressedButtonColors;

           
            m_easyButton.colors = m_pressedButtonColors;
            m_settings.m_chosenDiff = Difficulty.Easy;
        }
        else if(clickedButton == m_mediumButton)
        {
            m_easyButton.isOn = false;
            m_easyButton.colors = m_unpressedButtonColors;
            m_hardButton.isOn = false;
            m_hardButton.colors = m_unpressedButtonColors;

            m_mediumButton.colors = m_pressedButtonColors;
            m_settings.m_chosenDiff = Difficulty.Medium;
        }
        else if(clickedButton == m_hardButton)
        {
            m_easyButton.isOn = false;
            m_easyButton.colors = m_unpressedButtonColors;
            m_mediumButton.isOn = false;
            m_mediumButton.colors = m_unpressedButtonColors;

            m_hardButton.colors = m_pressedButtonColors;
            m_settings.m_chosenDiff = Difficulty.Hard;
        }
        UpdateLevelInfoText();
    }
    public void LoadLevel()
    {
        StartCoroutine(StartLevel());
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
    private IEnumerator StartLevel()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(0.667f);
        SceneChanger.instance.ChangeSceneWithName(m_settings.m_levelName);
    }
    private IEnumerator PlaySounds()
    {
        //this is as synchronised as I could make, this is hard af wtf
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
