using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 7f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1f;

    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator anim;

    private float attackPointDefaultX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        attackPointDefaultX = attackPoint.localPosition.x;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // FOLLOW
        if (distance < detectionRange && distance > attackRange)
        {
            FollowPlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // ATTACK
        if (distance <= attackRange)
        {
            TryAttack();
        }

        FlipSprite();
    }

    void FollowPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        anim?.SetBool("Run", true);
    }

    void FlipSprite()
    {
        bool lookingRight = player.position.x > transform.position.x;

        transform.localScale = lookingRight ? 
            new Vector3(1, 1, 1) : 
            new Vector3(-1, 1, 1);

        // 🔥 CAMBIAR LADO DEL ATTACKPOINT CON EL FLIP
        attackPoint.localPosition = new Vector3(
            lookingRight ? attackPointDefaultX : -attackPointDefaultX,
            attackPoint.localPosition.y,
            0
        );
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        anim?.SetTrigger("Attack"); // activar animación

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit != null)
        {
            hit.GetComponent<PlayerHealth>()?.TakeHit(transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
