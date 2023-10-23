using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SpawnSettings : ScriptableObject
{
    public List<Spawnable> Spawn;
    public int m_additionnalGoos = 0;
    public int GetTotalGooAmount()
    {
        int total = 0;
        foreach (Spawnable v in Spawn)
            total += v.Amount;

        return total + m_additionnalGoos;
    }
}
[Serializable]
public struct Spawnable
{
    public GameObject Object;
    public int Amount;
}