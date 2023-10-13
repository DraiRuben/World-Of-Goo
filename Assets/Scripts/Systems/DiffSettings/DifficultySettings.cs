using System;
using UnityEngine;

[CreateAssetMenu]
public class DifficultySettings : ScriptableObject
{
    public string m_levelName;
    [SerializeField, Range(0, 1)]
    private float m_easyMinPercentToGet;
    [SerializeField, Range(0, 1)]
    private float m_mediumMinPercentToGet;
    [SerializeField, Range(0, 1)]
    private float m_hardMinPercentToGet;

    public Difficulty m_chosenDiff = Difficulty.Easy;
    public float GetActiveMultiplier()
    {
        switch (m_chosenDiff)
        {
            case Difficulty.Easy:
                return m_easyMinPercentToGet;
            case Difficulty.Medium:
                return m_mediumMinPercentToGet;
            case Difficulty.Hard:
                return m_hardMinPercentToGet;
        }
        return 1f;
    }
}
[Serializable]
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}