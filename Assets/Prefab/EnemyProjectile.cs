using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public float speed;

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Попадание в игрока
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        // Попадание в разрушаемый ящик
        DestructibleObject destructible = collision.GetComponent<DestructibleObject>();
        if (destructible != null)
        {
            destructible.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        // Попадание в стены
        if (collision.CompareTag("Obstacle") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}