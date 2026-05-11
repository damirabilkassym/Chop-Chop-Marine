using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Враг получил урон! Осталось HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        
        Destroy(gameObject);
    }
}