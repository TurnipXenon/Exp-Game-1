using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System;

// todo add discrete movement (round location)

public class PlayerManager : MonoBehaviour
{
    public int startLifeCount = 3;
    public int scoreIncrement = 100;
    public Text scoreText;
    public Text lifeText;
    public Text gameOverText;
    public string gameOverMessage;
    public PacdotScript pacdotScript;
    public EnemyScript enemyScript;
    public MonoBehaviour playerWithMobility;

    public NavMeshAgent agent;
 
    public Vector3 startPosition;
    private int livesLeft = 0;
    private int score = 0;
    private static Mobility playerMobility;

    // Use this for initialization
    void Start()
    {
        if (playerWithMobility is Mobility )
        {
            playerMobility = (Mobility)playerWithMobility;
        }
        else
        {
            throw new Exception("Player Mobility not interface of CharacterInterface.Mobility");
        }

        livesLeft = startLifeCount;
        gameOverText.text = "";
        SetScoreText();
        endTime = 0.0f;
        levelNumber = 1;
        StartCoroutine(StartLevel());
    }

    public float spawnOffset;

    private bool CollectPacdot(Collider other)
    {
        other.gameObject.SetActive(false);
        score += scoreIncrement;
        SetScoreText();

        bool gameContinue = pacdotScript.RemoveOnePacdot();
        if (!gameContinue)
        {
            StartCoroutine(StartLevel());
        }

        return gameContinue;
    }

    public float entryPauseTime;
    private int levelNumber;

    private IEnumerator StartLevel()
    {
        // todo change to win condition
        // reset enemy

        // pause game here
        // todo: show stage level

        ResetPlayer();
        enemyScript.ResetEnemies();
        pacdotScript.ResetPacdots();

        // pause game
        SetAllMovable(false);
        yield return new WaitForSeconds(entryPauseTime);
        SetAllMovable(true);
        
        Debug.Log("Start level " + levelNumber.ToString());
    }

    private void SetAllMovable(bool isMovable)
    {
        enemyScript.SetAllGhostsMovable(isMovable);
        playerMobility.SetMovable(isMovable);
    }

    private void CollectSpecialPacdot()
    {
        StartCoroutine(SetPacdotInvincibility());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            CollectPacdot(other);
        }
        else if (other.CompareTag("Special Collectible"))
        {
            if (CollectPacdot(other))
            {
                // todo invincibility
                CollectSpecialPacdot();
            }
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

        if (other.CompareTag("Enemy"))
        {
            if (!isSafe)
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
                    // todo dead animation or transition here
                    ResetPlayer();
                    StartCoroutine(SetJustDiedInvincibility());
                }
            }
            else if (edibleGhost)
            {
                other.GetComponent<EnemyController>().GhostEaten();
            }
        }
    }

    public void ResetPlayer()
    {
        edibleGhost = false;
        agent.Warp(startPosition);
    }

    // todo: placeholder
    private void SetLifeCount()
    {
        lifeText.text = "Lives left: " + livesLeft.ToString();

    }

    private bool isSafe = false;
    public float shortSafeTime;
    public float longSafeTime;
    private bool edibleGhost = false;

    public IEnumerator SetJustDiedInvincibility()
    {
        isSafe = true;
        yield return new WaitForSeconds(shortSafeTime);
        isSafe = false;
    }

    private static float endTime;

    // todo remove non looping part in ienumerator and make a func for it

    public IEnumerator SetPacdotInvincibility()
    {
        if (Time.time < endTime)
        {

            //Debug.Log("Current time: " + Time.time.ToString());
            //Debug.Log("End time: " + endTime.ToString());
            endTime += longSafeTime;

        }
        else
        {

            endTime = Time.time + longSafeTime;
            isSafe = true;
            edibleGhost = true;
            enemyScript.SetGhostsVulnerable(true);
            //Debug.Log("Current time: " + Time.time.ToString());
            //Debug.Log("End time: " + endTime.ToString());
            
            while (Time.time < endTime)
            {
                yield return new WaitForSeconds(0.5f);
            }

            enemyScript.SetGhostsVulnerable(false);
            isSafe = false;
            edibleGhost = false;
            enemyScript.SetAllGhostsMovable(true);

        }

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator SetInvincible(bool isFast)
    {
        isSafe = true;
        Debug.Log("Safe");
        float waitTime = (isFast) ? shortSafeTime : longSafeTime;
        if (!isFast)
        {
            // do when eating special
            edibleGhost = true;
        }
        yield return new WaitForSeconds(waitTime);
        isSafe = false;
        if (!isFast)
        {
            edibleGhost = false;
            enemyScript.SetAllGhostsMovable(true);
        }
        Debug.Log("No Longer Safe");
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
