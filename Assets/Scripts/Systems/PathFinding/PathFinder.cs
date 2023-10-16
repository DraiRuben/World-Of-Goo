using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance;

    [Serializable]
    public struct Graph
    {
        public int vertices;
        //key is origin point, list is all available destinations
        public GOGOArrayDictionary Connections;
        public GameObject GetRandomDestination(GameObject Key)
        {
            return Connections[Key][UnityEngine.Random.Range(0, Connections[Key].Count)];
        }
        //not correct I think, I'll need to do a recursive algorythm, so I can return a list of destinations to go to in order I think

        public void RemovePoint(GameObject Point)
        {
            //removes the key and all references of the point/all connections to this point in the graph
            Connections.Remove(Point);
            foreach(var Connection in Connections)
            {
                Connections[Connection.Key] = FindAndRemovePoint(Connection,Point);
            }
        }
        private List<GameObject> FindAndRemovePoint(KeyValuePair<GameObject,List<GameObject>> Connection,  GameObject destination)
        {
            int index = Connection.Value.IndexOf(destination);
            if (index != -1)
            {
                //finds joint with point as connected body and resets it
                var connection = Connection.Value[index].GetComponents<SpringJoint2D>().ToList().Find(x => x.connectedBody == destination);
                if (connection != null)
                {
                    connection.connectedBody = null;
                    connection.autoConfigureDistance = true;
                    connection.enabled = false;
                }
                //finds bar with point as connected transform and deletes it
                if (Connection.Key.transform.childCount > 0)
                {
                    foreach (Transform child in Connection.Key.transform)
                    {
                        var comp = child.GetComponent<Connection>();
                        if (comp != null && comp.m_target == destination)
                        {
                            Destroy(comp.gameObject);
                        }
                    }
                }
                //removes point from connections in the graph
                Connection.Value.Remove(destination);
                return Connection.Value;
            }
            return Connection.Value;
        }
        public GameObject GetConnector(GameObject origin, GameObject destination)
        {
            var Connectors = origin.GetComponentsInChildren<Connection>();
            foreach (var Connector in Connectors)
            {
                if (Connector.m_target != null && Connector.m_target == destination)
                    return Connector.gameObject;
            }
            return null;
        }
        private void FindAndRemoveConnection(GameObject origin, GameObject destination)
        {
            if (origin.transform.childCount > 0)
            {
                foreach (Transform child in origin.transform)
                {
                    var comp = child.GetComponent<Connection>();
                    if (comp != null && comp.m_target == destination)
                    {
                        Destroy(comp.gameObject);
                    }
                }
                var connection = origin.GetComponents<SpringJoint2D>().ToList().Find(x => x.connectedBody == destination);
                if (connection != null)
                {
                    connection.connectedBody = null;
                    connection.autoConfigureDistance = true;
                    connection.enabled = false;
                }
            }
        }
        public void RemoveConnection(GameObject origin, GameObject destination)
        {
            //removes the key and all references of the point/all connections to this point in the graph & between objects
            Connections[origin].Remove(destination);
            Connections[destination].Remove(origin);
            FindAndRemoveConnection(origin, destination);
            FindAndRemoveConnection(destination, origin);
            

        }
        public List<GameObject> GetShortestPathBetween(GameObject origin, GameObject destination)
        {
            return BFS.FindShortestPath(Connections, origin, destination); ;
        }
    }
    [SerializeField]
    public Graph Structure;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
