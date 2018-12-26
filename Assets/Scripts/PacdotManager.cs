using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacdotManager : MonoBehaviour
{
    private int pacdotCount;
    private int pacdotMaxCount;

    // Start is called before the first frame update
    void Start()
    {
        pacdotMaxCount = 0;
        CountAllPacdots(gameObject.GetComponentsInChildren<Transform>());
        pacdotCount = pacdotMaxCount;
    }

    // Update is called once per frame
    void CountAllPacdots(Transform[] transformList)
    {
        foreach (Transform item in transformList)
        {
            if (item.CompareTag("Collectible") || item.CompareTag("Special Collectible"))
            {
                pacdotMaxCount++;
            }
        }
    }

    public void ResetPacdots()
    {
        pacdotCount = pacdotMaxCount;
        Transform[] transformList = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in transformList)
        {
            if (item.CompareTag("Collectible") || item.CompareTag("Special Collectible"))
            {
                item.gameObject.SetActive(true);
            }
        }
    }

    public bool RemoveOnePacdot()
    {
        pacdotCount--;
        return (pacdotCount > 0);
    }
}
