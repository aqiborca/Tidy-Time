using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetTaskManager : MonoBehaviour
{
    public GameObject[] clothingItems; //unorganized
    public GameObject[] organizedVersions; //organized
    public GameObject[] collectables; //trash and fish food
    private bool taskComplete = false;

    void Update()
    {
        if (taskComplete) return;

        bool allClothesPlaced = true;
        foreach (GameObject item in clothingItems)
        {
            if (item.activeSelf) //if still visible -> not placed yet
            {
                allClothesPlaced = false;
                break;
            }
        }

        bool allCollected = true;
        foreach (GameObject item in collectables)
        {
            if (item.activeSelf) //if still visible -> not collected yet
            {
                allCollected = false;
                break;
            }
        }

        if (allClothesPlaced && allCollected)
        {
            taskComplete = true;
            Debug.Log("Closet chore completed");
            ChoreManager.Instance.CompleteChore("organizecloset");
        }
    }
}
