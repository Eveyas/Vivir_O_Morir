using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMoverCholo : MonoBehaviour
{
    [Header("Movimiento")] 
    public float velocidadMax = 2.5f;
    public float cambioIntervalMin = 1f;
    public float cambioIntervalMax = 3f;

    [Header("Persecución")]
    public float chaseSpeed = 3.5f;      // Velocidad cuando persigue
    public float stopDistance = 0.5f;    // Para no empalmarse con el jugador

    [Header("Ataque")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float detectionRange = 5f;
    public float fireCooldown = 1.5f;
    private float lastFireTime = 0f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpForce = 3f;
    public float knockbackDuration = 0.5f;

    [Header("Ráfaga")]
    public int bulletsPerBurst = 3;       // cuántas balas por ráfaga
    public float burstDelay = 0.15f;      // tiempo entre balas de la ráfaga
    public float timeBetweenBursts = 2f;  // tiempo entre ráfagas
    private bool isShootingBurst = false;


    private Rigidbody2D rb;
    private int direccion = 1;
    private float nextCambioTime = 0f;

    private bool isFleeing = false;
    private Vector2 fleeDirection = Vector2.zero;

    private Transform targetPlayer = null; // Jugador más cercano

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direccion = Random.value > 0.5f ? 1 : -1;
        ScheduleNextCambio();
    }

    void Update()
    {
        DetectAndChasePlayer();   // detectar y perseguir
        DetectAndShootPlayers();
    }

    void FixedUpdate()
    {
        if (isFleeing)
        {
            rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);
            return;
        }

        // Si tiene un jugador objetivo lo persigue
        if (targetPlayer != null)
        {
            float dir = Mathf.Sign(targetPlayer.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);
            return;
        }

        // Movimiento aleatorio normal
        if (Time.time >= nextCambioTime)
        {
            direccion = Random.value > 0.5f ? 1 : -1;
            ScheduleNextCambio();
        }

        rb.linearVelocity = new Vector2(direccion * velocidadMax, rb.linearVelocity.y);
    }

    private void DetectAndChasePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            targetPlayer = null;
            return;
        }

        // Buscar jugador más cercano
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }

        // Si no está en rango → no perseguir
        if (closest == null || minDist > detectionRange)
        {
            targetPlayer = null;
            return;
        }

        // Si está muy cerca, no persigue
        if (minDist <= stopDistance)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            targetPlayer = closest;
            return;
        }

        targetPlayer = closest;
    }

    // Disparos
    private void DetectAndShootPlayers()
    {
        if (targetPlayer == null || isShootingBurst) return;

        float dist = Vector2.Distance(transform.position, targetPlayer.position);
        if (dist > detectionRange) return;

        if (Time.time >= lastFireTime + timeBetweenBursts)
        {
            StartCoroutine(ShootBurst());
            lastFireTime = Time.time;
        }
    }

    private IEnumerator ShootBurst()
    {
        isShootingBurst = true;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            ShootBullet(targetPlayer);
            yield return new WaitForSeconds(burstDelay);
        }

        isShootingBurst = false;
    }

    private void ShootBullet(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().SetTarget(target);
    }

    // KNOCKBACK
    private void ScheduleNextCambio()
    {
        nextCambioTime = Time.time + Random.Range(cambioIntervalMin, cambioIntervalMax);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var knockable = collision.collider.GetComponentInParent<IKnockbackable>();

        if (knockable != null)
        {
            Transform playerTransform = collision.collider.GetComponentInParent<Transform>();

            Vector2 dir = (playerTransform.position - transform.position);
            float signoX = dir.x == 0f ? -Mathf.Sign(transform.localScale.x) : Mathf.Sign(dir.x);
            Vector2 knock = new Vector2(signoX * knockbackForce, knockbackUpForce);

            knockable.Knockback(knock, knockbackDuration);

            Vector2 away = (transform.position - playerTransform.position).normalized;
            fleeDirection = away;

            if (!isFleeing)
                StartCoroutine(Flee(knockbackDuration));
        }
    }

    private IEnumerator Flee(float dur)
    {
        isFleeing = true;
        rb.linearVelocity = new Vector2(fleeDirection.x * velocidadMax, rb.linearVelocity.y);

        yield return new WaitForSeconds(dur);

        isFleeing = false;
        ScheduleNextCambio();
    }
}
