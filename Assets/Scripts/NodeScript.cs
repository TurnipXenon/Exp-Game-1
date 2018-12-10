using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    private Transform[] nodes;

    // Use this for initialization
    void Start()
    {
        nodes = GetComponentsInChildren<Transform>();
    }

    public void GetRandomDestination(int currentNode, out int nextNode, out Vector3 destination)
    {
        int nodeBound = nodes.Length - 1;

        do
        {
            nextNode = Mathf.RoundToInt(Random.value * nodeBound);
        }
        while (nextNode == currentNode);

        destination = nodes[nextNode].position;
    }

    public bool IsReady()
    {
        return nodes != null;
    }
}
