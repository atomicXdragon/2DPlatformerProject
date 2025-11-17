using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeHandler : MonoBehaviour
{
    public int life = 3;
    public TextMeshProUGUI livesCounter;
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        UpdateLifeUI();
    }

    void Update()
    {
        if (life <= 0)
        {
            GameOver();
        }
    }

    private void UpdateLifeUI()
    {
        if (livesCounter != null)
        {
            livesCounter.text = "Lives: " + life;
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void LoseLife()
    {
        life--;
        UpdateLifeUI(); 
    }

    public void GameOver()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        playerController.speed = 0;
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            livesCounter.gameObject.SetActive(false);

            Invoke("RestartGame", 2f);
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        LevelHandler.currentLevel = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}