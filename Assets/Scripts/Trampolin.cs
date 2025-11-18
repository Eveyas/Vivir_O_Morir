using UnityEngine;

public class Trampolin : MonoBehaviour
{
    public float fuerzaSalto = 20f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            }
        }
    }
}
