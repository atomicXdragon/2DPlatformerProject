using System.Collections;
using TMPro;
using UnityEngine;
public class KillPlayer : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    public GameManager GameManager;
    public float respawnDelay = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer(collision.gameObject));
            GameManager.LoseLife();
        }
    }
    void Start()
    {
        respawnPoint = transform.parent.Find("RespawnPoint");
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        sprite.enabled = false;

        yield return new WaitForSeconds(respawnDelay); 
        player.transform.position = respawnPoint.position;

        sprite.enabled = true;
        rb.simulated = true;

    }
}