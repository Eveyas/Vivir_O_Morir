using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class MenuManager : MonoBehaviour
{
    // Esta función se llama al presionar el botón "Play"
    public void IniciarJuego()
    {
        // El número '1' corresponde al índice de la primera escena de nivel
        // que colocaste en la ventana de Build Settings (debajo del menú, índice 0).
        SceneManager.LoadScene(1);
        
        // Si prefieres usar el nombre de la escena en lugar del índice, 
        // puedes usar esta línea (después de cambiar el nombre):
        // SceneManager.LoadScene("NombreDeTuPrimeraEscena");
    }

    // Opcional: Para salir del juego (útil si tienes un botón de "Salir")
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        // Esto solo funciona cuando el juego está compilado (no en el editor de Unity)
        Application.Quit();
    }
}