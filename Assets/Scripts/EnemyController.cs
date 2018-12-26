using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public NodeManager nodeScript;
    public NavMeshAgent agent;

    private Material material;
    private Vector3 destination;
    private int currentNode;
    private bool isMovable;
    private Color defaultColor;

	// Use this for initialization
	void Start () {
        currentNode = -1; // avoid error
        SetNewDestination();
        isMovable = true;
        agent.updateRotation = false;
        material = GetComponent<Renderer>().material;
        defaultColor = material.color;
	}
	
	// Update is called once per frame
	void Update () {

        // from https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        if (!agent.pathPending && agent.isOnNavMesh)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    SetNewDestination();
                }
            }
        }

        InstantlyTurn();
    }

    // from: http://answers.unity.com/answers/1344996/view.html
    // from 2: http://answers.unity.com/answers/988754/view.html
    public float rotSpeed;
    public float turningDistance;
    public float acceleration = 2.0f;
    public float deceleration = 60.0f;

    private void InstantlyTurn()
    {
        if ((destination - transform.position).magnitude < turningDistance)
        {
            agent.acceleration = deceleration;
        }
        else
        {
            agent.acceleration = acceleration;
        }
    }

    void SetNewDestination()
    {
        if ((nodeScript != null && nodeScript.IsReady()) && isMovable)
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

    private  Vector3 resetLocation = new Vector3(0, 0, 0);

    public void GhostEaten()
    {
            agent.Warp(resetLocation);
            isMovable = false;
    }

    private bool isFlashing;

    public void FlashGhosts(bool isFlashing)
    {
        this.isFlashing = isFlashing;

        if (isFlashing)
        {
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        bool isColorOne = true;

        while (isFlashing)
        {
            if (isColorOne)
            {
                material.color = Color.red;
            }
            else
            {
                material.color = Color.blue;
            }

            isColorOne = !isColorOne;

            yield return new WaitForSeconds(0.5f);
        }

        material.color = defaultColor;
    }

    public void SetGhostMovable(bool isMovable)
    {
        this.isMovable = isMovable;
        if (isMovable)
        {
            SetNewDestination();
        }
    }

    private void LateUpdate()
    {
        //// from: https://forum.unity.com/threads/solved-navmesh-agent-instant-turn-in-direction-he-moves.521794/
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }
}
