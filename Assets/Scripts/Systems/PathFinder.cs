using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance;

    [Serializable]
    public struct Graph
    {

        //key is origin point, list is all available destinations
        public GOGOArrayDictionary Connections;
        public GameObject GetRandomDestination(GameObject Key)
        {
            return Connections[Key][UnityEngine.Random.Range(0, Connections[Key].Count)];
        }
        //not correct I think, I'll need to do a recursive algorythm, so I can return a list of destinations to go to in order I think
        public GameObject GetNextStepTo(GameObject destination)
        {
            return null;
        }
        public void RemovePoint(GameObject Point)
        {
            //removes the key and all references of the point/all connections to this point in the graph
            Connections.Remove(Point);
            foreach(var Connection in Connections)
            {
                Connection.Value.Remove(Point);
            }
        }
        public void RemoveConnection(GameObject origin, GameObject Destination)
        {
            //removes the key and all references of the point/all connections to this point in the graph
            Connections[origin].Remove(Destination);
            Connections[Destination].Remove(origin);     
        }
    }
    [SerializeField]
    public Graph Structure = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
