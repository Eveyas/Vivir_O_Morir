using UnityEngine;

public class Respawn : MonoBehaviour
{
    [Header("Altura del piso")]
    public float floorY;   // Altura donde debe respawnear el jugador

    [Header("Evita que aparezca dentro del piso")]
    public float offsetY = 0.3f; // Puedes ajustarlo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Guardar rigidbody
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            // Resetear velocidad para evitar rebotes
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            // Nueva posición
            Vector3 pos = collision.transform.position;

            // Mantener X, corregir Y, agregar offset
            pos.y = floorY + offsetY;

            // Aplicar posición final
            collision.transform.position = pos;

            Debug.Log("Jugador respawneado en piso sin rebote.");
        }
    }
}
