using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;
    public GameManager gameManager;
    public GameObject pauseCameraCollider;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        pauseCameraCollider.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        gameManager.isGamePaused = false;
    }

    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        pauseCameraCollider.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        gameManager.isGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        gameManager.isGamePaused = false;
        GameIsPaused = false;
        gameManager.ManualSave();
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting..");
        Application.Quit();
    }
}
