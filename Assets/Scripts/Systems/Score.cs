using TMPro;
using UnityEngine;
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


    private int m_score = 0;
    public int m_Score { get { return m_score; } set { m_score = value; m_scoreDisplay.text = $"Score: {m_score}/{m_minScoreForWin}"; m_scoreDisplay.enabled = true; } }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }
    private void Start()
    {
        //this start is called after spawner's start
        m_minPercentForWin = m_difficulty.GetActiveMultiplier();
        m_minScoreForWin = (int)(m_totalSpawnedGoos * m_minPercentForWin);
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
        }
    }
}
