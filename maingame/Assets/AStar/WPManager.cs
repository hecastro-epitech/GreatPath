using UnityEngine;

[System.Serializable]
public struct Link {
    public GameObject node1;
    public GameObject node2;
}

public class WPManager : MonoBehaviour {

    public GameObject[] waypoints;
    public Link[] links;
    public Graph graph = new Graph();

    void Awake() {
        if (waypoints.Length > 0) {
            foreach (GameObject wp in waypoints) {
                graph.AddNode(wp);
            }

            foreach (Link l in links) {
                graph.AddEdge(l.node1, l.node2);
                graph.AddEdge(l.node2, l.node1);
            }
        }
    }

    void Update() {
        graph.debugDraw();
    }

    public GameObject findNearestNode(Vector3 currentPosition)
    {
        float lowestDist = Mathf.Infinity;
        GameObject nearest = null;;

        foreach (GameObject obj in waypoints)
        {
            float dist = Vector3.Distance(currentPosition, obj.transform.position);
            if(dist < lowestDist)
            {
                lowestDist = dist;
                nearest = obj;
            }
        }
        return nearest;
    }
}
