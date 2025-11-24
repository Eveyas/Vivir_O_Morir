using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCoins pc = other.GetComponent<PlayerCoins>();
        if (pc != null)
        {
            pc.AddCoins(value);
            Destroy(gameObject);
        }
    }
}
