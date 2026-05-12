using UnityEngine;
using UnityEngine.SceneManagement; // Нужно для перезагрузки игры

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Settings")]
    public GameObject gameOverScreen; // Сюда перетащим наш Canvas

    void Start()
    {
        currentHealth = maxHealth;
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false); // Прячем экран при старте
    }

    public void TakeDamage(int damage)
    {
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
        Debug.Log("Морпех погиб...");

        // 1. Показываем экран Game Over
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        // 2. Останавливаем время в игре
        Time.timeScale = 0f;
    }

    // Метод для перезагрузки (можно вызвать кнопкой или клавишей)
    void Update()
    {
        if (currentHealth <= 0 && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f; // Обязательно возвращаем время в норму!
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}