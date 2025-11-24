using UnityEngine;

public class CarroInteractuable : MonoBehaviour
{
    // --- Configuración en el Inspector ---
    [Header("Configuración del Carro")]
    public float velocidadCarro = 15f;          // Velocidad de movimiento del carro
    public float distanciaRecorrido = 10f;      // Distancia que el carro se moverá
    public float tiempoRetrasoAparecer = 0.5f;  // Retraso antes de que el jugador reaparezca
    public string tagJugador = "Player";        // Tag que deben tener los jugadores

    [Header("Referencias")]
    public Rigidbody2D rbCarro;                 // Rigidbody2D del carro (Tipo Kinematic o Dynamic)
    private GameObject playerActualEnCarro;      // Referencia al jugador que está siendo transportado

    // --- Variables de Estado Interno ---
    private Vector3 puntoInicioRecorrido;
    private bool carroActivo = false;
    private bool playerDesaparecido = false;

    void Awake()
    {
        // Obtener el Rigidbody2D automáticamente si no está asignado
        if (rbCarro == null)
        {
            rbCarro = GetComponent<Rigidbody2D>();
            if (rbCarro == null)
            {
                Debug.LogError("CarroInteractuable: Se requiere Rigidbody2D en el carro.", this);
                enabled = false;
            }
        }
    }

    // --- Detección de Interacción ---
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Debe ser un jugador (Tag)
        // 2. El carro no debe estar en movimiento (carroActivo)
        // Nota: Asegúrate que el Collider del carro tenga marcado 'Is Trigger'
        if (!carroActivo && other.CompareTag(tagJugador))
        {
            playerActualEnCarro = other.gameObject;
            ActivarCarro();
        }
    }

    // --- Lógica de Movimiento ---
    void FixedUpdate()
    {
        if (carroActivo && playerActualEnCarro != null)
        {
            // Mueve el carro
            rbCarro.linearVelocity = new Vector2(velocidadCarro, rbCarro.linearVelocity.y);

            // Calcula la distancia recorrida
            float distanciaActual = Vector3.Distance(puntoInicioRecorrido, transform.position);

            // Si el carro ha recorrido la distancia deseada, detente
            if (distanciaActual >= distanciaRecorrido)
            {
                DetenerCarro();
            }
        }
    }

    // --- Fases de la Interacción ---

    void ActivarCarro()
    {
        carroActivo = true;
        puntoInicioRecorrido = transform.position; // Marca el punto de partida

        // 1. El jugador desaparece y se detiene
        if (playerActualEnCarro != null)
        {
            playerActualEnCarro.SetActive(false); // Desactiva el GameObject del jugador
            playerDesaparecido = true;
            
            // Detén cualquier velocidad residual del jugador
            Rigidbody2D playerRb = playerActualEnCarro.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }
            Debug.Log($"Jugador {playerActualEnCarro.name} subió al carro y desapareció.");
        }
    }

    void DetenerCarro()
    {
        carroActivo = false;
        rbCarro.linearVelocity = Vector2.zero; // Detiene el movimiento del carro
        
        Debug.Log("Carro se detuvo.");

        // Llama a la función para reaparecer al jugador con un pequeño retraso
        if (playerDesaparecido && playerActualEnCarro != null)
        {
            Invoke("ReaparecerJugador", tiempoRetrasoAparecer);
        }
    }

    private void ReaparecerJugador()
    {
        if (playerActualEnCarro != null)
        {
            // 2. Reaparece el jugador encima del carro
            playerActualEnCarro.transform.position = transform.position + new Vector3(0, 1f, 0); // Reaparece un poco más arriba
            playerActualEnCarro.SetActive(true); // Activa el GameObject del jugador
            playerDesaparecido = false;
            
            // Asegúrate de que el jugador inicie sin velocidad
            Rigidbody2D playerRb = playerActualEnCarro.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            Debug.Log($"Jugador {playerActualEnCarro.name} bajó del carro y reapareció.");
        }

        // 💥 SOLUCIÓN DE USO ÚNICO: Destruye el objeto Carro para que no pueda ser usado más.
        Destroy(gameObject); 
    }

    // --- Visualización en el Editor (Gizmos) ---
    void OnDrawGizmos()
    {
        if (carroActivo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(puntoInicioRecorrido, 0.5f);
            Gizmos.color = Color.blue;
            // Dibuja una línea desde el inicio hasta la posición actual para ver el recorrido
            Gizmos.DrawLine(puntoInicioRecorrido, transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}