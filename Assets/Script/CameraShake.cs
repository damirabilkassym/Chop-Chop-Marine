using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Синглтон для легкого доступа
    public static CameraShake Instance { get; private set; }

    // Стартовая позиция камеры, чтобы знать, куда возвращаться
    private Vector3 originalPosition;

    // Внутренняя переменная для отслеживания тряски
    private bool isShaking = false;

    void Awake()
    {
        // Настройка синглтона
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Запоминаем исходную позицию (обычно это 0,0,-10)
        originalPosition = transform.localPosition;
    }

    // Главный метод для вызова тряски
    // magnitude: сила тряски, duration: длительность в секундах
    public void Shake(float magnitude, float duration)
    {
        // Если уже трясет, не запускаем новую (опционально)
        if (isShaking) return;

        StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Генерируем случайное смещение
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Применяем смещение к исходной позиции
            // Сохраняем originalPosition.z, чтобы камера не улетела
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            // Ждем следующего кадра
            yield return null;
        }

        // Возвращаем камеру строго в исходную позицию
        transform.localPosition = originalPosition;
        isShaking = false;
    }
}