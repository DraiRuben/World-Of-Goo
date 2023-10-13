using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SpawnSettings : ScriptableObject
{
    public List<Spawnable> Spawn;
    public int GetTotalGooAmount()
    {
        int total = 0;
        foreach (var v in Spawn)
            total += v.Amount;

        return total;
    }
}
[Serializable]
public struct Spawnable
{
    public GameObject Object;
    public int Amount;
}