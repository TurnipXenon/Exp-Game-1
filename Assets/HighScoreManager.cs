using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    public Text textName;
    public Image imageWarningDialogue;
    public float timeDialogShow;
    public float timeDialogFade;

    private Text textWarning;
    private GameManager gameManager;
    private long newScore;
    private string name;

    private void Awake()
    {
        gameManager = GameManager.getInstance();

        HighScoreData highScoreData = gameManager.GetStoredData<HighScoreData>(GameManager.KEY_HIGH_SCORE_DATA);

        if (highScoreData != null)
        {
            newScore = highScoreData.number;
        }
        else
        {
            newScore = 0;
        }

        name = null;
    }

    private void Start()
    {
        textWarning = imageWarningDialogue.GetComponentInChildren<Text>();
        imageWarningDialogue.gameObject.SetActive(false);
    }

    public void OnClick_BtnSave()
    {
        // validate
        string text = textName.text;
        string warning = null;

        if (text == null)
        {
            warning = "Please add you name";
        }
        else if (text.Length < 3)
        {
            warning = "Name should be longer than two (2) characters";
        }

        // get name if no warning
        if (warning == null)
        {
            name = text;
            Exit(true);
        }
        else
        {
            // warn user
            StartCoroutine(Warn(warning, timeDialogShow, timeDialogFade));
        }
    }

    private IEnumerator Warn(string strWarning, float timeShow, float timeFade)
    {
        Debug.Log("Warn user");
        this.textWarning.text = strWarning;

        List<Graphic> components = GetComponents(imageWarningDialogue);
        foreach (Graphic item in components)
        {
            if (item == textWarning)
            {
                Debug.Log("Item is warning text");
            }
            else
            {
                Debug.Log("Item is not warning text");
            }
            item.gameObject.SetActive(true);
            item.color = new Color(item.color.r, item.color.g, item.color.b, 1.0f);
        }

        yield return new WaitForSeconds(timeShow);

        StartCoroutine(FadeComponentsToZeroAlpha(components, timeFade));
    }

    private List<Graphic> GetComponents(Graphic parent)
    {

        List<Graphic> components = new List<Graphic>();
        components.Add(parent);
        components.AddRange(parent.GetComponentsInChildren<Graphic>(true));
        return components;
    }

    // from https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    // assumes that they are already shown full
    private IEnumerator FadeComponentsToZeroAlpha(List<Graphic> components, float time)
    {
        float startTime = Time.time;
        float endTime = startTime + time;
        while (Time.time < endTime)
        {
            float timeDiff = Time.time - startTime;
            float percentage = timeDiff / time;


            foreach (Graphic item in components)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 1.0f - percentage);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        foreach (Graphic item in components)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void OnClick_BtnCancel()
    {
        Exit(false);
    }

    void Exit(bool shouldSave)
    {
        if (shouldSave)
        {
            gameManager.SetNewHighScore(newScore, name);
        }

        Debug.Log("Went here");
        GameManager.loadScene(GameManager.INDEX_SCENE_SPLASH);
    }
}
