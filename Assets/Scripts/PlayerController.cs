using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 3f;
    public float reducedJumpMultiplier = 1.5f;
    public float fallMultiplier = 2f;

    float horizontalMovement;

    public bool isGrounded;
    private bool isJumping;
    public float jumpCutMultiplier = 0.5f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y); // Movement

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime; // Apply fall gravity increase
        }
        else if (rb.linearVelocity.y > 0 && !isJumping)
        {

            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (reducedJumpMultiplier - 1) * Time.fixedDeltaTime; // Apply reduced jump gravity
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Read input value from InputActions
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded &&! isJumping && (rb.linearVelocity.y == 0))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply jump force
            isJumping = true;
            isGrounded = false;
        }

        if (context.canceled && isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier); // Cut jump short
            isJumping = false;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            isGrounded = true; 
        }
    }
}