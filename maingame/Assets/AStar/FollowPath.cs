using UnityEngine;

public class FollowPath : MonoBehaviour {
    public GameObject wpManager;
    public GameObject player;

    Transform goal;
    float speed = 50.0f;
    float accuracy = 1.0f;


    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;

    void Start() {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        g = wpManager.GetComponent<WPManager>().graph;

        currentNode = wpManager.GetComponent<WPManager>().findNearestNode(transform.position);

        // Inicializar posicao da Gardenia junto ao player
        GameObject goalNode = wpManager.GetComponent<WPManager>().findNearestNode(player.transform.position);
        g.AStar(currentNode, goalNode);
        currentWP = 0;
    }

    public void SetNewGoal()
    {
        GameObject goalNode = wpManager.GetComponent<WPManager>().findNearestNode(player.transform.position);

        // Checa se o goal atual e diferente da posicao do player (goalNode)
        if (g.getPathLength() != 0 && Vector3.Distance(
            g.getPathPoint(g.getPathLength()-1).transform.position,
            goalNode.transform.position) > accuracy)
        {
            g.AStar(currentNode, goalNode);
            currentWP = 0;
        }
    }

    void LateUpdate() {
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;

        currentNode = g.getPathPoint(currentWP);

        // Se chegarmos no Node atual, ir para o prox
        if (Vector3.Distance(
            g.getPathPoint(currentWP).transform.position,
            transform.position) < accuracy) {
            currentWP++;
        }

        // Movimentar a Gardenia
        if (currentWP < g.getPathLength()) {
            goal = g.getPathPoint(currentWP).transform;

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goal.position, step);
        }

    }
}