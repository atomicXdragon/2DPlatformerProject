using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class GameManager : MonoBehaviour
{
    public int life = 3;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;


    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI startText;
    public bool gameRunning;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    private PlayerController playerController;
    private AudioManager audioManager;

    private bool gameOverMusicPlaying = false;
    public Transform respawnPoint;
    public bool gameEnded;

    void Start()
    {
        gameEnded = false;
        transform.position = respawnPoint.position;
        playerController = FindFirstObjectByType<PlayerController>();
        audioManager = FindFirstObjectByType<AudioManager>();

        playerController.enabled = false;

        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        UpdateLifeUI();
        gameRunning = false;

        audioManager.PlayMusic(audioManager.titleMusic); 

    }

    void Update()
    {
        if (gameRunning == false && (Keyboard.current.anyKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.allControls.Any(x => x is ButtonControl btn && btn.wasPressedThisFrame))))
        {
            StartCoroutine(WaitToStart());
        }
        if (life <= 0 && gameOverMusicPlaying == false)
        {
            GameOver();
            gameOverMusicPlaying = true;
        }
    }

    private void UpdateLifeUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < life)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (gameOverText != null)
            {
                gameOverScreen.gameObject.SetActive(false);
            }
        }
    }

    public void LoseLife()
    {
        life--;
        UpdateLifeUI();
    }

    public void GameOver()
    {
        gameEnded = true;
        playerController.GetComponent<PlayerInput>().enabled = false;
        playerController.enabled = false;
        audioManager.musicSource.Stop();
        audioManager.PlaySFX(audioManager.gameOverSound, 0.5f);

        if (gameOverText != null)
        {
            gameOverScreen.gameObject.SetActive(true);
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
        audioManager.PlayMusic(audioManager.gameMusic);
    }

    IEnumerator WaitToStart()
    {
        gameRunning = true;
        yield return new WaitForSeconds(0.5f);
        startText.gameObject.SetActive(false);
        StartGame();
    }


}

