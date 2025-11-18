using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5f;
    public float dashSpeed = 10f;
    public float jumpForce = 3f;
    public float jumpMultiplier = 2.5f;

    private float lastDashDirection;
    float horizontalMovement;

    private bool isJumping;
    private bool isDashing = false;
    private bool canDash = true;
    public bool isBouncing;
    public float jumpCutMultiplier = 0.5f;
    

    public LayerMask groundLayer;
    public Vector2 boxSize;
    public float castDistance;
    private float dashTime = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
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

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Read input value from InputActions
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded() && !isJumping && !isBouncing)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply jump force
            isJumping = true;
        }

        if (context.canceled && isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier); // Cut jump short
            isJumping = false;
        }
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            float direction = horizontalMovement != 0 ? Mathf.Sign(horizontalMovement) : 1f;
            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            isDashing = true;
            lastDashDirection = direction;
            dashTime = 0.2f; 
            canDash = false;
            StartCoroutine(DashCooldown());
        }
    }

    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(1f); // 1 second cooldown
        canDash = true;
    }
    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.down, castDistance, groundLayer)) // Detecting the ground 
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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && isDashing)
        {
            Debug.Log("Hit wall during dash.");
            rb.linearVelocity = new Vector2((-lastDashDirection * bounceX), bounceY);
            dashTime = 0.2f;
            isBouncing = true;
            isJumping = false;
        }
    }

    public void OnGameOver()
    {
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; // Disable player controls
    }
}
