using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public NodeScript nodeScript;
    public NavMeshAgent agent;

    private Vector3 destination;
    private int currentNode;
    private bool isFrozen;

	// Use this for initialization
	void Start () {
        currentNode = -1; // avoid error
        SetNewDestination();
        isFrozen = false;
        agent.updateRotation = false;
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
        if ((nodeScript != null && nodeScript.IsReady()) && !isFrozen)
        {
            nodeScript.GetRandomDestination(currentNode, out currentNode, out destination);
            agent.SetDestination(destination);
        }
    }

    public void WarpToOrigin()
    {
        Vector3 resetLocation = new Vector3(0,0,0);
        agent.Warp(resetLocation);
    }

    public void GhostEaten()
    {
        Vector3 resetLocation = new Vector3(0, 0, 0);
        agent.SetDestination(resetLocation);
        isFrozen = true;
    }

    public void SetGhostMovable(bool isMovable)
    {
        isFrozen = false;
    }

    private void LateUpdate()
    {
        // from: https://forum.unity.com/threads/solved-navmesh-agent-instant-turn-in-direction-he-moves.521794/
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }
}
