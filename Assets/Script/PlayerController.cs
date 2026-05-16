using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Получаем ввод движения (WASD или стрелочки)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Передаем направление в Blend Tree, только когда игрок идет
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);

            // Логика отзеркаливания: меняем только при движении по горизонтали
            if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true; // Разворот влево
            }
            else if (moveInput.x > 0)
            {
                spriteRenderer.flipX = false; // Разворот вправо
            }
        }

        // Передаем общую скорость, чтобы переключать Idle и Walk дерева
        float moveMagnitude = moveInput.sqrMagnitude;
        anim.SetFloat("Speed", moveMagnitude);
    }

    void FixedUpdate()
    {
        // Физическое перемещение Rigidbody2D с нормализацией вектора (чтобы не бегал быстрее по диагонали)
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}