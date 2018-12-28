using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    // build setting should follow this order
    public static readonly int INDEX_SCENE_SPLASH = 0;
    public static readonly int INDEX_SCENE_LEVEL = 1;
    public static readonly int INDEX_SCENE_HIGH_SCORE = 2;

    public static readonly int KEY_HIGH_SCORE_DATA = 0;

    private static GameManager instance;
    private long highScore;
    private Dictionary<int, Data> storedData;

    // from https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;

            storedData = new Dictionary<int, Data>();

        }


        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public static GameManager getInstance()
    {
        return instance;
    }

    private readonly string HIGHSCORE_DAT_PATH = "/highScore2.dat";

    public HighScoreData GetHighScore()
    {
        string highScoreName = null;

        if (File.Exists(Application.persistentDataPath + HIGHSCORE_DAT_PATH))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + HIGHSCORE_DAT_PATH, FileMode.Open);
            HighScoreData data = (HighScoreData)bf.Deserialize(file);
            file.Close();

            this.highScore = data.number;
            highScoreName = data.name;
        }

        return new HighScoreData(this.highScore, highScoreName);
    }

    private void SaveHighScore(long newHighScore, string name)
    {
        this.highScore = newHighScore;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + HIGHSCORE_DAT_PATH);

        HighScoreData data = new HighScoreData(this.highScore, name);

        bf.Serialize(file, data);
        file.Close();
    }

    public bool SetNewHighScore(long newScore, string name)
    {
        bool isScoreTheHighest = IsScoreTheHighest(newScore);

        if (isScoreTheHighest)
        {
            SaveHighScore(newScore, name);
        }
        else
        {
            Debug.LogError("Set new high score ignores score that are not > high score");
        }

        return isScoreTheHighest;
    }

    private bool IsScoreTheHighest(long score)
    {
        return score > highScore;
    }

    public static void loadScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    public void OverwriteData<T>(int key, T data) where T : Data
    {
        storedData.Remove(key);
        storedData.Add(key, (Data)data);
    }

    public T GetStoredData<T>(int key) where T : Data
    {
        Data data = null;
        T convertedData = null;
        storedData.TryGetValue(key, out data);

        if (data is T)
        {
            convertedData = (T) data;
        }

        return convertedData;
    }
}

[Serializable]
public class Data { }

[Serializable]
public class HighScoreData : Data
{
    public long number;
    public string name;
    private int minimumNameLen = 3;

    public HighScoreData(long highScore, string highScoreName)
    {
        this.number = highScore;
        this.name = highScoreName;
    }

    public bool isAvailable()
    {
        return (name != null) && (name.Length >= minimumNameLen);
    }
}
