using TMPro;
using UnityEngine;
public class LevelHandler : MonoBehaviour
{
    public Transform nextLevel;
    public Transform respawnPoint;
    private CameraController cam;
    public TextMeshProUGUI levelCounter;
    public static int currentLevel = 0;
    public AudioManager audioManager;
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();

        levelCounter.text = currentLevel.ToString();
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            {
                if (gameManager.life >= 1)
                {
                    audioManager.PlaySFX(audioManager.checkPointSound, 0.5f);
                }
                collision.transform.position = respawnPoint.position;
                cam.MoveToNewRoom(nextLevel);
                currentLevel++;
                levelCounter.text = currentLevel.ToString();
                Destroy(gameObject);
            }
        }
    }
}