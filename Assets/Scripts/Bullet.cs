using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float seekDuration = 2f;
    private Transform target;
    private Vector2 moveDirection;
    private bool seeking = true;

    public void SetTarget(Transform player)
    {
        target = player;
        Destroy(gameObject, lifeTime);
        Invoke(nameof(StopSeeking), seekDuration);

        // Convertir target.position a Vector2
        if (target != null)
            moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        else
            moveDirection = transform.right;
    }

    private void StopSeeking()
    {
        seeking = false;

        if (target != null)
        {
            moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

            if (moveDirection == Vector2.zero)
                moveDirection = Vector2.right;
        }
    }

    void Update()
    {
        if (seeking && target != null)
        {
            moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        }

        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}