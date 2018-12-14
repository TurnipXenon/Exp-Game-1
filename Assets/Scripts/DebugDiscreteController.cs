using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDiscreteController : MonoBehaviour
{

    public float speed = 5;

    private readonly int IND_HORIZONTAL = 0;
    private readonly int IND_VERTICAL = 1;
    private readonly string[] AXIS_ARGS = { "Horizontal", "Vertical" };

    private Rigidbody rb;

    private float[] movement = { 1, 0 };

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // control
        for (int index = 0; index < 2; index++)
        {
            movement[index] = Input.GetAxis(AXIS_ARGS[index]);
        }
    }

    void FixedUpdate()
    {
       rb.velocity = new Vector3(movement[IND_HORIZONTAL] * speed, 0.0f, movement[IND_VERTICAL] * speed);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("SpawnPoint"))
        {
            Debug.Log("Spawn hit!");
        }
    }
}
