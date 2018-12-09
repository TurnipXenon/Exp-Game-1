using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float movementFrame = 10.0f;
    public float incrementRadius = 0.5f;
    public GameObject gamePlane;

    private readonly int IND_HORIZONTAL = 1;
    private readonly int IND_VERTICAL = 0;
    private readonly string[] AXIS_ARGS = { "Vertical", "Horizontal" };

    private Rigidbody rb;
    private float[] directions = { 1, 0 };
    private float spawnOffset = 0;
    private float SPAWN_POSITION_LIMIT = 0;

    private readonly int[] COORD_RANGE = new int[]{0, 1};

    private readonly int IND_UP = 0;
    private readonly int IND_DOWN = 1;
    private readonly int IND_LEFT = 2;
    private readonly int IND_RIGHT = 3;

    private readonly int BIT_UP = 1;
    private readonly int BIT_DOWN = 2;
    private readonly int BIT_LEFT = 4;
    private readonly int BIT_RIGHT = 8;

    private int[] BIT_DIR_ARRAY; // WARNING: BIT_LEFT AND RIGHT ARE REVERSED HERE
    private Vector3 staticCenter;
    private int currentDirection;
    private int validDirection; // use bitwise
    private Vector3 destination;

    private float increment;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        increment = incrementRadius / movementFrame;
        BIT_DIR_ARRAY = new int[] { BIT_UP, BIT_DOWN, BIT_RIGHT, BIT_LEFT };
        currentDirection = BIT_RIGHT;
        validDirection = BIT_RIGHT | BIT_LEFT;
        staticCenter = CloneVector(transform.position);

        SetDestination(currentDirection);
    }

    private Vector3 CloneVector(Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    private bool AndBitwise(int val1, int val2)
    {
        return (val1 & val2) > 0;
    }

    private void SetDestination(int directionBitwise)
    {
        if (AndBitwise(directionBitwise, BIT_UP))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z + incrementRadius);
        }
        else if (AndBitwise(directionBitwise, BIT_UP))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z - incrementRadius);
        }
        else if (AndBitwise(directionBitwise, BIT_LEFT))
        {
            destination = new Vector3(staticCenter.x - incrementRadius, staticCenter.y, staticCenter.z);
        }
        else if (AndBitwise(directionBitwise, BIT_RIGHT))
        {
            destination = new Vector3(staticCenter.x + incrementRadius, staticCenter.y, staticCenter.z);
        }
    }

    private bool IsDirectionHorizontal()
    {
        return AndBitwise(currentDirection, BIT_RIGHT | BIT_LEFT);
    }

    private void Update()
    {
        // control
        int movementInput = 0;
        foreach (int index in COORD_RANGE)
        {
            float movement = Input.GetAxis(AXIS_ARGS[index]);
            if (movement != 0)
            {
                int factor = (movement > 0) ? 0 : 1;
                movementInput = movementInput | BIT_DIR_ARRAY[(index * 2) + factor];
            }
        }

        // change movement based on control
        bool isValidDirection = AndBitwise(validDirection, movementInput);
        bool isDifferentInput = !AndBitwise(currentDirection, movementInput);
        if (isValidDirection && isDifferentInput)
        {
            // set new destination
            SetDestination(movementInput);
        }


        // go to center (how and when)
        // if aligned to destination
        Vector3 currentPosition = transform.position;
        if (Mathf.Approximately(destination.x, currentPosition.x) ||
            Mathf.Approximately(destination.z, currentPosition.z))
        {
            // move to desination
           if (currentPosition.z > destination.z)
            {

            }
        }
        else
        {
            // move to center

        }

        // go to destination

        // set position


        // check if reach destination

        // if true, change static center
    }

    void FixedUpdate()
    {   

    }
}
