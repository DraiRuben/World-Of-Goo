using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
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
    private float m_timeSinceLevelStart = 0f;

    private JsonDataService m_saver = new();
    public int m_Score
    {
        get
        {
            return m_score;
        }
        set
        {
            m_score = value;
            m_scoreDisplay.text = $"Score: {m_score}/{m_minScoreForWin}";
            if (m_score > 0) scoreChanged.Invoke();
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        scoreChanged = new UnityEvent();

    }
    private void Start()
    {
        //this start is called after spawner's & cannon start
        m_minPercentForWin = m_difficulty.GetActiveMultiplier();
        m_minScoreForWin = (int)(m_totalSpawnedGoos * m_minPercentForWin);
        m_Score = 0;
    }
    private void Update()
    {
        m_timeSinceLevelStart += Time.deltaTime;
    }
    public bool CanGoToNextLevel()
    {
        return m_score >= m_minScoreForWin;
    }

    public void SaveScore()
    {
        string levelName = SceneManager.GetActiveScene().name;
        if (levelName.Contains("Level"))
        {
            string path = Application.persistentDataPath + $"/{levelName}.json";
            LevelData data = new LevelData(levelName, m_minScoreForWin,m_totalSpawnedGoos, m_score, Goo.s_moves, m_timeSinceLevelStart);
            if (!File.Exists(path))
            {
                LevelStats stats = new LevelStats();
                stats.GetStats(m_difficulty.m_chosenDiff).Add(data);
                m_saver.SaveData(levelName, stats);
            }
            else
            {
                //avoids overwriting previous scores
                LevelStats stats = m_saver.LoadData<LevelStats>(levelName);
                stats.GetStats(m_difficulty.m_chosenDiff).Add(data);
                m_saver.SaveData(levelName, stats);
            }
            //unlocks next difficulty & level
            path = Application.persistentDataPath + "/UnlockedLevels.json";
            if (!File.Exists(path))
            {
                UnlockedLevels unlockedLevels = new UnlockedLevels();
                unlockedLevels.m_easy.Add(SceneManager.GetActiveScene().buildIndex);
                unlockedLevels.m_easy.Add(SceneManager.GetActiveScene().buildIndex +1);
                if (unlockedLevels.GetNextDifficulty(m_difficulty.m_chosenDiff) != null)
                    unlockedLevels.GetNextDifficulty(m_difficulty.m_chosenDiff).Add(SceneManager.GetActiveScene().buildIndex);

                m_saver.SaveData("UnlockedLevels",unlockedLevels);
            }
            else
            {
                UnlockedLevels unlockedLevels = m_saver.LoadData<UnlockedLevels>("UnlockedLevels");
                if (!unlockedLevels.m_easy.Contains(SceneManager.GetActiveScene().buildIndex + 1))
                {
                    unlockedLevels.m_easy.Add(SceneManager.GetActiveScene().buildIndex + 1);
                }
                if (unlockedLevels.GetNextDifficulty(m_difficulty.m_chosenDiff)!= null 
                    && !unlockedLevels.GetNextDifficulty(m_difficulty.m_chosenDiff).Contains(SceneManager.GetActiveScene().buildIndex))
                {
                    unlockedLevels.GetNextDifficulty(m_difficulty.m_chosenDiff).Add(SceneManager.GetActiveScene().buildIndex);
                }
                m_saver.SaveData("UnlockedLevels", unlockedLevels);
            }

        }
    }
}

public class LevelData
{
    public string m_levelName;
    public int m_minToComplete;
    public int m_totalSpawnedGoos;
    public int m_score;
    public int m_moves;
    public float m_timeToComplete;
    public LevelData(string levelName, int minToComplete,int totalSpawnedGoos ,int score, int moves, float timeToComplete)
        => (m_levelName, m_minToComplete,m_totalSpawnedGoos, m_score, m_moves, m_timeToComplete) = (levelName, minToComplete,totalSpawnedGoos, score, moves, timeToComplete);
}
public class LevelStats
{
    public List<LevelData> m_EasyStats = new();
    public List<LevelData> m_MediumStats = new();
    public List<LevelData> m_HardStats = new();

    public List<LevelData> GetStats(Difficulty levelDifficulty)
    {
        switch (levelDifficulty)
        {
            case (Difficulty.Easy):
                return m_EasyStats;
            case (Difficulty.Medium):
                return m_MediumStats;
            case (Difficulty.Hard):
                return m_HardStats;
            default: return null;
        }
    }
}

public class UnlockedLevels
{
    public List<int> m_easy = new();
    public List<int> m_medium = new();
    public List<int> m_hard = new();

    public List<int> GetNextDifficulty(Difficulty currentDiff)
    {
        switch (currentDiff)
        {
            case (Difficulty.Easy):
                return m_medium;
            case (Difficulty.Medium):
                return m_hard;
            default: return null;
        }
    }
}