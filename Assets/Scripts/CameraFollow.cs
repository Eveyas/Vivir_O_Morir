using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Ajustes de Suavizado")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Límites de Cámara")]
    public float minX = 1f;  // evita que la cámara visualice atrás del jugador

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("¡ERROR! El campo 'Target' en CameraFollow no está asignado.");
            return;
        }

        // Calcula la posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Evita que la cámara se mueva a la izquierda del nivel
        desiredPosition.x = Mathf.Max(desiredPosition.x, minX);

        // 3. Suavizado
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicar posición
        transform.position = smoothedPosition;
    }
}

