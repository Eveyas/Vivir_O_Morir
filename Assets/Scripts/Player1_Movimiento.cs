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
            GameObject respawnObj = new GameObject("PuntoRespawnAuto");
            puntoRespawn = respawnObj.transform;
            puntoRespawn.position = transform.position;
        }
    }

    void Update()
    {
        if (estaMuerto) return;

        if (checkSuelo != null)
        {
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;

        // --- Movimiento Horizontal ---
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    // -------------------------------------------------------------------
    // -------------------------- INPUT SYSTEM ----------------------------
    // -------------------------------------------------------------------

    public void OnMove(InputValue value)
    {
        if (estaMuerto) return;

        inputHorizontal = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (estaMuerto) return;

        if (value.isPressed && estaEnSuelo)
        {
            Debug.Log("Salto DETECTADO y estaba en suelo");
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
        else if (value.isPressed && !estaEnSuelo)
        {
            Debug.Log("Intentaste saltar pero NO está en suelo");
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
