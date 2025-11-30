using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum ItemType { Speed, Slow }

    // Límite de tiempo antes de autodestruir (opcional)
    [Tooltip("Tiempo en segundos antes de que el item desaparezca. 0 o menos para que no desaparezca.")]
    public float lifeTime = 30f;

    void Start()
    {
        if (lifeTime > 0f) 
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Intentar obtener el componente del JUGADOR 1 (PlayerItems)
        // Usamos GetComponentInParent para manejar jerarquías de colliders.
        var playerItems = other.GetComponentInParent<PlayerItems>(); 
        
        // 2. Si no se encuentra, intentar obtener el componente del JUGADOR 2 (PlayerItems2)
        if (playerItems == null)
        {
            var playerItems2 = other.GetComponentInParent<PlayerItems2>();
            
            // Si encontramos el JUGADOR 2:
            if (playerItems2 != null)
            {
                GiveItemTo(playerItems2);
                return; 
            }
        }
        // Si encontramos el JUGADOR 1:
        else
        {
            GiveItemTo(playerItems);
            return;
        }

        // Si playerItems y playerItems2 son null, el objeto colisionado no es un jugador.
    }

    // Método para entregar el ítem y destruir el pickup (universalizado internamente)
    private void GiveItemTo(MonoBehaviour itemHandler)
    {
        // Elegir aleatoriamente Speed (0) o Slow (1)
        ItemType randomItem = (ItemType)Random.Range(0, 2);
        
        // El script itemHandler debe tener el método GiveItem.
        // Hacemos un casting seguro antes de llamar al método.
        if (itemHandler is PlayerItems p1Items)
        {
            p1Items.GiveItem(randomItem);
        }
        else if (itemHandler is PlayerItems2 p2Items)
        {
            p2Items.GiveItem(randomItem);
        }
        
        // Destruir el pickup una vez que ha sido recogido
        Destroy(gameObject);
    }
}