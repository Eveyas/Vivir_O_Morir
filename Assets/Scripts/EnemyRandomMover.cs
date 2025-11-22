using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRandomMover : MonoBehaviour
{
    [Header("Movimiento")] 
    public float velocidadMax = 2.5f;
    public float cambioIntervalMin = 1f;
    public float cambioIntervalMax = 3f;
    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpForce = 3f;
    public float knockbackDuration = 0.5f;

    private Rigidbody2D rb;
    private int direccion = 1; // -1 izquierda, 1 derecha
    private float nextCambioTime = 0f;
    private bool isFleeing = false;
    private Vector2 fleeDirection = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direccion = Random.value > 0.5f ? 1 : -1;
        ScheduleNextCambio();
    }

    void Update()
    {
        if (Time.time >= nextCambioTime)
        {
            // Cambiar dirección aleatoriamente
            direccion = Random.value > 0.5f ? 1 : -1;
            ScheduleNextCambio();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        if (isFleeing)
        {
            rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direccion * velocidadMax, rb.linearVelocity.y);
    }

    private void ScheduleNextCambio()
    {
        nextCambioTime = Time.time + Random.Range(cambioIntervalMin, cambioIntervalMax);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Intentar obtener el componente de movimiento del jugador en el objeto chocado
        var player = collision.collider.GetComponentInParent<Player1_Movimiento>();
        if (player != null)
        {
            // Calcular dirección relativa para aplicar knockback
            Vector2 dir = (player.transform.position - transform.position);
            float signoX = dir.x == 0f ? -Mathf.Sign(transform.localScale.x) : Mathf.Sign(dir.x);
            Vector2 knock = new Vector2(signoX * knockbackForce, knockbackUpForce);
            player.Knockback(knock, knockbackDuration);

            // Hacer que el enemigo se aleje del jugador durante la duración del knockback
            Vector2 away = (transform.position - player.transform.position).normalized;
            fleeDirection = away;
            if (!isFleeing)
            {
                StartCoroutine(Flee(knockbackDuration));
            }
        }
    }

    private System.Collections.IEnumerator Flee(float dur)
    {
        isFleeing = true;
        // Mantener la velocidad de huida inmediatamente
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);
        }

        yield return new WaitForSeconds(dur);

        isFleeing = false;
        ScheduleNextCambio();
    }
}
