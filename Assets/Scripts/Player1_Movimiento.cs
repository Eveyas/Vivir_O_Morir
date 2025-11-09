using UnityEngine;
using UnityEngine.InputSystem;

// IMPORTANTE: El nombre de la clase debe COINCIDIR con el nombre del archivo.
public class Player1_Movimiento : MonoBehaviour
{
    // --- Físicas y Velocidades ---
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 15f;
    
    // --- Referencias de Componentes ---
    [Header("Referencias (Asignar en Inspector)")]
    public Rigidbody2D rb;
    public Transform checkSuelo; // Punto bajo los pies del personaje
    public LayerMask capaDelSuelo; // La Layer de tus plataformas/suelos

    // --- Variables de Estado ---
    private float inputHorizontal;
    private bool estaEnSuelo;
    private const float radioCheckSuelo = 0.2f;

    void Awake()
    {
        // Obtener el Rigidbody2D si no está asignado
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // Detección de suelo usando un círculo pequeño
        if (checkSuelo != null)
        {
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
        }
    }

    void FixedUpdate()
    {
        // Aplicar la velocidad de movimiento horizontal
        // Usamos rb.velocity para un movimiento suave
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    // --- MÉTODOS DE INPUT SYSTEM (Player Input - Send Messages) ---
    
    // Función llamada por la acción 'Move' (debe coincidir con el nombre de tu acción)
    public void OnMove(InputValue value)
    {
        // Obtener el valor horizontal (X) del Vector2 Composite (WASD)
        inputHorizontal = value.Get<Vector2>().x;
    }

    // Función llamada por la acción 'Jump'
    public void OnJump(InputValue value)
    {
        // Solo saltar si se presiona la tecla Y si el jugador está en el suelo
        if (value.isPressed && estaEnSuelo)
        {
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }
}