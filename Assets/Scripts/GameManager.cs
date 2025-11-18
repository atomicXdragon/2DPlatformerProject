using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int life = 3;
    public TextMeshProUGUI livesCounter;
    public TextMeshProUGUI gameOverText;
    public bool gameRunning;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    private PlayerController playerController;

    public Transform respawnPoint;


    void Start()
    {
        transform.position = respawnPoint.position;
        playerController = FindFirstObjectByType<PlayerController>();
        playerController.enabled = false;

        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        UpdateLifeUI();
        gameRunning = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && gameRunning == false)
        {
            StartGame();
            gameRunning = true;
        }
        if (life <= 0)
        {
            GameOver();
        }
    }

    private void UpdateLifeUI()
    {
        if (livesCounter != null)
        {
            livesCounter.text = "LIVES: " + life;
        }

        if (gameOverText != null)
        {
            gameOverScreen.gameObject.SetActive(false);
        }
    }

    public void LoseLife()
    {
        life--;
        UpdateLifeUI(); 
    }

    public void GameOver()
    {
        playerController.OnGameOver();

        if (gameOverText != null)
        {
            gameOverScreen.SetActive(true);
            livesCounter.gameObject.SetActive(false);

            Invoke("RestartGame", 2f);
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        LevelHandler.currentLevel = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void StartGame()
    {
        titleScreen.SetActive(false);
        playerController.enabled = true;


    }
}