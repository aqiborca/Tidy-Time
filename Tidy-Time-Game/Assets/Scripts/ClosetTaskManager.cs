using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetTaskManager : MonoBehaviour
{
    public GameObject[] clothingItems; //unorganized
    public GameObject[] organizedVersions; //organized
    public GameObject[] collectables; //trash and fish food
    public GameObject uiWithHints; //UI at the start
    public GameObject finalUi;     //UI after completion (no bubbles)

    private bool taskComplete = false;
    private List<GameObject> clothingQueue = new List<GameObject>();
    private int currentItemIndex = 0;

    void Start()
    {
        //shuffle clothingItems into clothingQueue
        clothingQueue = new List<GameObject>(clothingItems);
        Shuffle(clothingQueue);

        //deactivate all, then activate first
        foreach (GameObject item in clothingItems)
            item.SetActive(false);

        if (clothingQueue.Count > 0)
            clothingQueue[0].SetActive(true);
    }

    void Update()
    {
        if (taskComplete) return;

        //progress through clothing items
        if (currentItemIndex < clothingQueue.Count && !clothingQueue[currentItemIndex].activeSelf)
        {
            currentItemIndex++;
            if (currentItemIndex < clothingQueue.Count)
                clothingQueue[currentItemIndex].SetActive(true);
        }

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

            if (uiWithHints != null) uiWithHints.SetActive(false);
            if (finalUi != null) finalUi.SetActive(true);
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
