using UnityEngine;
using UnityEngine.AI;

public class DispayNavMeshPath : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (agent.hasPath) {
            lineRenderer.SetPositions(agent.path.corners);
        }
    }
}
