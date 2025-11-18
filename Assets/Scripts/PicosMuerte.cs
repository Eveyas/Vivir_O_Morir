using UnityEngine;

public class PicosMuerte : MonoBehaviour
{
    public Transform puntoRespawn;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscar el script del jugador de manera diferente
            MonoBehaviour jugador = other.GetComponent<MonoBehaviour>();
            
            // Si el jugador tiene un método "Morir", lo llamamos
            if (jugador != null && jugador.GetType().GetMethod("Morir") != null)
            {
                jugador.Invoke("Morir", 0f);
            }
            else
            {
                // Si no tiene método Morir, simplemente lo movemos al respawn
                other.transform.position = puntoRespawn.position;
                
                // Reiniciar velocidad si tiene Rigidbody2D
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }
}
