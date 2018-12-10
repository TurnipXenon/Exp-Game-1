using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public NodeScript nodeScript;
    public NavMeshAgent agent;

    private Vector3 destination;
    private int currentNode;

	// Use this for initialization
	void Start () {
        currentNode = -1; // avoid error
        SetNewDestination();
	}
	
	// Update is called once per frame
	void Update () {

        // from https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    SetNewDestination();
                }
            }
        }

    }

    void SetNewDestination()
    {
        if (nodeScript != null && nodeScript.IsReady())
        {
            nodeScript.GetRandomDestination(currentNode, out currentNode, out destination);
            agent.SetDestination(destination);
        }
    }
}
