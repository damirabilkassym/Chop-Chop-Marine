using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement & Maneuvering")]
    public float moveSpeed = 3f;         // Скорость перемещения врага
    public float minAttackDistance = 4f; // Минимальная дистанция до игрока
    public float maxAttackDistance = 7f; // Максимальная дистанция до игрока
    public float positionChangeInterval = 2f; // Как часто враг меняет позицию для маневра

    [Header("Attack Burst Settings")]
    public GameObject projectilePrefab;  // Префаб пули
    public Transform firePoint;          // Точка вылета пули
    public float attackCooldown = 2f;    // Перезарядка между очередями
    public int shotsPerBurst = 3;        // Пуль в очереди
    public float timeBetweenShots = 0.2f;// Пауза между выстрелами в очереди

    [Header("Projectile Settings")]
    public float projectileSpeed = 8f;
    public int projectileDamage = 10;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;               // Ссылка на аниматор для управления анимациями
    private bool isShootingBurst = false;
    private float nextAttackTime = 0f;

    private Vector2 targetManeuverPoint; // Случайная точка маневра, куда сейчас бежит враг
    private float nextPositionChangeTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Ищем аниматор на самом враге или на его дочернем спрайте
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            // Выбираем стартовую точку для маневра
            UpdateManeuverPoint();
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. ЛОГИКА ДВИЖЕНИЯ И МАНЕВРОВ
        // Если пришло время сменить позицию или мы уже добежали до старой точки
        if (Time.time >= nextPositionChangeTime || Vector2.Distance(transform.position, targetManeuverPoint) < 0.5f)
        {
            UpdateManeuverPoint();
            // Задаем таймер на следующую смену точки + капля рандома, чтобы враги не двигались синхронно
            nextPositionChangeTime = Time.time + positionChangeInterval + Random.Range(-0.5f, 0.5f);
        }

        // Двигаем врага к его текущей точке маневра
        MoveToPoint(targetManeuverPoint);

        // --- УПРАВЛЕНИЕ АНИМАЦИЕЙ ---
        if (anim != null)
        {
            // Если скорость Rigidbody больше мелкой погрешности — включаем анимацию бега
            bool moving = rb.velocity.magnitude > 0.1f;
            anim.SetBool("isMoving", moving);
        }

        // Поворот спрайта строго в сторону игрока (куда бы враг ни бежал, смотрит на морпеха)
        Vector2 directionToPlayer = player.position - transform.position;
        if (directionToPlayer.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (directionToPlayer.x < 0) transform.localScale = new Vector3(-1, 1, 1);


        // 2. ЛОГИКА СТРЕЛЬБЫ ОЧЕРЕДЯМИ
        if (!isShootingBurst && Time.time >= nextAttackTime)
        {
            float currentDistance = Vector2.Distance(transform.position, player.position);
            // Стреляем, только если игрок в зоне видимости/досягаемости пуль
            if (currentDistance <= maxAttackDistance + 1f)
            {
                StartCoroutine(FireBurstSequence());
            }
        }
    }

    void MoveToPoint(Vector2 point)
    {
        Vector2 direction = (point - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    void UpdateManeuverPoint()
    {
        if (player == null) return;

        // Генерируем случайный угол вокруг игрока
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        // Выбираем случайную дистанцию в пределах наших настроек атак
        float randomDistance = Random.Range(minAttackDistance, maxAttackDistance);

        // Получаем итоговую точку на сцене, куда нужно бежать врагу
        targetManeuverPoint = (Vector2)player.position + randomDirection * randomDistance;
    }

    // Корутина для стрельбы очередью
    IEnumerator FireBurstSequence()
    {
        isShootingBurst = true;

        for (int i = 0; i < shotsPerBurst; i++)
        {
            Shoot();
            // Микропауза между выстрелами внутри одной серии
            yield return new WaitForSeconds(timeBetweenShots);
        }

        // К перезарядке тоже накидываем немного рандома, чтобы выстрелы ощущались естественнее
        nextAttackTime = Time.time + attackCooldown + Random.Range(-0.3f, 0.3f);
        isShootingBurst = false;
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Спавним пулю в точке firePoint
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDirection = (player.position - firePoint.position).normalized;

        // Запускаем её по Rigidbody
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.velocity = shootDirection * projectileSpeed;
        }

        // Передаем пуле урон и скорость напрямую из этого скрипта
        EnemyProjectile bulletScript = proj.GetComponent<EnemyProjectile>();
        if (bulletScript == null)
        {
            bulletScript = proj.AddComponent<EnemyProjectile>();
        }

        bulletScript.damage = projectileDamage;
        bulletScript.speed = projectileSpeed;
    }

    // Рисуем радиусы атак в окне Scene для удобной отладки баланса
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minAttackDistance);
            Gizmos.DrawWireSphere(player.position, maxAttackDistance);
        }
    }
}