using UnityEngine;

public class PlayerStun : MonoBehaviour
{
    public float stunTime = 0.25f; // 250 ms, menos que la cadencia del disparo

    private bool isStunned = false;
    private float stunTimer = 0f;

    private PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0)
            {
                isStunned = false;
                controller.enabled = true;
            }
        }
    }

    public void ApplyStun()
    {
        isStunned = true;
        stunTimer = stunTime;
        controller.enabled = false;
    }
}
