using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// todo move non player functions and details here

public class LevelManager : MonoBehaviour { 

    public int scoreIncrement;

    // UI components
    public Text scoreText;
    public Text gameOverText;
    public Text levelText;
    public Text textHighScore;
    public GameObject parentLifeSprites;
    public Sprite lifeSprite;

    public string gameOverMessage;
    public string highScoreMessage;
    public PacdotManager pacdotScript;
    public EnemyManager enemyScript;
    public PlayerManager playerManager;

    public float entryPauseTime;
    public float exitPauseTime;
    private int levelNumber;
    public float spriteScale;
    public float spriteOffset;

    private string topScorer;
    private long score = 0;
    private GameManager gameManager;
    private HighScoreData highScoreData;

    private void Awake()
    {
        gameManager = GameManager.getInstance();
        highScoreData = gameManager.GetHighScore();
    }

    // Use this for initialization
    void Start()
    {
        textHighScore.text = highScoreMessage + highScoreData.number.ToString();

        SetLifeSprites();

        gameOverText.text = "";
        levelText.text = "";
        SetScoreText(0);

        endTime = 0.0f;
        levelNumber = 0;

        NewLevel();
        //gameManager = GameManager.instance;
        //GameData data = gameManager.GetHighScore();
    }

    public void SetLifeSprites()
    {
        int livesLeft = playerManager.GetLivesLeft();
        Image[] spriteList = parentLifeSprites.GetComponentsInChildren<Image>();
        int lenSprites = spriteList.Length;
        //Debug.Log("Initial lenSprites: " + lenSprites.ToString());

        while (lenSprites > livesLeft)
        {
            // todo remove
            lenSprites--;
            Destroy(spriteList[lenSprites]);
        }

        while (lenSprites < livesLeft)
        {
            // from https://gamedev.stackexchange.com/a/102432
            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            image.sprite = lifeSprite;
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
            rectTransform.SetParent(parentLifeSprites.transform);
            float xPos = lenSprites * spriteOffset;
            rectTransform.anchoredPosition = new Vector2(xPos, 0);
            obj.SetActive(true);
            lenSprites++;
            //Debug.Log(lenSprites.ToString() + " < " + livesLeft.ToString() + ": " + (lenSprites < livesLeft).ToString());
        }
    }

    public void NewLevel()
    {
        levelNumber++;
        if (levelNumber > 1)
        {
            SetScoreText(scoreIncrement * 150);
        }
        StartCoroutine(StartLevel());
        enemyScript.SetDifficulty(levelNumber);
    }

    private IEnumerator StartLevel()
    {
        //Debug.Log("We went here");
        // todo change to win condition
        // reset enemy

        // pause game here
        // todo: show stage level

        ResetPlayer();
        StopInvincibility();
        enemyScript.ResetEnemies();
        pacdotScript.ResetPacdots();

        // pause game
        //Debug.Log(levelText != null);
        SetAllMovable(false);
        levelText.text = "Level " + levelNumber.ToString();

        yield return new WaitForSeconds(entryPauseTime);
        levelText.text = "";

        SetAllMovable(true);
    }

    private void SetAllMovable(bool isMovable)
    {
        playerManager.SetMovable(isMovable);
        enemyScript.SetAllGhostsMovable(isMovable);
    }

    public bool CollectSpecialPacdot(bool isSafe)
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

        return isSafe;
    }

    public void ResetPlayer()
    {
        edibleGhost = false;
        playerManager.ResetPlayer();
    }

    private bool isSafe = false;
    public float shortSafeTime;
    public float longSafeTime;
    public bool edibleGhost = false;
    private static float endTime;

    // todo remove non looping part in ienumerator and make a func for it

    private IEnumerator invincibilityTimer = null;

    // set bool as false
    private void StopInvincibility()
    {
        if (invincibilityTimer != null)
        {
            StopCoroutine(invincibilityTimer);
            invincibilityTimer = null;
        }

        endTime = 0; //  reset for cases like invincibility still happening while new level
        playerManager.SetSafe(false);
        enemyScript.SetGhostsVulnerable(false);
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

    private void SetScoreText(int scoreIncrement)
    {
        score += scoreIncrement;
        scoreText.text = "Score: " + score.ToString();
    }

    public bool CollectPacdot()
    {
        SetScoreText(scoreIncrement);
        return pacdotScript.RemoveOnePacdot();
    }

    public void SetGameOver()
    {
        gameOverText.text = gameOverMessage;

        // pause
        StartCoroutine(WaitThenGoBack());
    }

    private IEnumerator WaitThenGoBack()
    {
        yield return new WaitForSeconds(exitPauseTime);

        GoBack();
    }

    private void GoBack()
    {
        // check if high score
        if (score > highScoreData.number)
        {
            // true: go to high score screen
            highScoreData.number = score;
            gameManager.OverwriteData<HighScoreData>(GameManager.KEY_HIGH_SCORE_DATA, highScoreData);
            GameManager.loadScene(GameManager.INDEX_SCENE_HIGH_SCORE);
        }
        else
        {
            // false: go back normally
            GameManager.loadScene(GameManager.INDEX_SCENE_SPLASH);
        }

    }

    public void SetAllGhostsMovable(bool isMovable)
    {
        enemyScript.SetAllGhostsMovable(isMovable);
    }

    public void SetGhostsVulnerable(bool isVulnerable)
    {
        enemyScript.SetGhostsVulnerable(isVulnerable);
    }
}
