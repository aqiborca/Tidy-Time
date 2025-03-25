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