using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public AudioClip coinSound;
    public int coinValue = 5;
    private ScoreManager scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioSource cameraAudio = Camera.main.GetComponent<AudioSource>();

        if (other.CompareTag("Player"))
        {
            cameraAudio.PlayOneShot(coinSound);
            scoreManager.AddScore(coinValue);
            Destroy(gameObject);
        }
    }
}
