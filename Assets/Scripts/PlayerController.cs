using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

// todo add discrete movement (round location)

public class PlayerController : MonoBehaviour
{
    public int startLifeCount = 3;
    public int scoreIncrement = 100;
    public Text scoreText;
    public Text lifeText;
    public Text gameOverText;
    public string gameOverMessage;

    public float movementFrame = 10.0f;
    public float incrementRadius = 0.5f;
    public NavMeshAgent agent;

    private readonly int IND_HORIZONTAL = 1;
    private readonly int IND_VERTICAL = 0;
    private readonly string[] AXIS_ARGS = { "Vertical", "Horizontal" };
    private readonly int[] COORD_RANGE = new int[]{0, 1};

    private readonly static int BIT_UP = 1;
    private readonly static int BIT_DOWN = 2;
    private readonly static int BIT_LEFT = 4;
    private readonly static int BIT_RIGHT = 8;

    private readonly static int[] BIT_DIR_ARRAY = 
        new int[] { BIT_UP, BIT_DOWN, BIT_RIGHT, BIT_LEFT };
    // WARNING: BIT_LEFT AND RIGHT ARE REVERSED HERE
    private int validDirection; // use bitwise
    private Vector3 destination;
 
    public float mainIncrement;
    public float secondaryIncrement;
    public Vector3 startPosition;
    private Vector3 staticCenter;
    private int currentDirection;
    private int livesLeft = 0;
    private int score = 0;

    // Use this for initialization
    void Start()
    {
        livesLeft = startLifeCount;
        currentDirection = BIT_RIGHT;
        validDirection = BIT_RIGHT | BIT_LEFT;
        staticCenter = CloneVector(transform.position);

        SetDestination(currentDirection);
        gameOverText.text = "";
        SetScoreText();
    }

    private Vector3 CloneVector(Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    private bool AndBitwise(int val1, int val2)
    {
        return (val1 & val2) > 0;
    }

    private void SetIndTarget(int direction, float increment, float x, float z, 
        out float x2, out float z2)
    {
        if (AndBitwise(direction, BIT_UP))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z + incrementRadius);
            z += increment;
        }
        else if (AndBitwise(direction, BIT_DOWN))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z - incrementRadius);
            z -= increment;
        }
        if (AndBitwise(direction, BIT_RIGHT))
        {
            destination = new Vector3(staticCenter.x - incrementRadius, staticCenter.y, staticCenter.z);
            x += increment;
        }
        else if (AndBitwise(direction, BIT_LEFT))
        {
            x -= increment;
        }
        
        x2 = x;
        z2 = z;
    }

    //private float RoundToNearestHalf(float num)
    //{
    //    return Mathf.Round(num * 2) / 2;

    //}

    private void SetNewDestination(int directionBitwise)
    {
        if (AndBitwise(directionBitwise, BIT_UP))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z + mainIncrement);
        }
        else if (AndBitwise(directionBitwise, BIT_UP))
        {
            destination = new Vector3(staticCenter.x, staticCenter.y, staticCenter.z - mainIncrement);
        }
        else if (AndBitwise(directionBitwise, BIT_LEFT))
        {
            destination = new Vector3(staticCenter.x - mainIncrement, staticCenter.y, staticCenter.z);
        }
        else if (AndBitwise(directionBitwise, BIT_RIGHT))
        {
            destination = new Vector3(staticCenter.x + mainIncrement, staticCenter.y, staticCenter.z);
        }
    }

    private Vector3 SetTarget(int mainDirection, int secondaryDirection)
    {
        Vector3 currentPosition = transform.position;
        float x = currentPosition.x;
        float y = currentPosition.y;
        float z = currentPosition.z;
        
        SetIndTarget(mainDirection, mainIncrement, x, z, out x, out z);
        SetIndTarget(secondaryDirection, secondaryIncrement, x, z, out x, out z);

        //float x2 = RoundToNearestHalf(x);
        //float z2 = RoundToNearestHalf(z);
        return new Vector3(x, y, z);
    }

    private bool IsDirectionHorizontal()
    {
        return AndBitwise(currentDirection, BIT_RIGHT | BIT_LEFT);
    }

    private int[] horizontalOptions = new int[] { 0, BIT_LEFT, BIT_RIGHT };
    private int[] verticalOptions = new int[] { 0, BIT_UP, BIT_DOWN };
    public float sampleMovementRadius;

    // if returns false, is vertical
    // assumes only one direction
    private bool IsHorizontal(int direction)
    {
        return AndBitwise(direction, BIT_LEFT | BIT_RIGHT);
    }

    private void SetDestination(int mainBit)
    {
        int[] optionalBits = horizontalOptions;
        if (IsHorizontal(mainBit))
         {
            optionalBits = verticalOptions;
        } // else option already done

        foreach (int secondaryBit in optionalBits)
        {
            Vector3 tmpDestination = SetTarget(mainBit, secondaryBit);
            NavMeshHit hit;
            //print(tmpDestination.ToString());

            bool isHit = NavMesh.SamplePosition(tmpDestination, out hit, sampleMovementRadius, NavMesh.AllAreas);
            if (isHit)
             {
                //print("Detects hit");
                agent.SetDestination(hit.position);
                break;
             }
         }
    }
 
    private void CheckDestination(int directionBitwise)
    {
        int mainBit = 0;
        
        foreach (int bitDirection in BIT_DIR_ARRAY)
         {
            mainBit = directionBitwise & bitDirection;
 
            if (mainBit != 0)
             {
                SetDestination(mainBit);
                break;
             }
         }
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
                break;
            }
        }
 
        CheckDestination(movementInput);
     }

    public float spawnOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            other.gameObject.SetActive(false);
            score += scoreIncrement;
            SetScoreText();
        }
        else if (other.CompareTag("SpawnPoint"))
        {
            Vector3 position = transform.position;
            float positionX = -position.x;
            // put offset
            positionX = (positionX > 0) ? positionX - spawnOffset : positionX + spawnOffset;
            Vector3 spawnPosition = new Vector3(positionX, position.y, position.z);
            agent.Warp(spawnPosition);
        }

        if (!isSafe && other.CompareTag("Enemy"))
        {
            livesLeft--;
            SetLifeCount();

            if (livesLeft <= 0)
            {
                gameObject.SetActive(false);
                gameOverText.text = gameOverMessage;
            }
            else
            {
                agent.Warp(startPosition);
                // todo dead animation or transition here
                StartCoroutine(SetInvincible(true));
            }
        }
    }

    // todo: placeholder
    private void SetLifeCount()
    {
        lifeText.text = "Lives left: " + livesLeft.ToString();

    }

    private bool isSafe = false;
    public float shortSafeTime;
    public float longSafeTime;

    public IEnumerator SetInvincible(bool isFast)
    {
        isSafe = true;
        Debug.Log("Safe");
        float waitTime = (isFast) ? shortSafeTime : longSafeTime;
        yield return new WaitForSeconds(waitTime);
        isSafe = false;
        Debug.Log("No Longer Safe");
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
