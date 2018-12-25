using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyController : MonoBehaviour, Mobility
{
    public float speed;
    public float discreteIncrement;

    private readonly int IND_HORIZONTAL = 0;
    private readonly int IND_VERTICAL = 1;
    private readonly string[] AXIS_ARGS = { "Horizontal", "Vertical" };

    private Rigidbody rb;

    private float[] movement = { 1, 0 };
    private bool isMovable;

    // Use this for initialization
    void Start()
    {
        isMovable = true;
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
        if (isMovable)
        {
            rb.velocity = new Vector3(movement[IND_HORIZONTAL] * speed, 0.0f, movement[IND_VERTICAL] * speed);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void SetMovable(bool isMovable)
    {
        this.isMovable = isMovable;
    }
}
