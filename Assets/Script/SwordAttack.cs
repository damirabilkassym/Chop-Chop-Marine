using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.name == "Hitbox")
        {
            
            EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}