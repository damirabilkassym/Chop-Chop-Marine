using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 1;

    void Start()
    {
        // Уничтожить пулю через 3 секунды, чтобы не засорять память
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Пуля летит только вперед
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если попали в игрока
        if (collision.CompareTag("Player"))
        {
            // Здесь будет логика получения урона игроком
            Destroy(gameObject);
        }
    }
}