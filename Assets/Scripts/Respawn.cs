using UnityEngine;

public class Respawn : MonoBehaviour
{
    public float floorY;   // Altura del piso a donde regresará

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 pos = collision.transform.position;

            // Mantiene la X original, pero sube la Y al piso
            pos.y = floorY;

            collision.transform.position = pos;
        }
    }
}
