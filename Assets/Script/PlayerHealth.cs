using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Нужно для работы Корутин (задержки)

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Settings")]
    public GameObject gameOverScreen; // Наш Canvas Game Over

    private Animator anim;
    private PlayerController moveScript;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        moveScript = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Если уже мертв, урон не принимаем

        currentHealth -= damage;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.4f, 0.2f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Морпех погиб...");

        // 1. Отключаем управление и физику, чтобы мертвый морпех не бегал
        if (moveScript != null) moveScript.enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;

        // 2. Включаем триггер смерти в Аниматоре
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 3. Запускаем задержку перед появлением экрана Game Over
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // Ждем 1.5 секунды, пока проиграется анимация падения
        yield return new WaitForSeconds(1.5f);

        // Показываем экран смерти и останавливаем мир
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    void Update()
    {
        // Перезагрузка сцены на R, если игрок мертв
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}