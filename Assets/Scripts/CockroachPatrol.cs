using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class CockroachPatrol : MonoBehaviour
{
    private GameObject pointA;
    private GameObject pointB;
    public float speed;
    public BoxCollider2D body;

    public float respawnDelay = 0.5f;
    private Transform respawnPoint;
    private float distanceToPoint;
    private bool isAlive;
    private Rigidbody2D rb;
    private Transform currentPoint;
    private GameManager gameManager;
    private AudioManager audioManager;
    private ScoreManager scoreManager;
    public SpriteRenderer spriteRenderer;
    public Sprite squashSprite;
    Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        animator = GetComponent<Animator>();

        respawnPoint = transform.parent.parent.Find("RespawnPoint");

        pointA = transform.parent.Find("PointA").gameObject;
        pointB = transform.parent.Find("PointB").gameObject;

        currentPoint = pointB.transform;
        isAlive = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAlive)
        {
            Patrol();
        }
    }
    void Patrol()
    {
        distanceToPoint = Vector2.Distance(transform.position, currentPoint.position);

        if (currentPoint == pointB.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }

        if (distanceToPoint < 0.5f)
        {
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
            FlipSprite();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioManager.PlaySFX(audioManager.robotBreak, 0.5f);
            scoreManager.AddScore(50);
            StartCoroutine(CockroachSquish());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioManager.PlaySFX(audioManager.deathSound, 0.5f);
            StartCoroutine(RespawnPlayer(collision.gameObject));
            gameManager.LoseLife();
        }
    }
    private IEnumerator RespawnPlayer(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();

        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;
        sprite.enabled = false;

        if (gameManager.life > 0)
        {
            yield return new WaitForSeconds(respawnDelay);
            player.transform.position = respawnPoint.position;
            sprite.enabled = true;
            playerRb.simulated = true;
        }
    }
    void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public Rigidbody2D playerRb;
    public float bounce = 2f;

    IEnumerator CockroachSquish()
    {
        playerRb.linearVelocity = new Vector2(0, bounce);
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        animator.enabled = false;
        spriteRenderer.sprite = squashSprite;
        yield return new WaitForSeconds(0.5f);
        body.enabled = false;
        Destroy(gameObject);
    }
}
