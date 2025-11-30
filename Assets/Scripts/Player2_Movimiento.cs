using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2_Movimiento : MonoBehaviour, IKnockbackable // <-- ¡Implementación de IKnockbackable!
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
    public int maxSaltos = 1; 

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

    [Header("Efecto Visual de Parálisis")]
    public SpriteRenderer spriteRenderer;
    public Color paralizadoColor = new Color(0.3f, 0.5f, 1f, 1f); // Azul suave
    private Color normalColor;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = gravityScale;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            var mat = new PhysicsMaterial2D("Player_NoFriction") { friction = 0f, bounciness = 0f };
            playerCollider.sharedMaterial = mat;
        }

        if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
    }

    void Start()
    {
        if (puntoRespawn == null)
        {
            GameObject respawnObj = new GameObject("PuntoRespawnAuto_J2");
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
            bool enSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
            if (enSuelo && !estaEnSuelo)
            {
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
        
        if (rb != null && estaEnSuelo && Mathf.Abs(inputHorizontal) > 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f && !estaAturdido)
        {
            rb.AddForce(new Vector2(inputHorizontal * velocidadMovimiento * 0.15f, 0f), ForceMode2D.Impulse);
        }
    }

    // -------------------------- FUNCIÓN DE GIRO --------------------------

    private void Girar()
    {
        if (inputHorizontal != 0)
        {
            float direccion = Mathf.Sign(inputHorizontal);
            Vector3 escalaActual = transform.localScale;
            escalaActual.x = direccion;
            transform.localScale = escalaActual;
        }
    }

    // -------------------------- INPUT SYSTEM ----------------------------

    public void OnMove(InputValue value)
    {
        if (estaMuerto || estaAturdido) return;

        // Asume que este PlayerAction está configurado para el JUGADOR 2
        inputHorizontal = value.Get<Vector2>().x; 
    }

    public void OnJump(InputValue value)
    {
        if (estaMuerto || estaAturdido) return;

        if (!value.isPressed) return;

        if (estaEnSuelo || saltosUsados < maxSaltos)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            }
            saltosUsados++;
        }
    }

    // ------------------------- MUERTE & RESPAWN -------------------------

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        rb.linearVelocity = Vector2.zero;
        inputHorizontal = 0f;

        Invoke(nameof(Respawn), 1f);
    }

    // ---------------------------- ATURDIMIENTO --------------------------
    
    // Este método es requerido por la interfaz IKnockbackable
    public void Knockback(Vector2 velocidadKnockback, float dur)
    {
        if (estaMuerto) return;

        StopAllCoroutines();

        if (rb != null)
        {
            rb.linearVelocity = velocidadKnockback;
        }

        // Reutiliza la corrutina de aturdimiento para manejar la duración del Knockback
        StartCoroutine(AturdirCoroutine(dur, false)); 
    }

    private IEnumerator AturdirCoroutine(float dur, bool clearVelocity)
    {
        estaAturdido = true;

        // Desactivar movimiento
        inputHorizontal = 0f;

        // Limpiar velocidad si se requiere
        if (clearVelocity && rb != null)
            rb.linearVelocity = Vector2.zero;

        // ACTIVAR EFECTO VISUAL
        if (spriteRenderer != null)
            spriteRenderer.color = paralizadoColor;

        yield return new WaitForSeconds(dur);

        // DESACTIVAR EFECTO VISUAL
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        estaAturdido = false;
    }

    private void Respawn()
    {
        estaMuerto = false;

        if (puntoRespawn != null)
        {
            transform.position = puntoRespawn.position;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        rb.linearVelocity = Vector2.zero;
    }

    public void CambiarPuntoRespawn(Transform nuevoPunto)
    {
        puntoRespawn = nuevoPunto;
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