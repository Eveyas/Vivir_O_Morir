using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Objeto que la cámara seguirá. ¡Arrastra aquí a tu jugador (player1_0)!
    [Header("Objetivo")]
    public Transform target; 

    // Velocidad con la que la cámara intentará alcanzar al objetivo. 
    [Header("Ajustes de Suavizado")]
    [Range(0.01f, 1.0f)] // Limita el valor en el Inspector
    public float smoothSpeed = 0.125f; 

    // El offset (desplazamiento) en Z es crítico para 2D. 
    // La cámara debe estar detrás del plano de juego (ej: -10).
    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); 

    void LateUpdate()
    {
        // 🚨 CRÍTICO: Comprueba que el objetivo (target) ha sido asignado.
        if (target == null)
        {
            Debug.LogError("¡ERROR! El campo 'Target' en el script CameraFollow no está asignado. Arrastra al jugador a la Main Camera.");
            return; // Detiene la función si no hay objetivo
        }

        // 1. Calcular la posición deseada de la cámara.
        // Mantiene el mismo Z que el offset (ej: -10).
        Vector3 desiredPosition = target.position + offset; 
        
        // 2. Aplicar un suavizado (Lerp)
        // Lerp mueve la posición actual (transform.position) hacia la posición deseada 
        // a una velocidad constante (smoothSpeed).
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // 3. Asignar la posición suavizada a la cámara.
        transform.position = smoothedPosition;
    }
}