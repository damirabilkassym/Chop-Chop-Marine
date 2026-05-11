using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Rigidbody2D enemyRb; // Ссылка на Rigidbody2D врага
    public Vector2 minBounds;   // Границы комнаты (X min, Y min)
    public Vector2 maxBounds;   // Границы комнаты (X max, Y max)

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform player;
    public float shootingRange = 10f;
    public float fireRate = 2f;

    private Vector2 targetPosition;
    private float changeTargetTime = 2f;
    private float timer;
    private float nextFireTime;

    void Start()
    {
        // Если забыли прикрепить компоненты в инспекторе, скрипт попробует найти их сам
        if (enemyRb == null) enemyRb = GetComponent<Rigidbody2D>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        SetNewRandomTarget();
    }

    void Update()
    {
        // Таймер для смены случайной цели
        timer += Time.deltaTime;
        if (timer >= changeTargetTime)
        {
            SetNewRandomTarget();
            timer = 0;
        }

        // Логика стрельбы (остается в Update)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= shootingRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Для физического перемещения используем FixedUpdate
    void FixedUpdate()
    {
        if (targetPosition != null)
        {
            // Вычисляем направление к цели
            Vector2 direction = (targetPosition - enemyRb.position).normalized;

            // Проверка: если мы еще не дошли до точки
            if (Vector2.Distance(enemyRb.position, targetPosition) > 0.2f)
            {
                // Двигаем Rigidbody. Теперь он будет сталкиваться с коллайдерами стен
                enemyRb.MovePosition(enemyRb.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void SetNewRandomTarget()
    {
        // Выбираем случайную точку строго внутри заданных границ комнаты
        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        targetPosition = new Vector2(randomX, randomY);
    }

    void Shoot()
    {
        if (bulletPrefab != null && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }
    }
}