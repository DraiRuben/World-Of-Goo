using System.Collections.Generic;
using UnityEngine;


//Breadth first search, algorythm used to get shortest path, done with the help of GPT 3.5
public class BFS
{
    public static List<GameObject> FindShortestPath(
        Dictionary<GameObject, List<GameObject>> graph, GameObject start, GameObject end)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        Dictionary<GameObject, GameObject> parent = new Dictionary<GameObject, GameObject>();

        queue.Enqueue(start);
        parent[start] = null;

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            if (current == end)
                break;

            foreach (var neighbor in graph[current])
            {
                if (!parent.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    parent[neighbor] = current;
                }
            }
        }

        // If the end node wasn't reached, there is no path.
        if (!parent.ContainsKey(end))
            return null;

        // Reconstruct the shortest path by backtracking from end to start.
        List<GameObject> shortestPath = new List<GameObject>();
        GameObject currentNode = end;

        while (currentNode != null)
        {
            shortestPath.Add(currentNode);
            currentNode = parent[currentNode];
        }

        shortestPath.Reverse(); // Reverse to get the path from start to end.

        return shortestPath;
    }
}