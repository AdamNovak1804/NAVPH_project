using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    // Update is called once per frame
    public bool isPaused = false;

    public GameObject pauseMenu;

    private void Start()
    {
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
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
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

    }

    public void RestartLevel()
    {
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {

    }
}
