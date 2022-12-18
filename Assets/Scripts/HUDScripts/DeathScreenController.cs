using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : MonoBehaviour
{
    // Start is called before the first frame update
    public void Awake()
    {
        Time.timeScale = 0f;
    }
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
