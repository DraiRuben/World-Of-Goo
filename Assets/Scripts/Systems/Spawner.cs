using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    List<Spawnable> Spawn;
    void Start()
    {
        foreach(var v in Spawn)
        {
            for(int i = 0; i < v.Amount; i++)
            {
                Instantiate(v.Object,transform.position, Quaternion.identity);
            }
        }
    }
    [Serializable]
    struct Spawnable
    {
        public GameObject Object;
        public int Amount;
    }
    
}
