using System;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    public static Pooling Instance;
    public List<ObjectPool> Pools;
    [Serializable]
    public struct ObjectPool
    {
        public string name;
        public List<UnityEngine.Object> objects;
    }
    public Dictionary<string, List<UnityEngine.Object>> pools = new();


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //faster with keys than going through an entire list
        foreach (var pool in Pools) { pools.Add(pool.name, pool.objects); }
    }
}
