using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour
{
    public int health = 2;

    private bool isShaking = false;
    private Vector3 originalLocalPosition;

    void Start()
    {
        // Запоминаем точную стартовую локальную позицию один раз при старте
        originalLocalPosition = transform.localPosition;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Запускаем тряску, только если объект уже не трясется
        if (!isShaking && gameObject.activeInHierarchy)
        {
            StartCoroutine(HitShake());
        }

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

    IEnumerator HitShake()
    {
        isShaking = true;

        // Трясем относительно начальной локальной позиции
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * 0.05f;
            transform.localPosition = originalLocalPosition + randomOffset;
            yield return new WaitForSeconds(0.02f);
        }

        // Гарантированно возвращаем ящик строго на его законное место
        transform.localPosition = originalLocalPosition;
        isShaking = false;
    }
}