using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float swingAngle = 90f;
    public float swingSpeed = 25f;
    public int baseDamage = 1;

    private bool isAttacking = false;
    private PlayerRage rageSystem;

    void Start()
    {
        rageSystem = GetComponentInParent<PlayerRage>();

        // Авто-настройка физики
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformSwing());
        }
    }

    IEnumerator PerformSwing()
    {
        isAttacking = true;
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -swingAngle);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.localRotation = startRot;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;

        // Отладка в консоль
        Debug.Log("Меч попал в: " + collision.name);

        // 1. Урон ВРАГУ
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyHitbox"))
        {
            int damageToDeal = baseDamage;
            if (rageSystem != null && rageSystem.currentRage > 70f)
            {
                damageToDeal *= 2;
            }

            var health = collision.GetComponentInParent<EnemyHealth>();
            if (health != null) health.TakeDamage(damageToDeal);

            if (rageSystem != null) rageSystem.AddRage(12f);
        }

        // 2. Урон ОБЪЕКТУ
        DestructibleObject destructible = collision.GetComponent<DestructibleObject>();
        if (destructible != null)
        {
            destructible.TakeDamage(1);
        }
    }
}