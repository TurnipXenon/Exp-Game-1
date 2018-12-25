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
    public Text levelText;
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
        levelText.text = "";
        SetScoreText();
        endTime = 0.0f;
        levelNumber = 0;
        NewLevel();
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
            NewLevel();
        }

        return gameContinue;
    }

    private void NewLevel()
    {
        levelNumber++;
        StartCoroutine(StartLevel());
        enemyScript.SetDifficulty(levelNumber);
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
        StopInvincibility();
        enemyScript.ResetEnemies();
        pacdotScript.ResetPacdots();

        // pause game
        SetAllMovable(false);
        levelText.text = "Level " + levelNumber.ToString();

        yield return new WaitForSeconds(entryPauseTime);
        levelText.text = "";

        SetAllMovable(true);
    }

    private void SetAllMovable(bool isMovable)
    {
        enemyScript.SetAllGhostsMovable(isMovable);
        playerMobility.SetMovable(isMovable);
    }

    private void CollectSpecialPacdot()
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

            invincibilityTimer = InvincibitliyTimer();
            StartCoroutine(invincibilityTimer);
        }
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
                //Debug.Log("Ghost was eaten (Level " + levelNumber.ToString() + ")");
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

    private IEnumerator invincibilityTimer = null;

    private void StopInvincibility()
    {
        if (invincibilityTimer != null)
        {
            StopCoroutine(invincibilityTimer);
            invincibilityTimer = null;
        }

        endTime = 0; //  reset for cases like invincibility still happening while new level
        enemyScript.SetGhostsVulnerable(false);
        isSafe = false;
        edibleGhost = false;
        enemyScript.SetAllGhostsMovable(true);
    }

    private IEnumerator InvincibitliyTimer()
    {
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(0.5f);
        }

        StopInvincibility();
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
