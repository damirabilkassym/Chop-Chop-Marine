using UnityEngine;
using System.Collections;

public class WeaponRotation : MonoBehaviour
{
    [Header("Settings")]
    public Camera cam;
    public float swingSpeed = 20f;
    public float swingAngle = 90f;
    public int baseDamage = 1;

    [Header("References")]
    [SerializeField] private Transform weaponVisual;
    private PlayerRage rageSystem;

    private bool isAttacking = false;

    void Start()
    {
        if (weaponVisual == null) weaponVisual = transform.GetChild(0);
        rageSystem = GetComponentInParent<PlayerRage>();
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraShake.Instance.Shake(0.5f, 0.2f);
            Debug.Log("Тестовая тряска запущена!");
        }


        if (isAttacking) return;

        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(PerformAction(Quaternion.Euler(0, 0, -swingAngle), 0.1f));

        if (Input.GetMouseButtonDown(1))
            StartCoroutine(PerformAction(Quaternion.Euler(0, 0, 45f), 0.2f));
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Vector3 scale = weaponVisual.localScale;
        scale.x = (mousePos.x < transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        weaponVisual.localScale = scale;
    }

    IEnumerator PerformAction(Quaternion offset, float delay)
    {
        isAttacking = true;
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * offset;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(delay);
        transform.localRotation = startRot;
        isAttacking = false;
    }

    // РЕАКЦИЯ НА УДАР
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем по тегу, который ты создал
        if (collision.CompareTag("EnemyHitbox") || collision.CompareTag("Enemy"))
        {
            // 1. Применяем эффекты (тряска, хитстоп, ярость)
            ApplyHitEffects();

            // 2. Логика урона
            var enemyHealth = collision.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                int finalDamage = baseDamage;

                // Если ярость больше 70%, наносим двойной урон
                if (rageSystem != null && rageSystem.currentRage > 60f)
                {
                    finalDamage *= 2;
                    Debug.Log("<color=red>RAGE CRIT!</color>");
                    // Усиленная тряска для крита
                    if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.15f);
                }

                if (rageSystem != null && rageSystem.currentRage > 80f)
                {
                    finalDamage *= 3;
                    Debug.Log("<color=red>RAGE CRIT!</color>");
                    // Усиленная тряска для крита
                    if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.15f);
                }


                enemyHealth.TakeDamage(finalDamage);
            }
        }
    }

   
    private void ApplyHitEffects()
    {
        // Тряска камеры
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.15f, 0.1f);

        // Хит-стоп (замирание времени)
        if (HitStopManager.Instance != null)
            HitStopManager.Instance.DoHitStop(0.06f, 0.1f);

        // Добавление ярости
        if (rageSystem != null)
            rageSystem.AddRage(15f);
    }
}