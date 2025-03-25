using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushGameManager : MonoBehaviour
{
    public static PlushGameManager Instance;
    public PlushHeadSwap[] heads; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CheckTaskCompletion()
    {
        bool allCorrect = true;

        foreach (PlushHeadSwap head in heads)
        {
            if (!head.IsCorrectlyPlaced())
            {
                Debug.Log($"{head.name} is not in the correct position.");
                allCorrect = false;
            }
            else
            {
                Debug.Log($"{head.name} is correctly placed.");
            }
        }

        if (allCorrect)
        {
            Debug.Log("All plushies matched correctly");

            Debug.Log("All plushies matched correctly!");

            //mark the plushie swap as completed
            ChoreManager choreManager = FindObjectOfType<ChoreManager>();
            if (choreManager != null)
            {
                choreManager.CompleteChore("SwapPlushies");
            }

        }
    }

}