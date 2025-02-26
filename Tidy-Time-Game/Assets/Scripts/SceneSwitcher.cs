/*
This class contains all functions that allows for switching between scenes
Attached to: Main Camera
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Objects
    public GameObject choresListPanel;
    public PlayerScript playerMovement;
    // Loads Game (From Start)
    public void PlayGame()
    {
        // Reset player position to x = -7, y= 1.5
        // Reset timer to 6 PM
        CloseChoreList();
        // Load bedroom scene
        SceneManager.LoadSceneAsync(1);
    }
    // Loads bedroom scene
    public void PlayBedroom()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(1);
    }
    // Loads homework scene
    public void PlayHomework()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(2);
    }
    // Loads fishbowl scene
    public void PlayFishbowl()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(3);
    }
    // Loads fishbowl scene
    public void PlayPlushies()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(4);
    }
    // Loads end scene
    public void CallMom()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(5);
    }
    // Quits application
    public void QuitGame()
    {
        Application.Quit();
    }
    // Player position setter
    private void SetPlayerPosition()
    {
        // Find the player object and save its position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DataManager.Instance.SetPlayerPosition(player.transform.position);
        }
    }
    public void OpenChoreList()
    {
        choresListPanel.SetActive(true);
        if (playerMovement != null)
        {
            playerMovement.enabled = false; // Disable player movement
            playerMovement.StopMovement(); // Call a method to stop movement
        }
    }

    public void CloseChoreList()
    {
        choresListPanel.SetActive(false);
        if (playerMovement != null)
        {
            playerMovement.enabled = true; // Enable player movement
        }
    }
}