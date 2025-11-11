using UnityEngine;
using UnityEngine.InputSystem;

// Asegúrate que el nombre del archivo C# sea 'Player1_Movimiento.cs'
public class Player1_Movimiento : MonoBehaviour
{
    // --- Físicas y Velocidades ---
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 15f;
    
    // --- Referencias de Componentes ---
    [Header("Referencias (Asignar en Inspector)")]
    // Rigidbody 2D del jugador
    public Rigidbody2D rb; 
    // Objeto hijo vacío para chequear el suelo
    public Transform checkSuelo; 
    // La Layer de tus plataformas/suelos (ej: Piso)
    public LayerMask capaDelSuelo; 

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
        // Detección de suelo: Crea un círculo en la posición del objeto 'checkSuelo'
        if (checkSuelo != null)
        {
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioCheckSuelo, capaDelSuelo);
        }
        
        // 📢 DEBUG CRÍTICO: Muestra en la consola si el juego cree que estás en el suelo.
        // Si no salta, este mensaje debe decir 'False'.
        Debug.Log("¿Está en suelo? " + estaEnSuelo); 
    }

    void FixedUpdate()
    {
        // MOVIMIENTO HORIZONTAL
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    // --- MÉTODOS DE INPUT SYSTEM (Player Input - Send Messages) ---
    
    // Función llamada por la acción 'Move'
    public void OnMove(InputValue value)
    {
        inputHorizontal = value.Get<Vector2>().x;
    }

    // Función llamada por la acción 'Jump' (Asignada a 'W' en tu Input Asset)
    public void OnJump(InputValue value)
    {
        // ⬆️ LÓGICA DE SALTO
        if (value.isPressed && estaEnSuelo)
        {
            // Debug para confirmar que el input llega
            Debug.Log("¡Input de Salto Recibido y En Suelo! Saltando...");
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
        else if (value.isPressed && !estaEnSuelo)
        {
            // Debug para ver si presionas W fuera del suelo
             Debug.Log("¡Input de Salto Recibido, PERO NO ESTÁ EN SUELO!");
        }
    }
}