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
    
    // --- Configuración Respawn ---
    [Header("Configuración Respawn")]
    public Transform puntoRespawn;

    // --- Variables internas ---
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
        // Si no hay punto de respawn asignado, usar posición inicial
        if (puntoRespawn == null)
        {
            // Crear un punto de respawn automáticamente en la posición inicial
            GameObject respawnObj = new GameObject("PuntoRespawnAuto");
            puntoRespawn = respawnObj.transform;
            puntoRespawn.position = transform.position;
        }
    }

    void Update()
    {
        // Si está muerto, no procesar movimientos
        if (estaMuerto) return;

        // Detección de suelo: Crea un círculo en la posición del objeto 'checkSuelo'
        if (checkSuelo != null)
        {
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
        }
        
        Debug.Log("¿Está en suelo? " + estaEnSuelo); 
    }

    void FixedUpdate()
    {
        // Si está muerto, no aplicar movimiento físico
        if (estaMuerto) return;

        // MOVIMIENTO HORIZONTAL
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    // --- Input System (Send Messages) ---
    public void OnMove(InputValue value)
    {
        if (estaMuerto) return; // No mover si está muerto
        
        inputHorizontal = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (estaMuerto) return; // No saltar si está muerto
        
        // ⬆️ LÓGICA DE SALTO
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

    // --- SISTEMA DE MUERTE Y RESPAWN ---
    
    // Método público para que los picos llamen a este método
    public void Morir()
    {
        if (estaMuerto) return; // Evitar múltiples muertes
        
        estaMuerto = true;
        Debug.Log("¡Jugador ha muerto! Respawn en 1 segundo...");
        
        // Desactivar colisión temporalmente para evitar problemas durante el respawn
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        // Detener todo movimiento
        rb.linearVelocity = Vector2.zero;
        inputHorizontal = 0f;
        
        // Iniciar respawn después de un breve delay
        Invoke(nameof(Respawn), 1f);
    }
    
    // Método para revivir al jugador
    private void Respawn()
    {
        estaMuerto = false;
        
        // Restaurar posición al punto de respawn
        if (puntoRespawn != null)
        {
            transform.position = puntoRespawn.position;
            Debug.Log("Jugador respawneado en: " + puntoRespawn.position);
        }
        else
        {
            Debug.LogWarning("No hay punto de respawn asignado!");
        }
        
        // Reactivar colisión
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
            
        // Resetear velocidad
        rb.linearVelocity = Vector2.zero;
    }
    
    // Método opcional para cambiar el punto de respawn durante el juego
    public void CambiarPuntoRespawn(Transform nuevoPunto)
    {
        puntoRespawn = nuevoPunto;
        Debug.Log("Nuevo punto de respawn establecido: " + nuevoPunto.position);
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar el área de detección de suelo
        if (checkSuelo != null)
        {
            Gizmos.color = estaEnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioCheckSuelo);
        }
        
        // Dibujar el punto de respawn si está asignado
        if (puntoRespawn != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(puntoRespawn.position, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.DrawLine(transform.position, puntoRespawn.position);
        }
    }
}
