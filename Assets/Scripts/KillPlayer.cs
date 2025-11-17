using TMPro;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    public LifeHandler lifeHandler;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = respawnPoint.position;
            lifeHandler.LoseLife();
        }
    }

    void Start()
    {
        player.transform.position = respawnPoint.position;
    }
}

