using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int count = 0;
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col1 in colliders)
        {
            foreach (Collider col2 in colliders)
            {
                if (!col1.Equals(col2))
                {
                    Physics.IgnoreCollision(col1, col2, true);
                    count++;
                    Debug.Log("Ignoring Colliders Count: " + count);
                }
            }
        }
	}

    public void ResetEnemies()
    {
        EnemyController[] transformList = GetComponentsInChildren<EnemyController>();
        foreach (EnemyController item in transformList)
        {
            item.WarpToOrigin();
        }
    }

    public void SetAllGhostsMovable(bool isMovable)
    {
        foreach (EnemyController item in GetComponentsInChildren<EnemyController>())
        {
            item.SetGhostMovable(true);
        }
    }
}
