using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    // 1. Configuración de Escenas (Verificar en Inspector)
    public int NIVEL_META_INDICE = 2;   // Nivel 2
    public int ESCENA_FINAL_INDICE = 3; // Final

    // 2. Variables de Tiempo Guardadas (PERSISTENTES)
    public float J1_TiempoNivel1 = 0f;
    public float J2_TiempoNivel1 = 0f;
    public float J1_TiempoNivel2 = 0f;
    public float J2_TiempoNivel2 = 0f;

    // 3. Variables de Estado y Contadores Actuales
    private bool J1_Corriendo = false;
    private bool J2_Corriendo = false;
    private float tiempoActual_J1 = 0f; // Tiempo actual de J1 en este nivel
    private float tiempoActual_J2 = 0f; // Tiempo actual de J2 en este nivel

    public int nivelActual = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------------------------------------
    // Lógica de Detección de Cambio de Escena (CRÍTICO)
    // ---------------------------------------------------------
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Se ejecuta CADA VEZ que carga una escena nueva
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        nivelActual = scene.buildIndex;
        Debug.Log($"[GTM] Escena Cargada: {scene.name} (Índice: {nivelActual})");

        // Si es Nivel 1 o Nivel 2, iniciamos contadores
        if (nivelActual == 1 || nivelActual == NIVEL_META_INDICE)
        {
            IniciarContadores();
        }
        else
        {
            // Apagamos contadores si estamos en Menú o Final
            J1_Corriendo = false;
            J2_Corriendo = false;
        }
    }
    // ---------------------------------------------------------

    void Update()
    {
        // El Contador: Suma tiempo si el jugador sigue corriendo
        if (J1_Corriendo) tiempoActual_J1 += Time.deltaTime;
        if (J2_Corriendo) tiempoActual_J2 += Time.deltaTime;
    }

    public void IniciarContadores()
    {
        tiempoActual_J1 = 0f;
        tiempoActual_J2 = 0f;
        J1_Corriendo = true;
        J2_Corriendo = true;
        Debug.Log("[GTM] ¡Contadores Iniciados!");
    }

    // Función llamada por la meta (FinishLine.cs)
    public void RegistrarLlegada(int idJugador)
    {
        // 1. Protección: Ignoramos si estamos en nivel 0
        if (nivelActual == 0) 
        {
            Debug.LogWarning("[GTM] Ignorando llegada en Nivel 0 (Menú).");
            return;
        }
        
        // 2. Detener y Guardar Tiempo
        if (idJugador == 1 && J1_Corriendo)
        {
            J1_Corriendo = false;
            GuardarTiempo(1, tiempoActual_J1);
            Debug.Log($"[GTM] Jugador 1 terminó. Tiempo: {tiempoActual_J1:0.00}s");
        }
        else if (idJugador == 2 && J2_Corriendo)
        {
            J2_Corriendo = false;
            GuardarTiempo(2, tiempoActual_J2);
            Debug.Log($"[GTM] Jugador 2 terminó. Tiempo: {tiempoActual_J2:0.00}s");
        }

        // 3. Revisar si ambos terminaron y avanzar
        if (!J1_Corriendo && !J2_Corriendo)
        {
            Debug.Log("[GTM] Ambos terminaron. Cambiando de nivel...");
            AvanzarNivel();
        }
    }

    private void AvanzarNivel()
    {
        if (nivelActual < NIVEL_META_INDICE)
        {
            SceneManager.LoadScene(nivelActual + 1);
        }
        else
        {
            SceneManager.LoadScene(ESCENA_FINAL_INDICE);
        }
    }

    private void GuardarTiempo(int idJugador, float tiempo)
    {
        if (nivelActual == 1)
        {
            if (idJugador == 1) J1_TiempoNivel1 = tiempo; else J2_TiempoNivel1 = tiempo;
        }
        else if (nivelActual == NIVEL_META_INDICE)
        {
            if (idJugador == 1) J1_TiempoNivel2 = tiempo; else J2_TiempoNivel2 = tiempo;
        }
    }
    
    // Función pública para obtener los resultados finales (para DisplayFinalTime.cs)
    public (float J1_Total, float J2_Total) ObtenerResultadosTotales()
    {
        float total_J1 = J1_TiempoNivel1 + J1_TiempoNivel2;
        float total_J2 = J2_TiempoNivel1 + J2_TiempoNivel2;
        return (total_J1, total_J2);
    }
}