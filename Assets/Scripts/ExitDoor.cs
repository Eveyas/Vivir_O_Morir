using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para la línea de Debug

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verificar si el objeto que entra es un jugador (usando el Tag "Player")
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Game Over] Jugador ha entrado a la Puerta Final. Terminando el juego.");

            // 2. Ejecutar la función para salir del juego
            TerminarJuego();
        }
    }

    private void TerminarJuego()
    {
        // Este es el comando universal para cerrar la aplicación o el juego.

        // NOTA: Application.Quit() solo funciona al construir (Build) el juego.
        // NO funcionará cuando lo pruebes en el Editor de Unity.
        Application.Quit();

        // En el Editor de Unity, la siguiente línea se usa para simular que funciona:
        #if UNITY_EDITOR
            Debug.Log("--- JUEGO TERMINADO --- (Esto solo se ve en el Editor, pero el juego se cerraría en la versión compilada)");
            // Opcional: Detener la reproducción del Editor
            UnityEditor.EditorApplication.isPlaying = false; 
        #endif

        // Opcional: Destruir el GameManager si ya no lo necesitas
        if (GameTimeManager.Instance != null)
        {
            Destroy(GameTimeManager.Instance.gameObject);
        }
    }
}