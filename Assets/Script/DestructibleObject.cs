using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public int health = 2;

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Запускаем тряску самого ящика
        StartCoroutine(HitShake());

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        Debug.Log(gameObject.name + " разрушен!");
        Destroy(gameObject);
    }

    System.Collections.IEnumerator HitShake()
    {
        Vector3 origin = transform.position;
        for (int i = 0; i < 5; i++)
        {
            transform.position = origin + (Vector3)Random.insideUnitCircle * 0.05f;
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = origin;
    }
}