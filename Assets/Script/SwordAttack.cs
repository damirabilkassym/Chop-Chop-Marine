using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int damage = 1;
    public float ragePerHit = 20f;

    
    private PlayerRage rageSystem;

    void Awake()
    {
        
        rageSystem = GetComponentInParent<PlayerRage>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("EnemyHitbox") || collision.gameObject.name == "Hitbox")
        {
            
            EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                
                if (rageSystem != null)
                {
                    rageSystem.AddRage(ragePerHit);
                }
            }
        }
    }
}