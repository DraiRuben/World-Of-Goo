using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_levelNameText;
    [SerializeField]
    private Transform m_panel;
    [SerializeField]
    private GameObject m_linePrefab;

    [SerializeField]
    private Scrollbar m_scrollbar;

    private JsonDataService m_saver = new();
    private Animator m_animator;

    public DifficultySettings m_chosenLevelDifficulty;

    private LevelStats m_levelStats;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        m_animator.SetBool("Show", true);
        EventSystem.current.SetSelectedGameObject(m_scrollbar.gameObject);
        m_levelNameText.text = m_chosenLevelDifficulty.m_levelName + " "+ m_chosenLevelDifficulty.m_chosenDiff.ToString();
        //fill up the scoreboard
        //reuse previously filled scoreboard instead of destroying the lines and then instantiating new ones
        if (File.Exists(Application.persistentDataPath + $"/{m_chosenLevelDifficulty.m_levelName}.json"))
        {
            m_levelStats = m_saver.LoadData<LevelStats>(m_chosenLevelDifficulty.m_levelName);
            var levelData = m_levelStats.GetStats(m_chosenLevelDifficulty.m_chosenDiff).OrderBy(x => x.m_score).ToList();
            int childindex = 0;
            List<GameObject> allChildren = m_panel.transform.Cast<Transform>().Select(t => t.gameObject).ToList();
            if (m_panel.transform.childCount > 0)
            {
                foreach(GameObject child in allChildren)
                {
                    //if there's a still lines to set & we still have lines we can replace
                    if (levelData.Count > childindex)
                    {
                        var linedata = levelData[childindex];
                        allChildren[childindex].GetComponent<LevelStatsDisplayer>().SetLineText(linedata.m_minToComplete, linedata.m_score, linedata.m_totalSpawnedGoos, linedata.m_moves, linedata.m_timeToComplete);
                        childindex++;

                    }
                }
                //if we ran out of child to replace
                if (levelData.Count > childindex && childindex >= m_panel.transform.childCount)
                {
                    for (int i = childindex; i < levelData.Count; i++)
                    {
                        var temp = Instantiate(m_linePrefab, m_panel);
                        temp.GetComponent<LevelStatsDisplayer>().SetLineText(
                            levelData[i].m_minToComplete,
                            levelData[i].m_score,
                            levelData[i].m_totalSpawnedGoos,
                            levelData[i].m_moves,
                            levelData[i].m_timeToComplete);
                    }
                }//if we had more child than we needed, we erase the surplus
                else if (m_panel.transform.childCount>levelData.Count)
                {
                    for(int i = levelData.Count; i < m_panel.transform.childCount; i++)
                    {
                        Destroy(allChildren[i]);
                    }
                }
            }
            else // else we instantiate new lines to fill the scoreboard
            {
                foreach (var run in levelData)
                {
                    var temp = Instantiate(m_linePrefab, m_panel);
                    temp.GetComponent<LevelStatsDisplayer>().SetLineText(
                        levelData[childindex].m_minToComplete,
                        levelData[childindex].m_score,
                        levelData[childindex].m_totalSpawnedGoos,
                        levelData[childindex].m_moves,
                        levelData[childindex].m_timeToComplete);
                    childindex++;
                }
            }
        }
        else
        {
            foreach (Transform ch in m_panel)
            {
                Destroy(ch.gameObject);

            }
        }


    }

    public void Back()
    {
        StartCoroutine(Close());
    }
    private IEnumerator Close()
    {
        m_animator.SetBool("Show", false);
        yield return new WaitForSecondsRealtime(0.40f);
        gameObject.SetActive(false);
    }
}
