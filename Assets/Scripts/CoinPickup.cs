using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public AudioClip coinSound;
    public int coinValue = 5;
    private ScoreManager scoreManager;

    AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        audioManager = FindFirstObjectByType<AudioManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if (other.CompareTag("Player"))
        {
            audioManager.PlaySFX(audioManager.coinPickup, 0.8f);
            scoreManager.AddScore(coinValue);
            Destroy(gameObject);
        }
    }
}
