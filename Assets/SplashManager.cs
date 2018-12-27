using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public GameObject btnContinue;
    public GameObject txtHighScoreNumber;
    public GameObject txtHighScoreName;

    private GameManager gameManager;
    private HighScoreData gameData;
    private static readonly string LEVEL_SCENE = "Level Scene";

    private void Start()
    {
        // get high score data
        gameManager = GameManager.getInstance();
        gameData = gameManager.GetHighScore();

        if (gameData.isAvailable())
        {
            // display results
        }
        else
        {
            // hide
            txtHighScoreName.SetActive(false);
            txtHighScoreNumber.SetActive(false);
        }

        // if no data
        btnContinue.SetActive(false);
    }

    public void OnClick_BtnNewGame()
    {
        gameManager.loadScene(GameManager.INDEX_SCENE_LEVEL);
    }
}
