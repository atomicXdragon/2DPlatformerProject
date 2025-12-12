using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private GameObject pointA;
    private GameObject pointB;
    public float speed;
    public int health = 2;
    public float respawnDelay = 0.5f;
    private Transform respawnPoint;
    public BoxCollider2D normalHitbox;
    public BoxCollider2D bigHitbox;

    private Rigidbody2D rb;
    private Transform currentPoint;

    private GameManager gameManager;
    private AudioManager audioManager;
    private ScoreManager scoreManager;

    Animator animator;
    private PlayerController playerController;
    private float distanceToPoint;
    private bool hasSwapped = false;
    public Sprite hurtSprite;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = FindFirstObjectByType<PlayerController>();
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        animator = GetComponent<Animator>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        normalHitbox.isTrigger = false;
        bigHitbox.isTrigger = true;
        bigHitbox.enabled = false;

        respawnPoint = transform.parent.parent.Find("RespawnPoint");

        pointA = transform.parent.Find("PointA").gameObject;
        pointB = transform.parent.Find("PointB").gameObject;

        currentPoint = pointB.transform;

    }

    void FixedUpdate()
    {
        if (health == 2)
        {
            Patrol();
        }
        else if (health == 1)
        {
            rb.linearVelocity = Vector2.zero;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerController.isDashing)
        {
            health -= 1;
            if (health == 1 && !hasSwapped)
            {
                audioManager.PlaySFX(audioManager.armourBreak, 0.5f);
                hasSwapped = true;
                animator.enabled = false;
                spriteRenderer.sprite = hurtSprite;

                StartCoroutine(SwitchToBigHitbox());
            }
        }
        else if (collision.gameObject.CompareTag("Player") && !playerController.isDashing)
        {
            if (gameManager.life > 1)
            {
                audioManager.PlaySFX(audioManager.deathSound, 0.5f);
            }
            StartCoroutine(RespawnPlayer(collision.gameObject));
            gameManager.LoseLife();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerController.isDashing)
            {
                health -= 1;
                if (health <= 0)
                {
                    audioManager.PlaySFX(audioManager.robotBreak, 0.5f);
                    scoreManager.AddScore(10);
                    Destroy(gameObject);
                }
            }
            else
            {
                // Kill player if not dashing
                if (gameManager.life > 1)
                {
                    audioManager.PlaySFX(audioManager.deathSound, 0.5f);
                }
                StartCoroutine(RespawnPlayer(collision.gameObject));
                gameManager.LoseLife();
            }
        }
    }

    private IEnumerator SwitchToBigHitbox()
    {
        yield return new WaitForSeconds(0.2f);
        normalHitbox.enabled = false;
        bigHitbox.enabled = true;
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
}