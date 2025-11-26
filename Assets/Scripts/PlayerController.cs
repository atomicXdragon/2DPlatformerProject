using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5f;
    public float dashSpeed = 10f;
    public float jumpForce = 3.5f;
    public float jumpMultiplier = 2.5f;

    private float lastDashDirection;
    float horizontalMovement;

    private bool isJumping;
    public bool isDashing = false;
    private bool canDash = true;
    public bool isBouncing;
    public float jumpCutMultiplier = 0.5f;


    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Vector2 boxSize;
    public float castDistance;
    private float dashTime = 0f;

    private bool facingRight;

    private Rigidbody2D rb;
    Animator animator;
    private AudioManager audioManager;

    public Sprite jumpSprite;
    public Sprite fallSprite;
    public SpriteRenderer spriteRenderer;

    private float coyoteTime = 8f / 60f;
    private float coyoteTimeCounter;

    private bool isPlayingWalkSound = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();
    }


    private void Update()
    {
        FlipSprite();

        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        if (!isGrounded() && rb.linearVelocity.y > 0)
        {
            // Manually set jump sprite
            spriteRenderer.sprite = jumpSprite;
            animator.enabled = false;
        }
        else if (!isGrounded() && rb.linearVelocity.y < 0)
        {
            // Manually set fall sprite
            spriteRenderer.sprite = fallSprite;
            animator.enabled = false;
        }
        else
        {
            // Run animation
            animator.enabled = true;
            animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void FixedUpdate()
    {
        if (dashTime > 0)
        {
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                isBouncing = false;
            }
        }

        if (!isDashing)
        {
            float currentSpeed = isBouncing ? speed * 0.5f : speed;
            rb.linearVelocity = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocity.y); // Movement
            if (horizontalMovement != 0 && isGrounded() && !isPlayingWalkSound)
            {
                StartCoroutine(WalkSoundTimer());
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime; // Apply fall gravity increase
            isJumping = false;
        }
        else if (rb.linearVelocity.y > 0 && !isJumping)
        {

            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime; // Apply reduced jump gravity
        }
    }

    public IEnumerator WalkSoundTimer()
    {
        isPlayingWalkSound = true;

        while (horizontalMovement != 0 && isGrounded() && !isDashing)
        {
            audioManager.PlaySFX(audioManager.walkSound, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }

        isPlayingWalkSound = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Read input value from InputActions
    }

    void FlipSprite()
    {
        if (horizontalMovement > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingRight = true;
        }
        else if (horizontalMovement < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (context.performed && coyoteTimeCounter > 0 && !isJumping && !isBouncing)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply jump force
            isJumping = true;
            audioManager.PlaySFX(audioManager.jumpSound, 0.5f);
        }

        if (context.canceled && isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier); // Cut jump short
            coyoteTimeCounter = 0;
            isJumping = false;
        }
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            float direction = horizontalMovement != 0 ? Mathf.Sign(horizontalMovement) : (facingRight ? 1f : -1f);

            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            isDashing = true;
            audioManager.PlaySFX(audioManager.dashSound, 0.5f);
            lastDashDirection = direction;
            dashTime = 0.25f;
            canDash = false;
            StartCoroutine(DashCooldown());
        }
    }

    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(0.5f); // 1 second cooldown
        canDash = true;
    }
    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.down, castDistance, groundLayer) || Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.down, castDistance, wallLayer)) // Detecting the ground 
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * castDistance, boxSize); // Drawing box within "Scene" for ease of debugging
    }

    public float bounceX = 10;
    public float bounceY = 8;
    public Vector2 pointOfContact;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && isDashing)
        {
            pointOfContact = collision.contacts[0].normal;
            if (pointOfContact.x != 0)
            {
                rb.linearVelocity = new Vector2((-lastDashDirection * bounceX), bounceY);
                dashTime = 0.25f;
                isBouncing = true;
                isJumping = false;
                audioManager.PlaySFX(audioManager.wallSound, 0.5f);
            }
        }
    }


    public void OnGameOver()
    {
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; // Disable player controls
    }
}
