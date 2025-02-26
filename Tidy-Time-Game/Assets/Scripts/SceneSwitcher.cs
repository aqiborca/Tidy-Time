/*
This class contains all functions that allows for switching between scenes
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Loads bedroom scene
    public void PlayBedroom()
    {
        Debug.Log("PlayBedroom called: Switching to the Bedroom scene.");
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
    // Loads end scene
    public void CallMom()
    {
        // Save player position before switching scenes
        SetPlayerPosition();
        SceneManager.LoadSceneAsync(4);
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
}