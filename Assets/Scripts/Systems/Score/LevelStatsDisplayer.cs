using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelStatsDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_goal;
    [SerializeField]
    private TextMeshProUGUI m_score;
    [SerializeField]
    private TextMeshProUGUI m_spawned;
    [SerializeField]
    private TextMeshProUGUI m_moves;
    [SerializeField]
    private TextMeshProUGUI m_time;


    public void SetLineText(int goal,int score,int spawned, int moves, float time)
    {
        m_goal.text = goal.ToString();
        m_score.text = score.ToString();
        m_spawned.text = spawned.ToString();
        m_moves.text = moves.ToString();
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        m_time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
