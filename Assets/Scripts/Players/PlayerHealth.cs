using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Rigidbody2D rb;
    public float knockbackForce = 10f;
    public float knockbackUp = 2f;

   public void TakeHit(Transform attacker)
{
    rb.linearVelocity = Vector2.zero;

    float dx = transform.position.x - attacker.position.x;

    float dir = dx > 0 ? 1 : -1; // Nunca da 0

    Vector2 force = new Vector2(dir * knockbackForce, knockbackUp);

    rb.AddForce(force, ForceMode2D.Impulse);

    Debug.Log("Knockback dir = " + dir + "   dx = " + dx);
}

}
