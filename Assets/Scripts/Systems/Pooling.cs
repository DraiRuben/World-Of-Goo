using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    public List<ObjectPool> Pools;
    [Serializable]
    public struct ObjectPool
    {
        public string name;
        public List<GameObject> objects;
    }
    public Dictionary<string,List<GameObject>> pools;
    // Start is called before the first frame update
    void Start()
    {
        //faster with keys than going through an entire list
        foreach (var pool in Pools) { pools.Add(pool.name, pool.objects); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
