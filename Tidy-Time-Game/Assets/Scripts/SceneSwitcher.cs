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
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void LaunchBedroom()
    {
        // Reset all chores to incomplete
        ChoreManager choreManager = ChoreManager.Instance;
        if (choreManager != null)
        {
            choreManager.isAlphabetSoupCompleted = false;
            choreManager.isSwapPlushiesCompleted = false;
            choreManager.isMathHomeworkCompleted = false;
            choreManager.isGarbageCompleted = false;
            choreManager.isOrganizeClosetCompleted = false;
            choreManager.isFeedFishCompleted = false;
        }

        // Set the time to 4:00:00 PM every time LaunchBedroom is called
        TimerScript timer = FindObjectOfType<TimerScript>();
        if (timer != null)
        {
            timer.SetTime(4, 0, 0); // Set to 4 PM
            timer.RestartTimer(); // Restart the timer every time this method is called
        }

        // Load the Bedroom scene
        SceneManager.LoadSceneAsync(2);
    }

    public void PlayBedroom()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(2);
    }

    public void PlayHomework()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(3);
    }

    public void PlayFishbowl()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(4);
    }

    public void PlayPlushies()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(5);
    }

    public void PlaySoup()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(6);
    }

    public void PlayCloset()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(7);
    }

    public void PlayGarbage()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(8);
    }

    public void PlayUnderBed()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(9);
    }

    public void CallMom()
    {
        SetPlayerPosition();
        SaveTime();
        SceneManager.LoadSceneAsync(10);
    }

    public void LoadCredits()
    {
        SceneManager.LoadSceneAsync(11);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DataManager.Instance.SetPlayerPosition(player.transform.position);
        }
    }

    private void SaveTime()
    {
        TimerScript timer = FindObjectOfType<TimerScript>();
        if (timer != null)
        {
            DataManager.Instance.SetTime(timer.GetCurrentHour(), timer.GetCurrentMinute(), timer.GetCurrentSecond());
        }
    }
}