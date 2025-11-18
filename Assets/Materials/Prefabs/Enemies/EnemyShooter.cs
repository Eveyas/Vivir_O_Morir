using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireRate = 1.2f;
    public float detectionRange = 10f;

    private Transform targetPlayer;
    private float nextFireTime;

    void Update()
    {
        DetectPlayer();
        AimAtPlayer();
        Shoot();
    }

    void DetectPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDist = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (var p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPlayer = p.transform;
            }
        }

        if (closestDist <= detectionRange)
            targetPlayer = closestPlayer;
        else
            targetPlayer = null;
    }

    void AimAtPlayer()
    {
        if (targetPlayer == null) return;

        Vector3 dir = targetPlayer.position - transform.position;

        if (dir.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void Shoot()
    {
        if (targetPlayer == null) return;

        if (Time.time > nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }
}
