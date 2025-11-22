using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player1_Movimiento : MonoBehaviour
{
    // --- Físicas y Velocidades ---
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 15f;

    // --- Referencias ---
    [Header("Referencias (Asignar en Inspector)")]
    public Rigidbody2D rb;
    public Transform checkSuelo;
    public LayerMask capaDelSuelo;
    public float gravityScale = 1f;
    public int maxSaltos = 1; // 1 = salto simple, 2 = doble salto, etc.

    // --- Respawn ---
    [Header("Configuración Respawn")]
    public Transform puntoRespawn;

    // --- Internos ---
    private float inputHorizontal;
    private bool estaEnSuelo;
    private bool estaMuerto = false;
    private bool estaAturdido = false;
    private int saltosUsados = 0;
    private Collider2D playerCollider;

    private const float radioCheckSuelo = 0.2f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = gravityScale;
            // Usar detección continua para colisiones más suaves
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Asignar un PhysicsMaterial2D sin fricción para evitar que el jugador "se pegue" en esquinas
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            var mat = new PhysicsMaterial2D("Player_NoFriction") { friction = 0f, bounciness = 0f };
            playerCollider.sharedMaterial = mat;
        }
    }

    void Start()
    {
        // Crear respawn automático si no existe
        if (puntoRespawn == null)
        {
            GameObject respawnObj = new GameObject("PuntoRespawnAuto");
            puntoRespawn = respawnObj.transform;
            puntoRespawn.position = transform.position;
        }
    }

    void Update()
    {
        if (estaMuerto || estaAturdido) return;
    }

    void FixedUpdate()
    {
        if (estaMuerto || estaAturdido) return;

        // --- Comprobación de suelo (física) ---
        if (checkSuelo != null)
        {
            // Usamos Physics2D.OverlapCircle para detectar el suelo
            bool enSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
            if (enSuelo && !estaEnSuelo)
            {
                // Acabó de tocar suelo: resetear contador de saltos
                saltosUsados = 0;
            }
            estaEnSuelo = enSuelo;
        }

        // --- Movimiento Horizontal ---
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
        }

        // --- Aplicar giro del Sprite ---
        Girar();
        
        // Evitar quedar pegado en esquinas: si estamos en suelo, el input es activo pero
        // la velocidad horizontal es casi cero, damos un pequeño empujón en la dirección del input.
        if (rb != null && estaEnSuelo && Mathf.Abs(inputHorizontal) > 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f && !estaAturdido)
        {
            rb.AddForce(new Vector2(inputHorizontal * velocidadMovimiento * 0.15f, 0f), ForceMode2D.Impulse);
        }
    }

    // -------------------------------------------------------------------
    // -------------------------- FUNCIÓN DE GIRO --------------------------
    // -------------------------------------------------------------------

    private void Girar()
    {
        // Solo aplica el giro si hay movimiento horizontal (inputHorizontal no es cero)
        if (inputHorizontal != 0)
        {
            // La función Mathf.Sign() devuelve 1 (si es positivo/derecha) o -1 (si es negativo/izquierda).
            float direccion = Mathf.Sign(inputHorizontal);

            // Obtenemos la escala actual
            Vector3 escalaActual = transform.localScale;

            // Establecemos la escala X al valor de la dirección, manteniendo la escala Y y Z
            escalaActual.x = direccion;
            
            // Aplicamos la nueva escala al Transform
            transform.localScale = escalaActual;
        }
    }

    // -------------------------------------------------------------------
    // -------------------------- INPUT SYSTEM ----------------------------
    // -------------------------------------------------------------------

    public void OnMove(InputValue value)
    {
        if (estaMuerto || estaAturdido) return;

        inputHorizontal = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (estaMuerto || estaAturdido) return;

        if (!value.isPressed) return;

        // Permitir salto si está en suelo o si quedan saltos disponibles
        if (estaEnSuelo || saltosUsados < maxSaltos)
        {
            Debug.Log("Salto DETECTADO");
            if (rb != null)
            {
                // Normalizar la velocidad vertical (ponerla a cero) antes de aplicar impulso para saltos consistentes
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            }
            saltosUsados++;
        }
        else
        {
            Debug.Log("Intentaste saltar pero NO está en suelo y no quedan saltos");
        }
    }

    // -------------------------------------------------------------------
    // ------------------------- MUERTE & RESPAWN -------------------------
    // -------------------------------------------------------------------

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;

        Debug.Log("¡Jugador ha muerto! Respawn en 1s...");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        rb.linearVelocity = Vector2.zero;
        inputHorizontal = 0f;

        Invoke(nameof(Respawn), 1f);
    }

    // -------------------------------------------------------------------
    // ---------------------------- ATURDIMIENTO --------------------------
    // -------------------------------------------------------------------

    public void Aturdir(float dur)
    {
        if (estaMuerto) return;

        // Iniciar coroutine de aturdimiento y limpiar la velocidad
        StopAllCoroutines();
        StartCoroutine(AturdirCoroutine(dur, true));
    }

    // Aplicar retroceso (knockback) y aturdir durante una duración
    public void Knockback(Vector2 velocidadKnockback, float dur)
    {
        if (estaMuerto) return;

        StopAllCoroutines();

        if (rb != null)
        {
            rb.linearVelocity = velocidadKnockback;
        }

        StartCoroutine(AturdirCoroutine(dur, false));
    }

    private IEnumerator AturdirCoroutine(float dur, bool clearVelocity)
    {
        estaAturdido = true;

        // Evitar que el jugador recupere la dirección previa al aturdimiento
        inputHorizontal = 0f;

        if (clearVelocity && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(dur);

        estaAturdido = false;
    }

    private void Respawn()
    {
        estaMuerto = false;

        if (puntoRespawn != null)
        {
            transform.position = puntoRespawn.position;
            Debug.Log("Respawneado en " + puntoRespawn.position);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        rb.linearVelocity = Vector2.zero;
    }

    public void CambiarPuntoRespawn(Transform nuevoPunto)
    {
        puntoRespawn = nuevoPunto;
        Debug.Log("Nuevo punto de respawn: " + nuevoPunto.position);
    }

    void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = estaEnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioCheckSuelo);
        }

        if (puntoRespawn != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(puntoRespawn.position, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}
