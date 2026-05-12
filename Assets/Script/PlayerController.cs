using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Эта переменная будет меняться скриптом PlayerRage

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Получаем ссылку на компонент физики
        rb = GetComponent<Rigidbody2D>();

        // Убедись, что гравитация выключена для Top-Down игры
        if (rb != null)
        {
            rb.gravityScale = 0f;
            // Чтобы персонаж не вращался при столкновениях
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Считываем ввод от игрока (WASD или стрелки)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Нормализуем вектор, чтобы по диагонали не бегал быстрее
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // Двигаем персонажа через физику
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }
}