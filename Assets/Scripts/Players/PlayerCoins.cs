using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    public int coins = 0;

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Monedas: " + coins);
    }
}
