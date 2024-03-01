using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void Reset()
    {
        SaveSystem.DeleteGameSave();
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        Time.timeScale = 1f;
    }
}
