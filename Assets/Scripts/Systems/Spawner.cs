using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<Spawnable> Spawn;
    [SerializeField]
    private SpawnSettings SpawnSettings;
    void Start()
    {
        Spawn = SpawnSettings.Spawn;
        foreach (Spawnable v in Spawn)
        {
            for (int i = 0; i < v.Amount; i++)
            {
                GameObject goo = Instantiate(v.Object, transform.position, Quaternion.identity);
                goo.GetComponent<Goo>().MoveOutOfStructure(false);
            }
        }
        Score.Instance.m_totalSpawnedGoos = SpawnSettings.GetTotalGooAmount();
    }

}
