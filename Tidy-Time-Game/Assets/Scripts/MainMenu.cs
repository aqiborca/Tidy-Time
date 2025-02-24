using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// UnityScript
public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayHomework()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
{
    Application.Quit();
}
}