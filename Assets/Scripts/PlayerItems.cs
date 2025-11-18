using UnityEngine;
using System.Collections;

public class PlayerItems : MonoBehaviour
{
    public PowerUpPickup.ItemType? currentItem = null;

    // Referencias: asigna desde el Inspector
    public Player1_Movimiento movementScript;     
    public Player1_Movimiento otherPlayerMovement;

    // Configuración
    public float boostDuration = 2f;
    public float boostMultiplier = 2f;
    public float slowDuration = 2f;
    public float slowMultiplier = 0.5f;

    bool boosting = false;
    bool slowing = false;

    public void GiveItem(PowerUpPickup.ItemType item)
    {
        currentItem = item;
        Debug.Log(gameObject.name + " recibió: " + item);
    }

    // --------------------------------------------------------------
    // 🎮 LLAMADO AUTOMÁTICO POR Player Input (Send Messages)
    // --------------------------------------------------------------
    public void OnUse(UnityEngine.InputSystem.InputValue value)
    {
        if (!value.isPressed) return;

        Debug.Log("USE DETECTADO en: " + gameObject.name);

        if (currentItem == null)
        {
            Debug.Log("No tienes item para usar");
            return;
        }

        UseItem();
    }

    void UseItem()
    {
        if (currentItem == PowerUpPickup.ItemType.Speed)
            StartCoroutine(DoSpeedBoost());
        else if (currentItem == PowerUpPickup.ItemType.Slow)
            StartCoroutine(DoSlowOpponent());

        currentItem = null;
    }

    IEnumerator DoSpeedBoost()
    {
        if (boosting) yield break;
        if (movementScript == null) yield break;

        boosting = true;
        float original = movementScript.velocidadMovimiento;
        movementScript.velocidadMovimiento = original * boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        movementScript.velocidadMovimiento = original;
        boosting = false;
    }

    IEnumerator DoSlowOpponent()
    {
        if (slowing) yield break;
        if (otherPlayerMovement == null) yield break;

        slowing = true;
        float original = otherPlayerMovement.velocidadMovimiento;
        otherPlayerMovement.velocidadMovimiento = original * slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        otherPlayerMovement.velocidadMovimiento = original;
        slowing = false;
    }
}
