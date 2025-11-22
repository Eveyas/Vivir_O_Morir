using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Rigidbody2D rb;
    public float knockbackForce = 7f;
    public float knockbackUp = 3f;

    public void TakeHit(Transform attacker)
    {
        Vector2 direction = (transform.position - attacker.position).normalized;

        rb.velocity = Vector2.zero;

        rb.AddForce(new Vector2(direction.x * knockbackForce,
                                knockbackUp),
                    ForceMode2D.Impulse);
    }
}
