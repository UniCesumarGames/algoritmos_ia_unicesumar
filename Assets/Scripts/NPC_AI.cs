using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

public class NPC_AI : MonoBehaviour
{
    public List<Transform> wayPoints;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    public int currentWaipointIndex = 0;
    public float speed = 2f;
    void Start()
   {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = speed;
   }


    void Update()
    {
        Walking();
    }

    private void Walking()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }

        float distanceToWaypoint = Vector3.Distance
            (wayPoints[currentWaipointIndex].position, transform.position);

        if (distanceToWaypoint <= 2)
        {
            currentWaipointIndex = (currentWaipointIndex + 1) % wayPoints.Count;
        }

        navMeshAgent.SetDestination(wayPoints[currentWaipointIndex].position);

    }
}
