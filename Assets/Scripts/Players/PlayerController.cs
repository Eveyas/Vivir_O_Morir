using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Identidad")]
    public int playerNumber = 1; // 1 o 2

    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 crouchColliderSize = new Vector2(1f, 0.6f);

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.4f;

    Rigidbody2D rb;
    Collider2D col;
    Animator animator;
    float horizontal;
    bool canFire = true;
    Vector2 originalColliderSize;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        if (col != null) originalColliderSize = col.bounds.size;
    }

    void Update()
    {
        // Ejes y botones por playerNumber: Horizontal1, Jump1, Fire1, Crouch1
        horizontal = Input.GetAxis("Horizontal" + playerNumber);
        if (Input.GetButtonDown("Jump" + playerNumber) && IsGrounded()) Jump();
        if (Input.GetButton("Crouch" + playerNumber)) Crouch(true); else Crouch(false);

        if (Input.GetButtonDown("Fire" + playerNumber)) Fire();

        // Animaciones
        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
            animator.SetBool("Grounded", IsGrounded());
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Crouch(bool enable)
    {
        if (enable)
        {
            // ejemplo simple: reducir collider (si es BoxCollider2D)
            var box = col as BoxCollider2D;
            if (box) box.size = crouchColliderSize;
            if (animator) animator.SetBool("Crouch", true);
        }
        else
        {
            var box = col as BoxCollider2D;
            if (box) box.size = originalColliderSize;
            if (animator) animator.SetBool("Crouch", false);
        }
    }

    void Fire()
    {
        if (!canFire || bulletPrefab == null || firePoint == null) return;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        StartCoroutine(FireCooldown());
    }

    IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}
