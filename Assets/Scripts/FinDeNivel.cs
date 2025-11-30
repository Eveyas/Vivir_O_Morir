using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class FinDeNivel : MonoBehaviour
{
    // Asegúrate de que tu jugador tenga el Tag "Player" en el Inspector
    public string TagDelJugador = "Player"; 

    // **AJUSTA ESTE NÚMERO** al índice de tu escena "Ganaste"
    public int IndiceSiguienteEscena = 2; 

    // Esta función se llama automáticamente cuando otro collider entra en el Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprueba si el objeto que entró es el jugador (usando su Tag)
        if (other.CompareTag(TagDelJugador))
        {
            Debug.Log("¡Nivel Terminado! Cargando escena: " + IndiceSiguienteEscena);
            
            // Carga la escena final
            SceneManager.LoadScene(IndiceSiguienteEscena);
        }
    }
}