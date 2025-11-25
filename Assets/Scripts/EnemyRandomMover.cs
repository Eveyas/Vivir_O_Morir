using UnityEngine;
using System.Collections; // Necesario para la Corrutina

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
            // Mantiene el movimiento de huida
            rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);
            return;
        }

        // Movimiento aleatorio normal
        rb.linearVelocity = new Vector2(direccion * velocidadMax, rb.linearVelocity.y);
    }

    private void ScheduleNextCambio()
    {
        nextCambioTime = Time.time + Random.Range(cambioIntervalMin, cambioIntervalMax);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // *** LÓGICA CLAVE: Buscar la interfaz IKnockbackable ***
        // Esto encuentra al Jugador 1 o al Jugador 2, ya que ambos la implementan.
        var knockable = collision.collider.GetComponentInParent<IKnockbackable>();
        
        // Solo continuar si encontramos un objeto que puede recibir Knockback (J1 o J2)
        if (knockable != null)
        {
            // Obtenemos el Transform del objeto que choca para el cálculo de dirección.
            Transform playerTransform = collision.collider.GetComponentInParent<Transform>();
            
            // Calcular dirección relativa para aplicar knockback
            Vector2 dir = (playerTransform.position - transform.position);
            // Determinar la dirección de empuje (lejos del enemigo)
            float signoX = dir.x == 0f ? -Mathf.Sign(transform.localScale.x) : Mathf.Sign(dir.x);
            Vector2 knock = new Vector2(signoX * knockbackForce, knockbackUpForce);
            
            // Aplicar Knockback usando el método universal de la interfaz
            knockable.Knockback(knock, knockbackDuration);

            // Hacer que el enemigo se aleje del jugador
            Vector2 away = (transform.position - playerTransform.position).normalized;
            fleeDirection = away;
            if (!isFleeing)
            {
                StartCoroutine(Flee(knockbackDuration));
            }
        }
    }

    private IEnumerator Flee(float dur)
    {
        isFleeing = true;
        
        // Aplica la velocidad de huida inmediatamente.
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);
        }

        yield return new WaitForSeconds(dur);

        isFleeing = false;
        ScheduleNextCambio(); // Vuelve a la rutina de movimiento aleatorio
    }
}