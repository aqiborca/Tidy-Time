using UnityEngine;

public class ChoresListManager : MonoBehaviour
{
    public GameObject choresListPanel;
    public PlayerScript playerMovement;
    public TimerScript timerScript;

    void Start()
    {
        if (timerScript == null)
        {
            timerScript = FindObjectOfType<TimerScript>();
        }
    }

    public void OpenChoreList()
    {
        if (choresListPanel != null)
        {
            choresListPanel.SetActive(true);
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                playerMovement.StopMovement();
            }
            // Time continues running (don't pause timer)
        }
        else
        {
            Debug.LogError("Chores List Panel is not assigned!");
        }
    }

    public void CloseChoreList()
    {
        if (choresListPanel != null)
        {
            choresListPanel.SetActive(false);
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
        else
        {
            Debug.LogError("Chores List Panel is not assigned!");
        }
    }
}