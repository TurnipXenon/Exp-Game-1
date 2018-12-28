using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public GameObject btnContinue;
    public Text txtHighScoreNumber;
    public Text txtHighScoreName;

    private GameManager gameManager;
    private HighScoreData highScoreData;
    private static readonly string LEVEL_SCENE = "Level Scene";

    private void Start()
    {
        // get high score data
        gameManager = GameManager.getInstance();
        highScoreData = gameManager.GetHighScore();

        if (highScoreData.isAvailable())
        {
            // display results
            txtHighScoreName.gameObject.SetActive(true);
            txtHighScoreNumber.gameObject.SetActive(true);
            txtHighScoreNumber.text = "Highscore: " + highScoreData.number.ToString();
            txtHighScoreName.text = "by " + highScoreData.name;
        }
        else
        {
            // hide
            txtHighScoreName.gameObject.SetActive(false);
            txtHighScoreNumber.gameObject.SetActive(false);
        }

        // if no data
        btnContinue.SetActive(false);
    }

    public void OnClick_BtnNewGame()
    {
        GameManager.loadScene(GameManager.INDEX_SCENE_LEVEL);
    }
}
