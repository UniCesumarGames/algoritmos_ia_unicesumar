using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

public class NPC_AI : MonoBehaviour
{
    public List<Transform> wayPoints;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Animator anim;

    public int currentWaipointIndex = 0;
    public float speed = 2f;
    public bool isPlayerDetected = false;

    private bool onRadius;
    private bool hasAttacked = false;

    void Start()
   {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = speed;
        anim = GetComponent<Animator>();
   }


    void Update()
    {
        if (!isPlayerDetected)
        {
            Walking();
        }
        else
        {
            StopWalking();
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
            hasAttacked = true;
        }
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

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(wayPoints[currentWaipointIndex].position);
        onRadius = false;

    }

    private void StopWalking()
    {
        onRadius = true;
        navMeshAgent.isStopped = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            Debug.Log("Player Detectado!");
            isPlayerDetected = true;
            hasAttacked = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu do raio de interação!");
            isPlayerDetected = false;
            navMeshAgent.isStopped = false;
            anim.ResetTrigger("Attack");
        }
    }

}
