using UnityEngine;
using TMPro; // *** CAMBIO CRÍTICO: Usamos la librería TextMeshPro ***

public class DisplayFinalTime : MonoBehaviour
{
    // Arrastra los componentes TextMeshProUGUI aquí desde el Inspector
    public TextMeshProUGUI J1_TiempoTotalText;
    public TextMeshProUGUI J2_TiempoTotalText;
    public TextMeshProUGUI GanadorText;

    void Start()
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogError("ERROR: GameTimeManager no encontrado. Asegúrate de iniciar el juego desde el menú.");
            if (J1_TiempoTotalText != null) J1_TiempoTotalText.text = "Error: 0.00s";
            return;
        }
        
        MostrarTiempos();
    }

    private void MostrarTiempos()
    {
        (float totalJ1, float totalJ2) = GameTimeManager.Instance.ObtenerResultadosTotales();

        // 1. Mostrar tiempos
        if (J1_TiempoTotalText != null) 
            J1_TiempoTotalText.text = $"Jugador 1: {totalJ1:0.00}s";
        
        if (J2_TiempoTotalText != null)
            J2_TiempoTotalText.text = $"Jugador 2: {totalJ2:0.00}s";
        
        // 2. Determinar y mostrar al ganador
        if (GanadorText != null) 
        {
            if (totalJ1 < totalJ2)
            {
                GanadorText.text = "¡El Ganador es el Jugador 1!";
            }
            else if (totalJ2 < totalJ1)
            {
                GanadorText.text = "¡El Ganador es el Jugador 2!";
            }
            else
            {
                GanadorText.text = "¡Empate!";
            }
        }
    }
}