using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushGameManager : MonoBehaviour
{
    public static PlushGameManager Instance;
    public Transform[] correctHeadPositions; 
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
        float snapThreshold = 0.2f; 

        for (int i = 0; i < heads.Length; i++)
        {
            
            if (Vector3.Distance(heads[i].transform.position, correctHeadPositions[i].position) > snapThreshold)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("All plushies matched correctly");
            Invoke("ReturnToBedroom", 2f);
        }
        else
        {
            Debug.Log("Not all plushies are in the correct spots.");
        }
    }

    void ReturnToBedroom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Bedroom");
    }
}
