using UnityEngine;

public class PlayerRage : MonoBehaviour
{
    [Header("Rage Stats")]
    public float currentRage = 0f;
    public float maxRage = 100f;
    public float decayRate = 15f; // Сколько ярости теряется в секунду

    [Header("Buff Settings")]
    public float normalSpeed = 5f;
    public float rageSpeed = 9f;
    public Color normalColor = new Color(0.09f, 0.32f, 0.53f); // Твой синий
    public Color rageColor = Color.red;

    private PlayerController moveScript;
    private SpriteRenderer sprite;

    void Start()
    {
        moveScript = GetComponent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Постепенное затухание ярости
        if (currentRage > 0)
        {
            currentRage -= decayRate * Time.deltaTime;
        }

        ApplyEffects();
    }

    public void AddRage(float amount)
    {
        currentRage = Mathf.Clamp(currentRage + amount, 0, maxRage);
    }

    void ApplyEffects()
    {
        // Рассчитываем множитель ярости (от 0 до 1)
        float ragePercent = currentRage / maxRage;

        // Плавное изменение скорости и цвета
        if (moveScript != null)
        {
            moveScript.moveSpeed = Mathf.Lerp(normalSpeed, rageSpeed, ragePercent);
        }

        if (sprite != null)
        {
            sprite.color = Color.Lerp(normalColor, rageColor, ragePercent);
        }
    }
}