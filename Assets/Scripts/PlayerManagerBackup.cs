using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System;

// todo player being targeted
// todo player being seen
// todo ghost can no longer be eaten
// todo when ghost stuck inside make them go wild

public class PlayerManagerBackup : MonoBehaviour
{
    public int startLifeCount;
    public MonoBehaviour playerWithMobility;
    public NavMeshAgent agent;
    public Vector3 startPosition;
    private int livesLeft = 0;
    private int score = 0;
    private static Mobility playerMobility;
    public float spawnOffset;
    public GameManager gameControl;

    // Use this for initialization
    void Start()
    {
        if (playerWithMobility is Mobility)
        {
            playerMobility = (Mobility)playerWithMobility;
        }
        else
        {
            throw new Exception("Player Mobility not interface of CharacterInterface.Mobility");
        }

        livesLeft = startLifeCount;
    }


    private bool CollectPacdot(Collider other)
    {
        other.gameObject.SetActive(false);

        bool gameContinue = gameControl.CollectPacdot();
        if (!gameContinue)
        {
            gameControl.NewLevel();
        }

        return gameContinue;
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
                this.isSafe = gameControl.CollectSpecialPacdot(this.isSafe);
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
                gameControl.SetLifeSprites();

                if (livesLeft <= 0)
                {
                    gameObject.SetActive(false);
                    gameControl.SetGameOver();
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
        gameControl.SetGhostsVulnerable(false);
        isSafe = false;
        edibleGhost = false;
        gameControl.SetAllGhostsMovable(true);
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
            gameControl.SetAllGhostsMovable(true);
        }
        Debug.Log("No Longer Safe");
    }

    public int GetLivesLeft()
    {
        return livesLeft;
    }

    public void SetMovable(bool isMovable)
    {
        playerMobility.SetMovable(true);
    }

    public void ResetPosition()
    {
        agent.Warp(startPosition);
    }

    public void SetSafe(bool isSafe)
    {
        this.isSafe = isSafe;
    }
}
