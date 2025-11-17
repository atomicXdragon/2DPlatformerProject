using TMPro;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public Transform previousLevel;
    public Transform nextLevel;
    public Transform respawnPoint;

    private CameraController cam;
    public TextMeshProUGUI levelCounter;
    public static int currentLevel = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
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
                KillPlayer killPlayer = FindFirstObjectByType<KillPlayer>();
                if (killPlayer != null && respawnPoint != null)
                {
                    killPlayer.respawnPoint = respawnPoint;
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
