using UnityEngine;
using UnityEngine.InputSystem;

// IMPORTANTE: El nombre de la clase debe coincidir EXACTAMENTE con el nombre del archivo C#
public class Player2_Movimiento : MonoBehaviour
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

    // --- Respawn ---
    [Header("Configuración Respawn")]
    public Transform puntoRespawn;

    // --- Internos ---
    private float inputHorizontal;
    private bool estaEnSuelo;
    private bool estaMuerto = false;

    private const float radioCheckSuelo = 0.2f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Crear respawn automático si no existe
        if (puntoRespawn == null)
        {
            GameObject respawnObj = new GameObject("PuntoRespawnAuto_P2");
            puntoRespawn = respawnObj.transform;
            puntoRespawn.position = transform.position;
        }
    }

    void Update()
    {
        if (estaMuerto) return;

        // Detección de suelo
        if (checkSuelo != null)
        {
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;

        // --- Movimiento Horizontal ---
        // Nota: Si Unity te da error aquí, cambia rb.linearVelocity por rb.velocity
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // --- Aplicar giro del Sprite ---
        Girar();
    }

    // -------------------------------------------------------------------
    // -------------------------- FUNCIÓN DE GIRO --------------------------
    // -------------------------------------------------------------------

    private void Girar()
    {
        // Solo aplica el giro si hay movimiento horizontal (inputHorizontal no es cero)
        if (inputHorizontal != 0)
        {
            // La función Mathf.Sign() devuelve 1 (derecha) o -1 (izquierda).
            float direccion = Mathf.Sign(inputHorizontal);

            // Obtenemos la escala actual
            Vector3 escalaActual = transform.localScale;

            // Establecemos la escala X al valor de la dirección
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
        if (estaMuerto) return;

        // Usará Left Arrow (-1) o Right Arrow (1)
        inputHorizontal = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (estaMuerto) return;

        // Usará Up Arrow (Salto)
        if (value.isPressed && estaEnSuelo)
        {
            Debug.Log("P2: Salto DETECTADO y estaba en suelo");
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
        else if (value.isPressed && !estaEnSuelo)
        {
            Debug.Log("P2: Intentaste saltar pero NO está en suelo");
        }
    }

    // -------------------------------------------------------------------
    // ------------------------- MUERTE & RESPAWN -------------------------
    // -------------------------------------------------------------------

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;

        Debug.Log("P2: ¡Jugador ha muerto! Respawn en 1s...");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        rb.linearVelocity = Vector2.zero;
        inputHorizontal = 0f;

        Invoke(nameof(Respawn), 1f);
    }

    private void Respawn()
    {
        estaMuerto = false;

        if (puntoRespawn != null)
        {
            transform.position = puntoRespawn.position;
            Debug.Log("P2: Respawneado en " + puntoRespawn.position);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        rb.linearVelocity = Vector2.zero;
    }

    public void CambiarPuntoRespawn(Transform nuevoPunto)
    {
        puntoRespawn = nuevoPunto;
        Debug.Log("P2: Nuevo punto de respawn: " + nuevoPunto.position);
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