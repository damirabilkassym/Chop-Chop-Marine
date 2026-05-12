using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    void Update()
    {
        // Пуля летит вперед
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Попадание в разрушаемый объект (ящик)
        DestructibleObject box = collision.GetComponent<DestructibleObject>();
        if (box != null)
        {
            // Пуля просто исчезает, ящик она не ломает (его ломаешь только ты мечом)
            Destroy(gameObject);
            return;
        }

        // 2. Попадание в игрока
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // 3. Попадание в стены (если есть тег Wall)
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}