using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    struct Graph
    {
        //key is origin point, list is all available destinations
        Dictionary<GameObject, List<GameObject>> Connections;
        public GameObject GetRandomDestination(GameObject Key)
        {
            return Connections[Key][Random.Range(0, Connections[Key].Count)];
        }
        public GameObject GetNextStepTo(GameObject destination)
        {
            return null;
        }
    }

}
