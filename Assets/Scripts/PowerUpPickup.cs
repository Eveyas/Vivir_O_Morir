using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum ItemType { Speed, Slow }

    // Límite de tiempo antes de autodestruir (opcional)
    public float lifeTime = 30f;

    void Start()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Intentamos obtener el componente PlayerItems del jugador que colisiona
        PlayerItems playerItems = other.GetComponent<PlayerItems>();
        if (playerItems == null) return;

        // Elegir aleatoriamente Speed o Slow
        ItemType randomItem = (ItemType)Random.Range(0, 2);
        playerItems.GiveItem(randomItem);

        // Destruir pickup (o desactivarlo si quieres respawn manual)
        Destroy(gameObject);
    }
}
