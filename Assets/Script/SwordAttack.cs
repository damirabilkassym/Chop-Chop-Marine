using UnityEngine;
using System.Collections.Generic;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float swingAngle = 90f;
    public float normalSpeed = 10f;
    public float rageSpeed = 16f;
    public int baseDamage = 1;
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Block/Absorb Settings")]
    public float parryDuration = 0.5f;  // Окно защиты увеличено до 0.5 секунд!
    public float parryCooldown = 0.6f;  // Перезарядка блока (можно прожимать часто)
    private float parryEndTime = 0f;
    private float nextParryTime = 0f;
    private bool isParrying = false;

    [Header("Rage & Color Settings")]
    public float rageThreshold = 70f;
    public Color normalColor = Color.white;
    public Color rageColor = new Color(1f, 0.35f, 0.35f);
    public Color blockColor = Color.green; // Цвет меча во время поглощения пуль

    private bool isAttacking = false;
    private float attackProgress = 0f;

    private Quaternion startRotation;
    private Collider2D attackCollider;
    private PlayerRage rageSystem;
    private SpriteRenderer swordSprite;

    private List<Collider2D> hitTargets = new List<Collider2D>();

    void Start()
    {
        rageSystem = GetComponentInParent<PlayerRage>();
        attackCollider = GetComponent<Collider2D>();
        swordSprite = GetComponent<SpriteRenderer>();

        startRotation = transform.localRotation;

        if (attackCollider != null)
        {
            attackCollider.isTrigger = true;
            attackCollider.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // Подсветка ярости работает только если мы не в блоке
        if (!isParrying && rageSystem != null && swordSprite != null)
        {
            float ragePercent = rageSystem.currentRage / rageSystem.maxRage;
            swordSprite.color = Color.Lerp(normalColor, rageColor, ragePercent);
        }

        // Атака (ЛКМ)
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isParrying && Time.time >= nextAttackTime)
        {
            StartAttack();
        }

        // Поглощение пуль / Блок (ПКМ)
        if (Input.GetMouseButtonDown(1) && !isAttacking && !isParrying && Time.time >= nextParryTime)
        {
            StartParry();
        }

        if (isAttacking) ProcessSwing();
        if (isParrying) ProcessParryTimers();
    }

    void StartAttack()
    {
        isAttacking = true;
        attackProgress = 0f;
        hitTargets.Clear();
        if (attackCollider != null) attackCollider.enabled = true;
        nextAttackTime = Time.time + attackCooldown;
    }

    void ProcessSwing()
    {
        float currentSpeed = normalSpeed;
        if (rageSystem != null && rageSystem.currentRage >= rageThreshold) currentSpeed = rageSpeed;

        attackProgress += Time.deltaTime * currentSpeed;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, -swingAngle);
        transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, attackProgress);

        if (attackProgress >= 1f) EndAttack();
    }

    void EndAttack()
    {
        isAttacking = false;
        transform.localRotation = startRotation;
        if (attackCollider != null) attackCollider.enabled = false;
        EvaluateClosestTarget();
    }

    // --- АКТИВАЦИЯ ПОГЛОЩЕНИЯ ---
    void StartParry()
    {
        isParrying = true;
        parryEndTime = Time.time + parryDuration;
        nextParryTime = Time.time + parryCooldown;

        if (attackCollider != null) attackCollider.enabled = true; // Включаем коллайдер как щит

        if (swordSprite != null)
        {
            swordSprite.color = blockColor; // Подсвечиваем меч зеленым (или любым защитным цветом)
        }
    }

    void ProcessParryTimers()
    {
        if (Time.time >= parryEndTime)
        {
            isParrying = false;
            if (attackCollider != null) attackCollider.enabled = false;
            if (swordSprite != null) swordSprite.color = normalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если мы защищаемся и в меч влетает пуля
        if (isParrying)
        {
            EnemyProjectile projectile = collision.GetComponent<EnemyProjectile>();
            if (projectile != null)
            {
                // Просто уничтожаем пулю врага («впитываем» её)
                Destroy(projectile.gameObject);

                // Даем приятный бонус к ярости за успешную защиту (+15 за пулю)
                if (rageSystem != null)
                {
                    rageSystem.AddRage(15f);
                    Debug.Log("<color=green>Пуля заблокирована! +15 ярости.</color>");
                }

                // Небольшой сочный стоп-кадр для ощущения импакта
                if (HitStopManager.Instance != null)
                {
                    HitStopManager.Instance.DoHitStop(0.04f, 0.06f);
                }
                return;
            }
        }

        // Сбор целей для обычной атаки
        if (isAttacking)
        {
            if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyHitbox") || collision.GetComponent<DestructibleObject>() != null)
            {
                if (!hitTargets.Contains(collision)) hitTargets.Add(collision);
            }
        }
    }

    void EvaluateClosestTarget()
    {
        if (hitTargets.Count == 0) return;
        Collider2D closestTarget = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (Collider2D target in hitTargets)
        {
            if (target == null) continue;
            float distance = Vector3.Distance(playerPosition, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            if (closestTarget.CompareTag("Enemy") || closestTarget.CompareTag("EnemyHitbox"))
            {
                int damageToDeal = baseDamage;
                if (rageSystem != null && rageSystem.currentRage >= rageThreshold)
                {
                    damageToDeal *= 2;
                }

                EnemyHealth health = closestTarget.GetComponentInParent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damageToDeal);
                    if (HitStopManager.Instance != null) HitStopManager.Instance.DoHitStop(0.07f, 0.1f);
                }
                if (rageSystem != null) rageSystem.AddRage(12f);
            }
            else
            {
                DestructibleObject destructible = closestTarget.GetComponent<DestructibleObject>();
                if (destructible != null) destructible.TakeDamage(1);
            }
        }
    }
}