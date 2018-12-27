using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerLoader : MonoBehaviour
{
    public GameObject gameManager;

    private void Awake()
    {
        if (GameManager.getInstance() == null)
            Instantiate(gameManager);
    }
}
