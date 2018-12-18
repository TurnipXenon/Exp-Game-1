using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

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

    public NavMeshAgent agent;
 
    public Vector3 startPosition;
    private int livesLeft = 0;
    private int score = 0;

    // Use this for initialization
    void Start()
    {
        livesLeft = startLifeCount;
        gameOverText.text = "";
        SetScoreText();
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
            // todo change to win condition
            // reset enemy
            ResetPlayer();
            enemyScript.ResetEnemies();
            // reset pacdots
            pacdotScript.ResetPacdots();
        }

        return gameContinue;
    }

    private void CollectSpecialPacdot()
    {
        StartCoroutine(SetInvincible(false));

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
                    StartCoroutine(SetInvincible(true));
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
