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
    public GameObject victoryScreen;
    public GameObject victoryTrigger;
    private PlayerController playerController;
    private AudioManager audioManager;

    private bool gameOverMusicPlaying = false;
    public Transform respawnPoint;
    public bool gameEnded;

    public GameObject mouseNPC;
    public SpriteRenderer NPCSprite;
    public Sprite npcJump;
    private Rigidbody2D npcRb;
    public Sprite npcStand;

    public Sprite playerJump;
    public Sprite playerStand;
    private Rigidbody2D playerRb;
    public SpriteRenderer playerSprite;
    public ScoreManager scoreManager;
    private bool winTriggered = false;

    void Start()
    {
        npcRb = mouseNPC.GetComponent<Rigidbody2D>();
        playerRb = gameObject.GetComponent<Rigidbody2D>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == victoryTrigger)
        {
            WinScreen();
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
    public void WinScreen()
    {
        if (!winTriggered)
        {
            winTriggered = true;

            // Stop all movement instantly
            playerRb.linearVelocity = Vector2.zero;
            npcRb.linearVelocity = Vector2.zero;

            // Disable animators so they don't override sprites
            playerController.GetComponent<Animator>().enabled = false;

            // Set to standing sprites
            playerSprite.sprite = playerStand;
            NPCSprite.sprite = npcStand;

            playerController.enabled = false;
            StartCoroutine(WaitForGroundedThenWin());
        }
    }

    private float jumpForce = 7f;

    private IEnumerator WaitForGroundedThenWin()
    {
        while (Mathf.Abs(playerRb.linearVelocity.y) > 0.1f)
        {
            yield return null;
        }

        scoreManager.AddScore(100);
        gameEnded = true;
        StartCoroutine(ContinuousJumping());
        StartCoroutine(UpdateSprites());
        StartCoroutine(WaitForWinScreen());
    }

    private IEnumerator ContinuousJumping()
    {
        while (!victoryScreen.activeSelf)
        {
            // Change to jump sprites
            playerSprite.sprite = playerJump;
            NPCSprite.sprite = npcJump;

            npcRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            audioManager.PlaySFX(audioManager.jumpSound, 0.5f);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator UpdateSprites()
    {
        while (!victoryScreen.activeSelf)
        {
            // Only change to stand when they hit the ground (velocity is near zero or negative)
            if (playerRb.linearVelocity.y <= 0.1f)
            {
                playerSprite.sprite = playerStand;
            }

            if (npcRb.linearVelocity.y <= 0.1f)
            {
                NPCSprite.sprite = npcStand;
            }

            yield return null; // Check every frame
        }
    }

    private IEnumerator WaitForWinScreen()
    {
        yield return new WaitForSeconds(5f);
        victoryScreen.SetActive(true);
        playerController.GetComponent<PlayerInput>().enabled = false;
        playerController.enabled = false;
        audioManager.musicSource.Stop();
        audioManager.PlaySFX(audioManager.victorySound, 0.5f);
        Invoke("RestartGame", 3f);
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

