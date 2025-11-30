using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verificamos si es un jugador
        if (!other.CompareTag("Player")) return;

        // 2. Obtenemos el ID del jugador
        PlayerID1 id1 = other.GetComponent<PlayerID1>();
        PlayerID2 id2 = other.GetComponent<PlayerID2>();

        int idJugador = 0;
        if (id1 != null) idJugador = 1;
        else if (id2 != null) idJugador = 2;

        // 3. Enviamos la señal al Manager
        if (idJugador != 0 && GameTimeManager.Instance != null)
        {
            // El Manager internamente verifica si el jugador ID ya terminó (J1_Corriendo) 
            // y si el nivel actual es válido.
            GameTimeManager.Instance.RegistrarLlegada(idJugador);
            
            // CRÍTICO: No desactivamos el objeto aquí para que el otro jugador pueda tocarlo.
        }
    }
}