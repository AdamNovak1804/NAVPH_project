using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishScreenController: MonoBehaviour
{
    // Start is called before the first frame update

    public PlayerStats stats;

    public TextMeshProUGUI score = null;
    public TextMeshProUGUI time = null;
    public TextMeshProUGUI enemies = null;
    public TextMeshProUGUI totalScore = null;

    public float setTime = 90;

    public Sprite[] medals;
    public Image medalImage;

    public string nextLevel;


    private void Awake()
    {
        Time.timeScale = 0f;
        score.text = "Score: " + stats.score.ToString();
        score.text = "Time: " + stats.time.ToString();
        score.text = "Enemies killed: " + stats.enemiesKilled.ToString();
        var total = ComputeScore(stats.score, stats.time, stats.enemiesKilled);
        score.text = "Total score: " + total.ToString();

        ShowSprite(total);
    }

    int ComputeScore(int score, float time, int enemies)
    {
        var enemyScore = (enemies == 0) ? 50f : (enemies / stats.maxEnemies) * 50; 
        return Mathf.RoundToInt(score/stats.maxScore * 50 +  setTime/time * 50 + enemyScore);
    }

    void ShowSprite(int total)
    {
        
        if (total < 90)
        {
            medalImage.sprite = medals[0];
        }
        else if (total < 120)
        {
            medalImage.sprite = medals[1];
        }
        else
        {
            medalImage.sprite = medals[2];
        }
    }

    void Continue()
    {
        // load scene next level
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
