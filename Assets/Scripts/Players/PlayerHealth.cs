using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Rigidbody2D rb;
    public float knockbackForce = 10f;
    public float knockbackUp = 2f;

public void TakeHit(Transform attacker)
{
    float dir = transform.position.x < attacker.position.x ? -1 : 1;

    rb.linearVelocity = Vector2.zero;

    Vector2 force = new Vector2(dir * knockbackForce, knockbackUp);

    rb.AddForce(force, ForceMode2D.Impulse);

    Debug.Log("Golpe recibido. Dirección: " + dir + "  Fuerza: " + force);
}


}
