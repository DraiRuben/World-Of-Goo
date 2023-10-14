using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static Score Instance;
    [SerializeField]
    private TextMeshProUGUI m_scoreDisplay;
    [SerializeField]
    private DifficultySettings m_difficulty;

    private float m_minPercentForWin;

    [HideInInspector]
    public int m_totalSpawnedGoos = 0;
    [HideInInspector]
    public int m_minScoreForWin = 0;

    public UnityEvent scoreChanged;
    private int m_score = 0;
    public int m_Score { get { return m_score; } set { m_score = value; m_scoreDisplay.text = $"Score: {m_score}/{m_minScoreForWin}"; if(m_score>0)scoreChanged.Invoke(); } }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        scoreChanged= new UnityEvent();

    }
    private void Start()
    {
        //this start is called after spawner's start
        m_minPercentForWin = m_difficulty.GetActiveMultiplier();
        m_minScoreForWin = (int)(m_totalSpawnedGoos * m_minPercentForWin);
        m_Score = 0;
    }
    public bool CanGoToNextLevel()
    {
        return m_score>= m_minScoreForWin;
    }
    public void SaveScore()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            string KeyName = SceneManager.GetActiveScene().name + "_" + m_difficulty.m_chosenDiff.ToString() + "_Score";
            if (PlayerPrefs.HasKey(KeyName))
            {
                if (PlayerPrefs.GetInt(KeyName) < m_Score)
                {
                    PlayerPrefs.SetInt(KeyName, m_Score);
                }
            }
            else
            {
                PlayerPrefs.SetInt(KeyName, m_Score);
            }
            //unlocks next difficulty
            if(m_difficulty.m_chosenDiff == Difficulty.Easy && !PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_Medium_Score"))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Medium_Score", 0);
            else if (m_difficulty.m_chosenDiff == Difficulty.Medium && !PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_Hard_Score"))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Hard_Score", 0);

        }
    }
    public void UnlockNextLevel()
    {
        if(!PlayerPrefs.HasKey(DifficultyDisplayer.instance.m_settings.m_levelName + "_Easy_Score"))
        {
            PlayerPrefs.SetInt(DifficultyDisplayer.instance.m_settings.m_levelName + "_Easy_Score", 0);
            if (PlayerPrefs.GetInt("HighestUnlockedLevel") < SceneManager.GetActiveScene().buildIndex+1)
                PlayerPrefs.SetInt("HighestUnlockedLevel", SceneManager.GetActiveScene().buildIndex+1);
        }

        
    }
}
